using Domain;
using Domain.Entity;
using Domain.Groups;
using Domain.Pairs;
using Domain.Users;

namespace Application.Services.Pairs;

public class PairService(
    EntityStorage entityStorage)
{
    public void Create(PairCreateDto[] dtos)
    {
        var dates = dtos.Select(x => x.Date).ToArray();
        var minDate = dates.Min();
        var maxDate = dates.Max();
        var existingPairs = entityStorage.Select<Pair>()
            .Where(x => x.Date >= minDate)
            .Where(x => x.Date <= maxDate);
        var teachers = entityStorage.Read<User>(dtos.Select(x1 => x1.TeacherId).ToArray())
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

        entityStorage.Create(createdPairs.ToArray());
    }

    public void Create(PairParseModel[] models)
    {
        var teachersByInitials = entityStorage.Select<User>().Where(x => x.Roles.Contains(Role.Teacher))
            .ToDictionary(x => x.Initials, x => x);

        foreach (var model in models)
            if (!teachersByInitials.TryGetValue(model.TeacherInitials, out _))
                throw new Exception($"Мы не знаем преподавателя {model.TeacherInitials}, " +
                                    $"указанного в паре {model.Date.ToStringFormat()} у {model.GroupName}");

        var groups = entityStorage
            .Select<Group>()
            .Where(x => models.Select(y => y.GroupName).Contains(x.Name))
            .ToDictionary(x => x.Name, x => x.Id);

        var dtos = models
            .Select(model => model.ToDto(teachersByInitials[model.TeacherInitials].Id, groups[model.GroupName]))
            .ToArray();

        Create(dtos);
    }

    public PairReadDto[] Read(GetPairModel model)
    {
        var user = entityStorage.Read<User>(model.UserId!.Value);
        if (user == null)
            throw new Exception("Пользователь не найден");



        if (user.Roles.Contains(Role.Teacher))
        {
            var groups = entityStorage.Select<Group>()
                .ToDictionary(x => x.Id, x => x.Name);

            return entityStorage.Select<Pair>()
                .Where(x => x.TeacherId == user.Id)
                .Where(x => x.Date <= model.End)
                .Where(x => x.Date >= model.Start)
                .Select(x => new PairReadDto(
                    x.Discipline,
                    user.FullName,
                    x.CabinetNumber,
                    groups[x.GroupId],
                    x.Date, x.Color))
                .ToArray();
        }

        if (user.Roles.Contains(Role.Student))
        {
            var groupName = entityStorage.Select<Group>()
                .Single(x => x.Id == user.GroupId!.Value)
                .Name;

            var teachers = entityStorage.Select<User>()
                .Where(x => x.Roles.Contains(Role.Teacher))
                .ToDictionary(x => x.Id, x => x.FullName);

            return entityStorage.Select<Pair>()
                .Where(x => x.GroupId == user.GroupId!.Value)
                .Where(x => x.Date <= model.End)
                .Where(x => x.Date >= model.Start)
                .Select(x => new PairReadDto(
                    x.Discipline,
                    teachers[x.TeacherId],
                    x.CabinetNumber,
                    groupName,
                    x.Date, x.Color))
                .ToArray();
        }

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