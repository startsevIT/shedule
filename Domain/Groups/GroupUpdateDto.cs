using Domain.Entity;

namespace Domain.Groups;

public record GroupUpdateDto(
    string Name) : UpdateDto;