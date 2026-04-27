using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entity;
using Domain.Users;

namespace Domain.Pairs;

[Table("pairs")]
public class Pair : ModifyEntity<PairUpdateDto>
{
    [Column("discipline")]
    public string Discipline { get; private set; }
    [Column("teacher_id")]
    public Guid TeacherId { get; private set; }
    [Column("cabinet_number")]
    public string CabinetNumber { get; private set; }
    [Column("date")]
    public DateTime Date { get; private set; }
    [Column("group_number")]
    public Guid GroupId { get; private set; }

    private Pair(Guid groupId, DateTime date, string cabinetNumber, Guid teacherId, string discipline)
    {
        GroupId = groupId;
        Date = date;
        CabinetNumber = cabinetNumber;
        TeacherId = teacherId;
        Discipline = discipline;
    }
    public bool IsOnline => CabinetNumber == "СДО";


    public static Pair Create(Guid groupId, DateTime date, string cabinetNumber, Guid teacherId, string discipline)
        => !ValidateTime(date)
            ? throw new Exception("Некорректное время пары")
            : new Pair(groupId, date, cabinetNumber, teacherId, discipline);


    private static bool ValidateTime(DateTime date)
        => !Times.Contains(TimeOnly.FromDateTime(date));

    private static readonly List<TimeOnly> Times =
    [
        new(09, 00),
        new(10, 45),
        new(13, 00),
        new(14, 40),
        new(16, 20),
        new(18, 00)
    ];

    public void NotContainsConflict(User[] teachers, Pair[] existingPairs)
    {
        if (IsOnline)
            return;

        var pairsByDate = existingPairs
            .Where(x => !x.IsOnline)
            .Where(x => x.Date == Date)
            .ToList();

        var teacherConflictPairs = pairsByDate
            .Where(x => x.TeacherId == TeacherId)
            .ToList();

        if ((teacherConflictPairs.Count > 1 && teacherConflictPairs.Select(x => x.CabinetNumber).Distinct().Count() != 1)
            || teacherConflictPairs.Any(x => x.CabinetNumber != CabinetNumber))
        {
            var teacher = teachers.SingleOrDefault(x => x.Id == teacherConflictPairs.Single().TeacherId);
            throw new Exception(
                teacher == null
                    ? "Указан несуществующий преподаватель"
                    : $"У преподавателя {teacher.FullName} уже есть пара на время  {Date.ToStringFormat()}");
        }

        var groupConflictPairs = pairsByDate
            .SingleOrDefault(x => x.GroupId == GroupId);

        if (groupConflictPairs != null)
            throw new Exception(($"У группы {GroupId} уже есть пара на время {Date.ToStringFormat()}"));

        var cabinetConflictPairs = pairsByDate
            .Where(x => x.CabinetNumber == CabinetNumber)
            .ToList();

        if ((cabinetConflictPairs.Count > 1 && cabinetConflictPairs.Select(x => x.TeacherId).Distinct().Count() != 1)
            || cabinetConflictPairs.Any(x => x.TeacherId != TeacherId))
            throw new Exception(($"В кабинете {CabinetNumber} уже есть пара на время {Date.ToStringFormat()}"));
    }

    public override void Update(PairUpdateDto dto)
    {
        Discipline = dto.Discipline ?? Discipline;
        TeacherId = dto.TeacherId ?? TeacherId;
        CabinetNumber = dto.CabinetNumber ?? CabinetNumber;
        Date = dto.Date ?? Date;
        GroupId = dto.GroupId ?? GroupId;
    }
}

public record PairUpdateDto(
    string? Discipline,
    Guid? TeacherId,
    string? CabinetNumber,
    DateTime? Date,
    Guid? GroupId) : UpdateDto;