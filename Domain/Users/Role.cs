namespace Domain.Users;

public static class Role
{
    public const string Student = "student";
    public const string Entrant = "entrant";
    public const string Teacher = "teacher";
    public const string Manager = "manager";
    public const string Admin = "admin";

    public const string Workers = Teacher + "," + Manager + "," + Admin;
}