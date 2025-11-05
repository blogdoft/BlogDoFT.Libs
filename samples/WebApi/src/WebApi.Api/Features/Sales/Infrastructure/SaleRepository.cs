using BlogDoFT.Libs.ResultPattern;
using Microsoft.EntityFrameworkCore;
using WebApi.Api.Domain.Sales;
using WebApi.Api.Features.Sales.Infrastructure.Models;

namespace WebApi.Api.Features.Sales.Infrastructure;

public class SaleRepository : ISaleRepository
{
    private readonly SalesDbContext _context;

    public SaleRepository(SalesDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> AddAsync(SaleEntity entity)
    {
        var model = SaleModel.From(entity);
        await _context.Sales.AddAsync(model);
        await _context.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<Result<SaleEntity>> GetByIdAsync(Guid id)
    {
        var model = await _context.Sales
            .AsNoTracking()
            .Include(s => s.Items)
            .FirstOrDefaultAsync(sale => sale.NavigationId == id);

        if (model is null)
        {
            return Failure.DataNotFound with
            {
                Message = $"Does not exist a sale with id {id}",
            };
        }

        return model.ToEntity();
    }
}
