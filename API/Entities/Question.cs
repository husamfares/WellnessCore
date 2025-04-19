using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities;

public class Question
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string? Caption { get; set; }
    [Required]
    public string? AskedBy { get; set; }  // Username
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<Answer> Answers { get; set; } = new();
}
