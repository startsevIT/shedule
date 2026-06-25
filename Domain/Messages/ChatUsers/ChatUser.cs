using Domain.Entity;
using Domain.Entity.Entities;

namespace Domain.Messages.ChatUsers;

public class ChatUser : AutoModifyEntity<ChatUserUpdateDto>
{
    public Guid ChatId { get; private set; }
    public Guid UserId { get; private set; }
    public ChatRoleEnum Role { get; private set; }

    public static ChatUser Create(Guid chatId, Guid userId, ChatRoleEnum role) => new()
    {
        ChatId = chatId,
        UserId = userId,
        Role = role
    };
}