namespace Domain.Messages.Messages;

public class Message : Entity.Entities.Entity
{
    public Guid ChatId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Text { get; private set; } = null!;

    public static Message Create(Guid chatId, Guid authorId, string text) => new()
    {
        ChatId = chatId,
        AuthorId = authorId,
        Text = text,
    };
}