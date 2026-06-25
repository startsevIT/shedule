using Domain.Messages.ChatUsers;

namespace Domain.Messages.Chats;

public record ChatUserCreateDto(
    Guid UserId,
    ChatRoleEnum Role);