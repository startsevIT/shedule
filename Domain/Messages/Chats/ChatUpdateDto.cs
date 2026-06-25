using Domain.Entity;

namespace Domain.Messages.Chats;

public record ChatUpdateDto(
    string? Title,
    List<ChatUserCreateDto>? AddedUsers,
    List<Guid>? RemovedUsersIds) : UpdateDto;