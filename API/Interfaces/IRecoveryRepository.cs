using API.Entities;

namespace API.Interfaces;

public interface IRecoveryRepository
{
    Task AddRecoveryAsync(RecoveryRecord record);
    Task<IEnumerable<RecoveryRecord>> GetUserRecoveriesAsync(int userId);
}
