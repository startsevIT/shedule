namespace Domain.Messages.Messages;

public record MessageReadDto(
    string Author,
    string Text,
    DateTime CreatedAt);