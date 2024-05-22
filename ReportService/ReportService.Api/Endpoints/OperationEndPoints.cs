using Domain.Entities;
using Domain.UseCases;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShareLib;
using System.Net;

public static partial class EndPointsExtensions
{
    public static WebApplication MapOperationEndPoints(this WebApplication app)
    {
        var group = app.MapGroup("/v1/report");

        group.MapGet("/", async (IGenericRepository<Operation> repository) =>
        {
            var operations = repository.GetAll();
            return Results.Ok(operations);
        })
        .WithOpenApi()
        .WithName("GetOperationReport")
        .Produces((int)HttpStatusCode.Created)
        .Produces((int)HttpStatusCode.BadRequest);

        return app;
    }
}
