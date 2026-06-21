using Domain.Entity;

namespace Domain.Messages;

public class Chat : ModifyEntity<ChatUpdateDto>
{
    public string Title { get; private set; }

    public static Chat Create(string title) => new()
    {
        Title = title
    };

    public override void Update(ChatUpdateDto dto)
    {
        Title = dto.Title;
    }
}

public record ChatUpdateDto(string Title) : UpdateDto;