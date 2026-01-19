using BoligInfo.Core.Models;
using BoligInfo.Database;
using Microsoft.EntityFrameworkCore;

namespace BoligInfo.HouseRepository;

/// <summary>
/// Repository implementation for managing <see cref="House"/> entities
/// using Entity Framework Core.
/// </summary>
public class HouseRepository(BoligInfoDbContext context) : IHouseRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<House>> GetAllAsync()
    {
        return await context.Houses.ToListAsync();
    }
    
    /// <inheritdoc />
    public async Task<House?> GetByIdAsync(long id)
    {
        return await context.Houses.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<House>> GetByEquityIdAsync(long equityId)
    {
        return await context.Houses
            .Where(h => h.EquityId == equityId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<House?> GetByIdWithCashFlowsAsync(long id)
    {
        return await context.Houses
            .Include(h => h.CashFlows)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    /// <inheritdoc />
    public async Task<House> AddAsync(House house)
    {
        context.Houses.Add(house);
        await context.SaveChangesAsync();
        return house;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(House house)
    {
        context.Houses.Update(house);
        await context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(long id)
    {
        var house = await context.Houses.FindAsync(id);
        if (house != null)
        {
            context.Houses.Remove(house);
            await context.SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(long id)
    {
        return await context.Houses.AnyAsync(h => h.Id == id);
    }
}