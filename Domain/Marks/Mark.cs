using Domain.Entity;

namespace Domain.Marks;

public class Mark : ModifyEntity<MarkUpdateDto>
{
    private Guid TeacherId { get; set; }
    private Guid StudentId { get; set; }
    private Guid ObjectId { get; set; }
    private MarkType MarkType { get; set; }
    private int Value { get; set; }


    public static Mark Create(
        Guid teacherId,
        Guid studentId,
        Guid objectId,
        MarkType markType,
        int value)
        => new()
        {
            TeacherId = teacherId,
            StudentId = studentId,
            ObjectId = objectId,
            MarkType = markType,
            Value = value
        };

    public override void Update(MarkUpdateDto dto)
    {
        TeacherId = dto.TeacherId ?? TeacherId;
        StudentId = dto.StudentId ?? StudentId;
        ObjectId = dto.ObjectId ?? ObjectId;
        MarkType = dto.MarkType ?? MarkType;
        Value = dto.Value ?? Value;
    }
}

public record MarkUpdateDto(
    Guid? TeacherId,
    Guid? StudentId,
    Guid? ObjectId,
    MarkType? MarkType,
    int? Value) : UpdateDto;