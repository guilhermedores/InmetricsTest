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
        var group = app.MapGroup("/v1/operation");

        group.MapPost("/credit", async ([FromServices] IMediatorHandler handler, [FromServices] INotificationHandler<DomainNotification> notifications, [FromBody] MakeCreditOperationRequest request) =>
        {
            await handler.SendCommand(new MakeCreditOperationCommand(request.OperationCode, request.Value));

            var _notifications = (DomainNotificationHandler) notifications;
            
            if(!_notifications.HasNotifications())
            {
                return Results.Created();
            }
            else
            {
                return Results.BadRequest(_notifications.GetNotifications());
            }
        })
        .WithOpenApi()
        .WithName("MakeCreditOperation")
        .Produces((int)HttpStatusCode.Created)
        .Produces((int)HttpStatusCode.BadRequest);

        group.MapPost("/debit", async ([FromServices] IMediatorHandler handler, [FromServices] INotificationHandler<DomainNotification> notifications, [FromBody] MakeDebitOperationRequest request) =>
        {
            await handler.SendCommand(new MakeDebitOperationCommand(request.OperationCode, request.Value));

            var _notifications = (DomainNotificationHandler)notifications;

            if (!_notifications.HasNotifications())
            {
                return Results.Created();
            }
            else
            {
                return Results.BadRequest(_notifications.GetNotifications());
            }
        })
        .WithOpenApi()
        .WithName("MakeDebitOperation")
        .Produces((int)HttpStatusCode.Created)
        .Produces((int)HttpStatusCode.BadRequest);

        return app;
    }
}
