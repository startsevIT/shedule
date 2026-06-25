using Domain.Entity;

namespace Domain.Marks;

public record MarkUpdateDto(
    Guid? TeacherId,
    Guid? StudentId,
    Guid? ObjectId,
    MarkType? MarkType,
    int? Value) : UpdateDto;