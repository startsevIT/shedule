namespace Domain.Pairs;

public record PairReadDto(
    string Discipline,
    string Teacher,
    string Cabinet,
    string Group,
    DateTime Date,
    string Color);