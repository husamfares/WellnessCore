namespace API.Entities;

public class ProfilePicture
{
<<<<<<< HEAD
=======
   
>>>>>>> 564c8e85b8a961b04f49e00213a87a2014b9aafa
    public int Id { get; set; }
    public required string Url { get; set; }
    public int UserId { get; set; }
    public AppUser? User { get; set; }
}
