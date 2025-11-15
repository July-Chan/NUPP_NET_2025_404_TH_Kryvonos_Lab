using Microsoft.EntityFrameworkCore;
using PublicTransit.Common.App.Crud;
using PublicTransit.Infrastructure;
using PublicTransit.Infrastructure.Models;
using PublicTransit.Infrastructure.Repositories;
using PublicTransit.REST.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PublicTransitContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        "Host=localhost;Port=5432;Database=PublicTransitDb;Username=postgres;Password=PublicT_s&c*ret_34z";
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure();
    });
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
    options.ConfigureWarnings(warnings =>
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
