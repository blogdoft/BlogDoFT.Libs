using BlogDoFT.Libs.ResultPattern;
using WebApi.Api.Domain.Sales;

namespace WebApi.Api.Features.Sales;

public interface ISaleRepository
{
    Task<Result<Guid>> AddAsync(SaleEntity entity);
    Task<Result<SaleEntity>> GetByIdAsync(Guid id);
}
