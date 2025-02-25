﻿namespace Infinity.Toolkit.Messaging.Diagnostics;

internal class ClientDiagnostics(string system, string broker, string? serverAddress)
{
    private readonly TagList tags = [
            new(DiagnosticProperty.MessagingSystem, system),
            new(DiagnosticProperty.MessagingTransport, broker),
            new(DiagnosticProperty.ServerAddress, serverAddress)
        ];

    public ClientDiagnostics(string system, string broker, string channelName, string serverAddress)
        : this(system, broker, serverAddress)
    {
        tags = [.. tags, .. new KeyValuePair<string, object?>[] { new(DiagnosticProperty.MessagingDestinationName, channelName) }];
    }

    /// <summary>
    /// Creates a new diagnostic activity scope for the message bus.
    ///
    /// Catch span id and trace id from the current activity. These are needed when resolving message handlers.
    /// If the current activity is null, create a new activity. This can happen when the message is sent from a background service.
    /// If the current activity exists use the id from the current activity. Otherwise we create a new activity.
    /// </summary>
    /// <param name="kind">The kind of activity.</param>
    /// <param name="operationName">The name of the operation.</param>
    /// <param name="operationType">The type of operation.</param>
    /// <param name="properties">Optional properties to add to the activity.</param>
    /// <returns>An <see cref="Activity"/> instance.</returns>
    internal Activity? CreateDiagnosticActivityScope(ActivityKind kind, string operationName, string operationType, IDictionary<string, object?> properties)
    {
        var activity = Activity.Current;

        if (activity is not null)
        {
            var currentActivity = Activity.Current;
            ActivityContext.TryParse(currentActivity?.Id, currentActivity?.TraceStateString, out var activityContext);
            activity = MessageBusActivitySource.StartActivity(operationName, kind, activityContext, tags);
            properties.TryAdd(DiagnosticProperty.MessageBusTraceParent, currentActivity?.Id);
            properties.TryAdd(DiagnosticProperty.MessageBusTraceState, activityContext.TraceState);
        }
        else
        {
            if (!TryExtractRootActivityContext(properties, out var rootActivityContext))
            {
                activity = MessageBusActivitySource.StartActivity(operationName, kind, rootActivityContext, tags);
                properties.TryAdd(DiagnosticProperty.MessageBusTraceParent, activity?.Id);
                properties.TryAdd(DiagnosticProperty.MessageBusTraceState, rootActivityContext.TraceState);
            }
            else
            {
                TryExtractTraceContext(properties, out var activityContext);
                activity = MessageBusActivitySource.StartActivity(operationName, kind, activityContext, tags);
            }
        }

        activity?.SetTag(DiagnosticProperty.MessagingOperation, operationName);
        activity?.SetTag(DiagnosticProperty.MessagingOperationName, operationName);
        activity?.SetTag(DiagnosticProperty.MessagingOperationType, operationType);

        return activity;
    }

    /// <summary>
    /// Creates a new diagnostic activity scope for a message handler.
    /// </summary>
    /// <param name="channelName">The name of the channel.</param>
    /// <param name="messageHandlerType">The type of the message handler.</param>
    /// <param name="properties">Optional properties to add to the activity.</param>
    /// <returns>An <see cref="Activity"/> instance.</returns>
    internal Activity? CreateDiagnosticActivityScopeForMessageHandler(string channelName, Type messageHandlerType, IDictionary<string, object?> properties)
    {
        TryExtractRootActivityContext(properties, out var rootActivityContext);
        var activityContext = Activity.Current?.Context ?? rootActivityContext;

        var activity = MessageBusActivitySource.StartActivity($"{DiagnosticProperty.OperationConsume} {messageHandlerType}", ActivityKind.Consumer, activityContext, tags, links: [new ActivityLink(rootActivityContext)]);

        activity?.SetTag(DiagnosticProperty.MessagingDestinationName, channelName);
        activity?.SetTag(DiagnosticProperty.MessagingOperation, DiagnosticProperty.OperationConsume);
        activity?.SetTag(DiagnosticProperty.MessagingOperationName, DiagnosticProperty.OperationConsume);
        activity?.SetTag(DiagnosticProperty.MessagingOperationType, DiagnosticProperty.OperationProcess);
        activity?.SetTag(DiagnosticProperty.MessageBusMessageHandler, messageHandlerType.FullName ?? messageHandlerType.Name);

        return activity;
    }

    internal static bool TryExtractRootActivityContext(IDictionary<string, object?> properties, out ActivityContext activityContext)
    {
        var traceparent = string.Empty;
        var tracestate = string.Empty;

        if (properties.TryGetValue(DiagnosticProperty.MessageBusTraceParent, out var traceparentValue) && traceparentValue is string traceparentString)
        {
            traceparent = traceparentString;
            if (properties.TryGetValue(DiagnosticProperty.MessageBusTraceState, out var tracestateValue) && tracestateValue is string tracestateString)
            {
                tracestate = tracestateString;
            }
        }

        return ActivityContext.TryParse(traceparent, tracestate, out activityContext);
    }

    internal static bool TryExtractTraceContext(IDictionary<string, object?> properties, out ActivityContext activityContext)
    {
        var traceparent = string.Empty;
        var tracestate = string.Empty;

        if (properties.TryGetValue(DiagnosticProperty.TraceParent, out var traceparentValue) && traceparentValue is string traceparentString)
        {
            traceparent = traceparentString;
            if (properties.TryGetValue(DiagnosticProperty.TraceState, out var tracestateValue) && tracestateValue is string tracestateString)
            {
                tracestate = tracestateString;
            }

            return ActivityContext.TryParse(traceparent, tracestate, out activityContext);
        }

        // Fallback to DiagnosticId
        if (properties.TryGetValue(DiagnosticProperty.DiagnosticId, out var diagnosticId) && diagnosticId is string diagnosticIdString)
        {
            return ActivityContext.TryParse(diagnosticIdString, tracestate, out activityContext);
        }

        // Fallback to root activity context
        if (TryExtractRootActivityContext(properties, out activityContext))
        {
            return ActivityContext.TryParse(traceparent, tracestate, out activityContext);
        }

        return false;
    }
}
