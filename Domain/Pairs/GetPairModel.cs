namespace Domain.Pairs;

public record GetPairModel(
    DateTime Start,
    DateTime End,
    Guid? UserId = null);