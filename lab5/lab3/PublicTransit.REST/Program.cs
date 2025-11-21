using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PublicTransit.Common.App.Crud;
using PublicTransit.Infrastructure;
using PublicTransit.Infrastructure.Models;
using PublicTransit.Infrastructure.Repositories;
using PublicTransit.REST.Constants;
using PublicTransit.REST.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PublicTransitContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Host=localhost;Port=5432;Database=PublicTransitDb;Username=postgres;Password=somepass";
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure();
    });
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
    options.ConfigureWarnings(warnings =>
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

builder.Services.AddIdentity<UserModel, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<PublicTransitContext>()
.AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] 
    ?? throw new InvalidOperationException("JWT SecretKey не налаштовано в appsettings.json");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Для розробки, в production має бути true
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "PublicTransitAPI",
        ValidAudience = jwtSettings["Audience"] ?? "PublicTransitAPI",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero // Видаляє затримку для перевірки часу закінчення
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFlyRepository, FlyRepository>();
builder.Services.AddScoped<ISpiderRepository, SpiderRepository>();
builder.Services.AddScoped<IInsectRepository, InsectRepository>();

builder.Services.AddScoped<IRepository<HabitatModel>, Repository<HabitatModel>>();
builder.Services.AddScoped<IRepository<FoodSourceModel>, Repository<FoodSourceModel>>();
builder.Services.AddScoped<IRepository<PredatorModel>, Repository<PredatorModel>>();

builder.Services.AddScoped<ICrudServiceAsync<FlyModel>>(sp =>
{
    var repository = sp.GetRequiredService<IFlyRepository>();
    var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
    return new RepositoryCrudServiceAsync<FlyModel>(repository, unitOfWork);
});

builder.Services.AddScoped<ICrudServiceAsync<SpiderModel>>(sp =>
{
    var repository = sp.GetRequiredService<ISpiderRepository>();
    var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
    return new RepositoryCrudServiceAsync<SpiderModel>(repository, unitOfWork);
});

builder.Services.AddScoped<ICrudServiceAsync<HabitatModel>>(sp =>
{
    var repository = sp.GetRequiredService<IRepository<HabitatModel>>();
    var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
    return new RepositoryCrudServiceAsync<HabitatModel>(repository, unitOfWork);
});

builder.Services.AddScoped<ICrudServiceAsync<FoodSourceModel>>(sp =>
{
    var repository = sp.GetRequiredService<IRepository<FoodSourceModel>>();
    var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
    return new RepositoryCrudServiceAsync<FoodSourceModel>(repository, unitOfWork);
});

builder.Services.AddScoped<ICrudServiceAsync<PredatorModel>>(sp =>
{
    var repository = sp.GetRequiredService<IRepository<PredatorModel>>();
    var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
    return new RepositoryCrudServiceAsync<PredatorModel>(repository, unitOfWork);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { Roles.User, Roles.Editor, Roles.Admin };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
