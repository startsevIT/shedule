using System.Security.Cryptography;

namespace Application.Services.Users;

public class PasswordGeneratorService
{
    private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
    private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "0123456789";
    private const string SpecialChars = "!@#$%^&*";
    private const string AllChars = LowerCase + UpperCase + Digits + SpecialChars;
    private readonly RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();

    public string Generate(int length = 12)
    {
        var password = new char[length];

        password[0] = GetRandomChar(LowerCase);
        password[1] = GetRandomChar(UpperCase);
        password[2] = GetRandomChar(Digits);
        password[3] = GetRandomChar(SpecialChars);

        for (var i = 4; i < length; i++)
            password[i] = GetRandomChar(AllChars);

        ShuffleArray(password);

        return new string(password);
    }

    private char GetRandomChar(string charSet)
    {
        var bytes = new byte[4];
        _randomNumberGenerator.GetBytes(bytes);
        var index = BitConverter.ToUInt32(bytes, 0) % (uint)charSet.Length;
        return charSet[(int)index];
    }

    private void ShuffleArray(char[] array)
    {
        for (var i = array.Length - 1; i > 0; i--)
        {
            var bytes = new byte[4];
            _randomNumberGenerator.GetBytes(bytes);
            var j = (int)(BitConverter.ToUInt32(bytes, 0) % (i + 1));

            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}