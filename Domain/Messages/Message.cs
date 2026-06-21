using Domain.Entity;

namespace Domain.Messages;

public class Message : ModifyEntity<MessageUpdateDto>
{
    public Guid ChatId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Text { get; private set; } = null!;

    public static Message Create(Guid chatId, Guid authorId, string text) => new()
    {
        ChatId = chatId,
        AuthorId = authorId,
        Text = text
    };

    public override void Update(MessageUpdateDto dto)
    {
        Text = dto.Text;
    }
}

public record MessageUpdateDto(string Text) : UpdateDto;