using BoligInfo.Core.Models;
using BoligInfo.Database;
using Microsoft.EntityFrameworkCore;

namespace BoligInfo.AddressRepository;

public class AddressRepository(BoligInfoDbContext context) : IAddressRepository
{
    public async Task<IEnumerable<Address>> GetAllAsync()
    {
        return await context.Addresses.ToListAsync();
    }

    public async Task<Address?> GetByIdAsync(long id)
    {
        return await context.Addresses.FindAsync(id);
    }

    public async Task<Address?> GetByHouseIdAsync(long houseId)
    {
        return await context.Addresses
            .FirstOrDefaultAsync(a => a.HouseId == houseId);
    }

    public async Task<Address> AddAsync(Address address)
    {
        context.Addresses.Add(address);
        await context.SaveChangesAsync();
        return address;
    }

    public async Task UpdateAsync(Address address)
    {
        context.Addresses.Update(address);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var address = await context.Addresses.FindAsync(id);
        if (address != null)
        {
            context.Addresses.Remove(address);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await context.Addresses.AnyAsync(a => a.Id == id);
    }
}