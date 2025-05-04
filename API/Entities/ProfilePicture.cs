namespace API.Entities;

public class ProfilePicture
{
   // public int Id { get; set; }
    public required string Url { get; set; }
    public int UserId { get; set; }
    public AppUser? User { get; set; }
}
