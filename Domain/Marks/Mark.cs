using Domain.Entity;
using Domain.Entity.Entities;

namespace Domain.Marks;

public class Mark : AutoModifyEntity<MarkUpdateDto>
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
}