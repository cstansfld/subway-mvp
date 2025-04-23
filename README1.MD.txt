namespace Api.Settings;

public sealed class JwtAuthOptions
{
    public string Key { get; init; } // Key-Vault or User-Secrets
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public int ExpirationInMinutes { get; init; }
    public int RefreshTokenExpirationDays { get; init; }

    public const string SectionName = "Jwt";
}

/*
  "Jwt": {
    "Key": "your-secret-key-here-should-be-fairly-long",
    "Issuer": "dev-havbit.api",
    "Audience": "dev-havbit.app",
    "ExpirationInMinutes": 30,
    "RefreshTokenExpirationDays": 7
  }
*/
Add-Migration Add_Habits -o Migrations/Application
Add-Migration Add_Tags -o Migrations/Application
Add-Migration Add_HabitTags -o Migrations/Application
Add-Migration Add_Users -o Migrations/Application
Add-Migration Add_Identity -context ApplicationIdentityDbContext -o Migrations/Identity
Add-Migration Add_RefreshTokens -context ApplicationIdentityDbContext -o Migrations/Identity
Add-Migration Add_UserIdReference -context ApplicationDbContext -o Migrations/Application

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Api.Entities;

public sealed class MachineClient
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string ClientId { get; set; } = Guid.CreateVersion7().ToString();
    public string Name { get; set; }
    public string AllowedRole { get; set; }
    public string Secret { get; set; } // hashed secret new Secret("your_secret".Sha256())
    public string AllowedGrantType { get; set; } // GrantTypes.ClientCredentials;
    public string AllowedScopes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<MachineClientRole> Roles { get; set; } = [];
    public ICollection<ResourcePermission> Permissions { get; set; } = [];
}

public class MachineClientRole
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string RoleName { get; set; }
    public string ClientId { get; set; }
    public MachineClient Client { get; set; }
}

public class ResourcePermission
{

    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string ResourceName { get; set; }  // Example: "Invoices"
    public string AllowedAction { get; set; } // Example: "Read", "Write", "ReadWrite", "Delete", "Any"
    public string ClientId { get; set; }
    public MachineClient Client { get; set; }
}

namespace Api.Entities;

public sealed class User
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>
    /// We'll use this to store the IdentityId from the Identity Provider
    /// </summary>
    public string IdentityId { get; set; }
}

namespace Api.Entities;

public static class Roles
{
    public const string Admin = nameof(Admin);
    public const string Member = nameof(Client);
}

namespace Api.Entities;

public static class Permisission
{
    public const string Admin = nameof(Admin);todo
    public const string Member = nameof(Client);todo
}

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Api.Entities;

namespace Api.Database.Configurations;

public class MachineClientConfiguration : IEntityTypeConfiguration<MachineClient>
{
    public void Configure(EntityTypeBuilder<MachineClient> builder)
    {
        builder.HasKey(c => c.ClientId);

        builder.Property(c => c.ClientId)
            .IsRequired()
            .HasMaxLength(100);

        // Store hashed client secret securely
        builder.Property(c => c.Secret)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(c => c.AllowedScopes)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.AllowedGrantType)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasMany(c => c.Roles)
            .WithOne(r => r.Client)
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Permissions)
            .WithOne(p => p.Client)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services;

/// <summary>
/// https://www.bing.com/videos/riverview/relatedvideo?q=ClaimTypes+for+permission+jwt&mid=C988EC538FA677F00903C988EC538FA677F00903&mcid=924E517712F1492E90BA6784846CD1C9&FORM=VIRE
/// </summary>
/// <param name="options"></param>
internal sealed class JwtProvider(IOptions<JwtAuthOptions> options)
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    public string GenerateJwtToken(string clientId, string scope, IEnumerable<string> permissions, IEnumerable<string> roles)
    {
        var permissionStrings = new HashSet<string>(permissions);
        // await GetPermissionsAsync(clientId, scope);

        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, clientId),
            new (CustomClaims.Scope, scope)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(permissionStrings.Select(permission => new Claim(CustomClaims.Permissions, permission)));

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthOptions.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtAuthOptions.Issuer,
            audience: _jwtAuthOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
1.
public class MachineClient
{
    [Key]
    public string ClientId { get; set; }

    public string ClientSecretHash { get; set; } // Store a hashed secret

    public string AllowedScopes { get; set; }

