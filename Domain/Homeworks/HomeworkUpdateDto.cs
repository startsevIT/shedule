using Domain.Entity;

namespace Domain.Homeworks;

public record HomeworkUpdateDto(
    Guid? TeacherId,
    string? Title,
    string? Description,
    DateTime? StartDate,
    DateTime? EndDate,
    HomeworkStatus? Status) : UpdateDto;