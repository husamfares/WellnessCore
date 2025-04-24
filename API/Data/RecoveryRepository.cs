using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class RecoveryRepository(DataContext context) : IRecoveryRepository
{
    public async Task AddRecoveryAsync(RecoveryRecord record)
    {
        context.RecoveryRecords.Add(record);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<RecoveryRecord>> GetUserRecoveriesAsync(int userId)
    {
        return await context.RecoveryRecords
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.Date)
            .ToListAsync();
    }
}
