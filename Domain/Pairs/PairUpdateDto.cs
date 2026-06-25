using Domain.Entity;

namespace Domain.Pairs;

public record PairUpdateDto(
    string? Discipline,
    Guid? TeacherId,
    string? CabinetNumber,
    DateTime? Date,
    Guid? GroupId) : UpdateDto;