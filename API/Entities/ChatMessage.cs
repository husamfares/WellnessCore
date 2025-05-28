using System;

namespace API.Entities;

public class ChatMessage
{
     public int Id { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; } = "user"; // or "assistant"
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
