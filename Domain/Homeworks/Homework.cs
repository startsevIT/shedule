using Domain.Entity;

namespace Domain.Homeworks;

public class Homework : ModifyEntity<HomeworkUpdateDto>
{
    private Guid TeacherId { get; set; }
    private Guid StudentId { get; set; }
    private string Title { get; set; } = null!;
    private string Description { get; set; } = null!;
    private DateTime StartDate { get; set; }
    private DateTime EndDate { get; set; }
    private HomeworkStatus Status { get; set; }

    public static Homework Create(Guid teacherId,
        Guid studentId,
        string title,
        string description,
        DateTime startDate,
        DateTime endDate)
        => new()
        {
            TeacherId = teacherId,
            StudentId = studentId,
            Title = title,
            Description = description,
            StartDate = startDate,
            EndDate = endDate,
            Status = HomeworkStatus.InProgress
        };

    public static Homework Create(Guid teacherId,
        Guid studentId,
        string title,
        string description,
        DateTime startDate,
        int durationInDay)
        => new()
        {
            TeacherId = teacherId,
            StudentId = studentId,
            Title = title,
            Description = description,
            StartDate = startDate,
            EndDate = startDate.AddDays(durationInDay),
            Status = HomeworkStatus.InProgress
        };

    public override void Update(HomeworkUpdateDto dto)
    {
        TeacherId = dto.TeacherId ?? TeacherId;
        Title = dto.Title ?? Title;
        Description = dto.Description ?? Description;
        StartDate = dto.StartDate ?? StartDate;
        EndDate = dto.EndDate ?? EndDate;
        Status = dto.Status ?? Status;
    }
}

//todo концепция, есть сущность дз, а есть сущность активного дз, разделить, в будуще можно будет шаблоны делать

public record HomeworkUpdateDto(
    Guid? TeacherId,
    string? Title,
    string? Description,
    DateTime? StartDate,
    DateTime? EndDate,
    HomeworkStatus? Status) : UpdateDto;