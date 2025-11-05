using BlogDoFT.Libs.DomainNotifications;
using Microsoft.EntityFrameworkCore;
using WebApi.Api.Features.Sales;
using WebApi.Api.Features.Sales.Infrastructure;
using WebApi.Api.Features.Sales.UseCases;
using WebApi.Api.Features.Sales.UseCases.Impl;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services
    .AddSwaggerGen()
    .AddDbContext<SalesDbContext>()
    .AddScoped<ISalesCreate, SalesCreate>()
    .AddScoped<ISalesQuery, SalesQuery>()
    .AddScoped<ISaleRepository, SaleRepository>()
    .AddDomainNotification();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "123Vendas API v1");
        c.RoutePrefix = "swagger";
    });
}

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<SalesDbContext>();
await dbContext.Database.MigrateAsync();

app.MapSales();

app.Run();
