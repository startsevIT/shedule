using Domain.Entity;

namespace Domain.Users;

public record UserUpdateDto(
    string? FirstName,
    string? Name,
    string? Patronymic,
    HashSet<string>? AddedRoles,
    HashSet<string>? RemovedRoles,
    Guid? GroupId = null) : UpdateDto;