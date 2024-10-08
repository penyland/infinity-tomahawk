﻿using Infinity.Toolkit.Mediator;

namespace MediatorSample;

internal record ProductCreated(int Id, string Name) : ICommand;

internal record ProductCreatedResult(string NewName);

internal class ProductCreatedHandler : ICommandHandler<ProductCreated>
{
    private readonly IPipeline<ProductCreated, ProductCreatedResult> pipeline;

    public ProductCreatedHandler(IPipeline<ProductCreated, ProductCreatedResult> pipeline)
    {
        this.pipeline = pipeline;
    }

    public async ValueTask HandleAsync(ProductCreated command, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("ProductCreatedHandler:HandleAsync");
        Console.WriteLine($"Product created: {command.Id} - {command.Name}");

        Console.WriteLine("Executing pipeline");
        var result = await pipeline.ExecuteAsync(command, cancellationToken);
        Console.WriteLine($"Pipeline executed: {result.NewName}");
    }
}

public static class ProductCreatedPipeline
{
    internal static IPipeline<ProductCreated, ProductCreatedResult> CreatePipeline(IServiceProvider services) => new Pipeline<ProductCreated, ProductCreatedResult>()
    .AddStep<ProductCreated, string>(input =>
    {
        Console.WriteLine("Step 1");
        Console.WriteLine($"Product created: {input.Id} - {input.Name}");
        var result = input.Name + " - Modified";
        return result;
    })
    .AddStep<string, ProductCreatedResult>(input =>
    {
        Console.WriteLine("Step 2");
        Console.WriteLine($"Product modified: {input}");
        return new ProductCreatedResult(input);
    })
    .Build();
}
