using BlogDoFT.Libs.DomainNotifications;
using BlogDoFT.Libs.DomainNotifications.Extensions;
using BlogDoFT.Libs.ResultPattern;
using WebApi.Api.Domain.Sales;
using WebApi.Api.Domain.Sales.ValueObjects;
using WebApi.Api.Features.Sales.Models;

namespace WebApi.Api.Features.Sales.UseCases.Impl;

public class SalesCreate : ISalesCreate
{
    private readonly IDomainNotifications _notifications;
    private readonly ISaleRepository _saleRepository;

    public SalesCreate(
        IDomainNotifications notifications,
        ISaleRepository saleRepository)
    {
        _notifications = notifications;
        _saleRepository = saleRepository;
    }

    public async Task<Result<Guid>> ExecuteAsync(SalesCreateRequest request)
    {
        return await CreateEntityFrom(request)
            .TapFailure(_notifications.Add)
            .ThenAsync(SaveSale)
            .TapFailureAsync(_notifications.Add);
    }

    private Result<SaleEntity> CreateEntityFrom(SalesCreateRequest request) =>
        new SaleFactory()
            .WithClient(Client.Create(request.Client.Id, request.Client.Name))
            .WithPaymentMethod(request.PaymentMethod)
            .WithInstallments(request.Installments)
            .WithSaleItem(request.Items.Select(item => SaleItemEntity.Create(
                product: Product.Create(item.Product.Id, item.Product.Description),
                quantity: item.Quantity,
                unitValue: item.UnitValue)))
            .Build();

    private Task<Result<Guid>> SaveSale(SaleEntity entity) =>
        _saleRepository.AddAsync(entity);
}
