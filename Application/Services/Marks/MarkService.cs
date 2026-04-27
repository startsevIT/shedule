using Domain.Marks;
using Domain.Pairs;
using Domain.Users;

namespace Application.Services.Marks;

public class MarkService(
    IMarkRepository markRepository,
    IUserRepository userRepository,
    IPairRepository pairRepository)
{
    public void Rate(MarkCreateDto dto)
    {
        var teacher = userRepository.Read(dto.TeacherId);
        if (teacher == null)
            throw new Exception("Преподаватель не найден");

        var student = userRepository.Read(dto.StudentId);
        if (student == null)
            throw new Exception("Студент не найден");

        if (dto.MarkType == MarkType.Pair)
        {
            var pair = pairRepository.Read(dto.ObjectId);
            if (pair == null)
                throw new Exception("Такая пара не найдена");
        }

        markRepository.Create(dto.ToMark());
    }
}

public record MarkCreateDto(
    Guid TeacherId,
    Guid StudentId,
    Guid ObjectId,
    MarkType MarkType,
    int Value)
{
    public Mark ToMark()
        => Mark.Create(TeacherId, StudentId, ObjectId, MarkType, Value);
};