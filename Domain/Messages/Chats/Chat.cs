using Domain.Entity;
using Domain.Entity.Entities;

namespace Domain.Messages.Chats;

public class Chat : AutoModifyEntity<ChatUpdateDto>
{
    public string Title { get; private set; } = null!;

    public static Chat Create(string title) => new()
    {
        Title = title
    };
}