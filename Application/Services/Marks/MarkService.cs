using Domain.Entity;
using Domain.Marks;
using Domain.Pairs;
using Domain.Users;

namespace Application.Services.Marks;

public class MarkService(EntityStorage entityStorage)
{
    public void Rate(MarkCreateDto dto)
    {
        var teacher = entityStorage.Read<User>(dto.TeacherId);
        if (teacher == null)
            throw new Exception("Преподаватель не найден");

        var student = entityStorage.Read<User>(dto.StudentId);
        if (student == null)
            throw new Exception("Студент не найден");

        if (dto.MarkType == MarkType.Pair)
        {
            var pair = entityStorage.Read<Pair>(dto.ObjectId);
            if (pair == null)
                throw new Exception("Такая пара не найдена");
        }

        entityStorage.Create(Mark.Create(dto.TeacherId, dto.StudentId, dto.ObjectId, dto.MarkType, dto.Value));
    }
}

public record MarkCreateDto(
    Guid TeacherId,
    Guid StudentId,
    Guid ObjectId,
    MarkType MarkType,
    int Value);