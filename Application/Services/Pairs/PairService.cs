using Domain;
using Domain.Groups;
using Domain.Pairs;
using Domain.Users;

namespace Application.Services.Pairs;

public class PairService(
    IPairRepository pairRepo,
    IUserRepository userRepo,
    IGroupRepository groupRepo)
{
    public void Create(PairCreateDto[] dtos)
    {
        var existingPairs = pairRepo.Read(dtos.Select(x1 => x1.Date));
        var teachers = userRepo.Read(dtos.Select(x1 => x1.TeacherId).ToArray())
            .Select(x => x)
            .ToArray();

        List<Pair> createdPairs = [];
        foreach (var dto in dtos)
        {
            var pair = Pair.Create(
                dto.GroupId,
                dto.Date,
                dto.CabinetNumber,
                dto.TeacherId,
                dto.Discipline,
                "white");

            pair.NotContainsConflict(teachers, existingPairs.Concat(createdPairs).ToArray());

            createdPairs.Add(pair);
        }

        pairRepo.Create(createdPairs.ToArray());
    }

    public void Create(PairParseModel[] models)
    {
        var teachersByInitials = userRepo.ReadTeachers()
            .ToDictionary(x => x.Initials, x => x);

        foreach (var model in models)
            if (!teachersByInitials.TryGetValue(model.TeacherInitials, out _))
                throw new Exception($"Мы не знаем преподавателя {model.TeacherInitials}, " +
                                    $"указанного в паре {model.Date.ToStringFormat()} у {model.GroupName}");

        var groups = groupRepo
            .Read(models.Select(x => x.GroupName).ToArray())
            .ToDictionary(x => x.Name, x => x.Id);

        var dtos = models
            .Select(model => model.ToDto(teachersByInitials[model.TeacherInitials].Id, groups[model.GroupName]))
            .ToArray();

        Create(dtos);
    }

    public PairReadDto[] Read(GetPairModel model)
    {
        var user = userRepo.Read(model.UserId!.Value);
        if (user == null)
            throw new Exception("Пользователь не найден");

        if (user.Roles.Contains(Role.Teacher))
            return pairRepo.ReadForTeacher(model);

        if (user.Roles.Contains(Role.Student))
            return pairRepo.Read(model, user.GroupId!.Value);

        throw new Exception("У пользователя нет пар или нет доступа к парам");
    }
}

public record PairCreateDto(
    string Discipline,
    Guid TeacherId,
    string CabinetNumber,
    DateTime Date,
    Guid GroupId);

public record PairParseModel(
    string Discipline,
    string TeacherInitials,
    string CabinetNumber,
    DateTime Date,
    string GroupName)
{
    public PairCreateDto ToDto(Guid teacherId, Guid groupId) => new(
        Discipline,
        teacherId,
        CabinetNumber,
        Date,
        groupId);
}



