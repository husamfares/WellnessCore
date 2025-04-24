using System;

namespace API.Entities;

public class RecoveryRecord
{
    //public int Id { get; set; }
     public int UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public double RecoveryPercentage { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
}
