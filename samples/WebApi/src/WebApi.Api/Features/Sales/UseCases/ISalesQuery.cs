using BlogDoFT.Libs.ResultPattern;
using WebApi.Api.Features.Sales.Models;

namespace WebApi.Api.Features.Sales.UseCases;

public interface ISalesQuery
{
    Task<Result<SaleResponse>> GetByIdAsync(Guid id);
}
