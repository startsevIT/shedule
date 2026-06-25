using Domain.Entity;

namespace Domain.Messages.ChatUsers;

public record ChatUserUpdateDto(
    ChatRoleEnum? Role) : UpdateDto;