using System;

namespace API.Dtos;

public class ChatResponseDto
{
    public List<ChatMessageDto> Messages { get; set; } = new();
}
