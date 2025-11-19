using BlogDoFT.Libs.ResultPattern;
using Microsoft.AspNetCore.Mvc;
using WebApi.Api.Features.Sales.Models;
using WebApi.Api.Features.Sales.UseCases;

namespace WebApi.Api.Features.Sales;

public static class SalesController
{
    public static IEndpointRouteBuilder MapSales(this IEndpointRouteBuilder app)
    {
        var controller = app
            .MapGroup("/sales")
            .WithTags("Sales");

        controller.MapPost("/", async (
            [FromServices] ISalesCreate create,
            [FromBody] SalesCreateRequest request) =>
        {
            var result = await create
                .ExecuteAsync(request)
                .MapAsync(
                    onSuccess: value => Results.Ok(value),
                    onFailure: failure => Results.BadRequest(failure));
        })
        .WithName($"{nameof(SalesController)}_Post")
        .WithSummary("Register sale")
        .WithDescription("Register a new sale onto database");

        controller.MapGet("/{id:guid}", async (
            [FromServices] ISalesQuery query,
            [FromRoute] Guid id) =>
        {
            var result = await query.GetByIdAsync(id);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Failure);
            }

            return Results.Ok(result.Value);
        })
        .WithName($"{nameof(SalesController)}_Get")
        .WithSummary("Return sale")
        .WithDescription("Returns sale's details");

        return app;
    }
}
