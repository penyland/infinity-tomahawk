using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Infinity.Toolkit.TestUtils;

public static class IntegrationTestDefaults
{
    public const string AuthenticationScheme = "IntegrationTest";
}

public static class WebApplicationFactoryExtensions
{
    public static WebApplicationFactory<T> WithAuthentication<T>(this WebApplicationFactory<T> factory, Action<IServiceCollection>? action = null, string authenticationScheme = IntegrationTestDefaults.AuthenticationScheme)
        where T : class
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(authenticationScheme)
                        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(authenticationScheme, op => { });

                services
                    .AddAuthorization()
                    .AddScoped<TestClaimsProvider>(_ => TestClaimsProvider.TestUser("TestUser"));

                action?.Invoke(services);
            });
        });
    }
}

public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly TestClaimsProvider claimsProvider;

    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        TestClaimsProvider claimsProvider)
        : base(options, logger, encoder)
    {
        this.claimsProvider = claimsProvider ?? throw new ArgumentNullException(nameof(claimsProvider));
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(claimsProvider.Claims, "TestUser");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, IntegrationTestDefaults.AuthenticationScheme);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}
public class TestClaimsProvider
{
    public TestClaimsProvider(IList<Claim> claims)
    {
        Claims = claims;
    }

    public TestClaimsProvider()
    {
        Claims = [];
    }

    public IList<Claim> Claims { get; }

    public static TestClaimsProvider TestUser(string name)
    {
        var provider = new TestClaimsProvider();
        provider.Claims.Add(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));
        provider.Claims.Add(new Claim(ClaimTypes.Name, name));

        return provider;
    }

    public static TestClaimsProvider TestUserWithRole(string name, string[] roleNames, bool defaultInboundRoleClaimType = false)
    {
        var provider = TestUser(name);

        var claimTypeRoles = defaultInboundRoleClaimType ?
            ClaimTypes.Role : "roles";

        foreach (var role in roleNames)
        {
            provider.Claims.Add(new Claim(claimTypeRoles, role));
        }

        return provider;
    }
}
