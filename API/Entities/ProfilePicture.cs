namespace API.Entities;

public class ProfilePicture
{
<<<<<<< HEAD
=======
   
>>>>>>> 837ce00eef0038d2e16e120ec65a5b1c8d86be79
    public int Id { get; set; }
    public required string Url { get; set; }
    public int UserId { get; set; }
    public AppUser? User { get; set; }
}