    public ICollection<MachineClientRole> Roles { get; set; } = new List<MachineClientRole>();
    public ICollection<ResourcePermission> Permissions { get; set; } = new List<ResourcePermission>();
}
This entity allows you to assign roles and permissions just like IdentityUser, but for non-human clients.

2. Define Roles for Machine Clients
csharp
public class MachineClientRole
{
    [Key]
    public int Id { get; set; }

    public string RoleName { get; set; }

    [ForeignKey("ClientId")]
    public string ClientId { get; set; }

    public MachineClient Client { get; set; }
}
3. Define Permissions for Machine Clients
csharp
public class ResourcePermission
{
    [Key]
    public int Id { get; set; }

    public string ResourceName { get; set; }  // Example: "Invoices"
    public string AllowedAction { get; set; } // Example: "Read", "Write"

    [ForeignKey("ClientId")]
    public string ClientId { get; set; }

    public MachineClient Client { get; set; }
}
4. Register in DbContext
Modify ApplicationDbContext to support MachineClient authentication:

csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<MachineClient> MachineClients { get; set; }
    public DbSet<MachineClientRole> MachineClientRoles { get; set; }
    public DbSet<ResourcePermission> ResourcePermissions { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MachineClient>()
            .HasMany(c => c.Roles)
            .WithOne(r => r.Client)
            .HasForeignKey(r => r.ClientId);

        modelBuilder.Entity<MachineClient>()
            .HasMany(c => c.Permissions)
            .WithOne(p => p.Client)
            .HasForeignKey(p => p.ClientId);
    }
}
5. Generate JWT Tokens for Machine Clients
Modify token generation logic to embed client roles and permissions:

csharp
public static string GenerateJwtToken(MachineClient client, List<string> roles, List<ResourcePermission> permissions)
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key"));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
    {
        new Claim("client_id", client.ClientId),
        new Claim("scope", client.AllowedScopes),
    };

    // Add role claims
    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    // Add resource permissions as claims
    claims.AddRange(permissions.Select(p => new Claim($"permission_{p.ResourceName}", p.AllowedAction)));

    var token = new JwtSecurityToken(
        issuer: "your_issuer",
        audience: "your_audience",
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
6. Secure API Based on MachineClient Roles & Permissions
Modify authorization policies without relying on IdentityUser:

csharp
services.AddAuthorization(options =>
{
    options.AddPolicy("CanReadInvoices", policy =>
        policy.Requirements.Add(new ResourcePermissionRequirement("Invoices", "Read")));
});

services.AddSingleton<IAuthorizationHandler, ResourcePermissionHandler>();
Secure an API endpoint using machine-client permissions:

csharp
[Authorize(Policy = "CanReadInvoices")]
[HttpGet("invoices")]
public IActionResult GetInvoices()
{
    return Ok(new { message = "Client has permission to READ invoices." });
}
7. Testing the Workflow
7.1 Insert Sample Clients and Permissions
csharp
var client = new MachineClient
{
    ClientId = "machine_client_1",
    ClientSecretHash = BCrypt.Net.BCrypt.HashPassword("super_secret"),
    AllowedScopes = "your_api_scope"
};

var role = new MachineClientRole
{
    ClientId = client.ClientId,
    RoleName = "Admin"
};

var permission = new ResourcePermission
{
    ClientId = client.ClientId,
    ResourceName = "Invoices",
    AllowedAction = "Read"
};

_dbContext.MachineClients.Add(client);
_dbContext.MachineClientRoles.Add(role);
_dbContext.ResourcePermissions.Add(permission);
_dbContext.SaveChanges();
7.2 Request a Token
Modify token request logic to validate MachineClient:

csharp
public IActionResult GetToken([FromBody] ClientAuthRequest request)
{
    var client = _dbContext.MachineClients
        .Include(c => c.Roles)
        .Include(c => c.Permissions)
        .SingleOrDefault(c => c.ClientId == request.ClientId);

    if (client == null || !BCrypt.Net.BCrypt.Verify(request.ClientSecret, client.ClientSecretHash))
        return Unauthorized();

    var roles = client.Roles.Select(r => r.RoleName).ToList();
    var permissions = client.Permissions.ToList();

    var token = GenerateJwtToken(client, roles, permissions);
    return Ok(new { access_token = token });
}
7.3 Make an Authenticated Request
csharp
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
var response = await client.GetAsync("https://your-api/invoices");

