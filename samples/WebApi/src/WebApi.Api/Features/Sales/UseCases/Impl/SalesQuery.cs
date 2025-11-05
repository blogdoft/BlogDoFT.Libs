using BlogDoFT.Libs.DomainNotifications;
using BlogDoFT.Libs.DomainNotifications.Extensions;
using BlogDoFT.Libs.ResultPattern;
using WebApi.Api.Features.Sales.Models;

namespace WebApi.Api.Features.Sales.UseCases.Impl;

public class SalesQuery : ISalesQuery
{
    private readonly IDomainNotifications _notifications;
    private readonly ISaleRepository _repository;

    public SalesQuery(
        IDomainNotifications notifications,
        ISaleRepository repository)
    {
        _notifications = notifications;
        _repository = repository;
    }

    public async Task<Result<SaleResponse>> GetByIdAsync(Guid id)
    {
        var entityResult = await _repository.GetByIdAsync(id);

        if (entityResult.IsFailure)
        {
            _notifications.Add(entityResult);
            return entityResult.Failure;
        }

        var responseResult = SaleResponse.From(entityResult.Value);


        if (responseResult.IsFailure)
        {
            _notifications.Add(responseResult);
            return responseResult.Failure;
        }

        return responseResult.Value;
    }
}
