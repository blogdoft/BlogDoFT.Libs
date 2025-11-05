using BlogDoFT.Libs.ResultPattern;
using WebApi.Api.Features.Sales.Models;

namespace WebApi.Api.Features.Sales.UseCases;

public interface ISalesCreate
{
    Task<Result<Guid>> ExecuteAsync(SalesCreateRequest request);
}
