using Catalog.Application;
using Catalog.Persistence;
using Shared;

var builder = WebApplication.CreateBuilder(args);

// Спільна інфраструктура — GlobalExceptionHandler, ProblemDetails
builder.Services.AddSharedInfrastructure();

// Catalog: MediatR + Behaviors (Logging → Validation → Transaction) + FluentValidation + Mapster
builder.Services.AddCatalogApplication();

// Catalog: DbContext, UnitOfWork
builder.Services.AddCatalogPersistence(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

// Активує GlobalExceptionHandler для перетворення виключень у ProblemDetails
app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();
