using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.RegularExpressions;
using Domain.Entity;

namespace Domain.Groups;

[Table("groups")]
public class Group : ModifyEntity<GroupUpdateDto>
{
    private const string CorrespondenceString = "correspondence";
    private const string DirectionString = "direction";
    private const string AcceleratedString = "accelerated";
    private const string MainNumberString = "main_number";
    private const string SecondNumberString = "main_number";

    private static readonly Regex GroupNameRegex =
        new(@$"^(?'{CorrespondenceString}'еоз|)(?'{DirectionString}'[А-Я]*)(?'{AcceleratedString}'у|)-(?'{MainNumberString}'\d*)(?:\((?'{SecondNumberString}'\d*)\)|)$$");

    public string Name
    {
        get
        {
            var result = new StringBuilder();
            if(IsCorrespondence)
                result.Append("еоз");
            result.Append(DirectionShortName);
            if (IsAccelerated)
                result.Append('у');

            result.Append('-');

            result.Append(Numbers[0]);
            if(IsCombined)
                result.Append($"({Numbers[1]})");
            return result.ToString();
        }
    }

    [Column("direction_short_name")]
    public string DirectionShortName { get; set; } = null!;

    [Column("numbers")]
    public short[] Numbers { get; set; } = null!;

    [Column("is_correspondence")]
    public bool IsCorrespondence { get; set; }

    [Column("is_accelerated")]
    public bool IsAccelerated { get; set; }

    [Column("is_combined")]
    public bool IsCombined { get; set; }

    public static Group Create(string name)
    {
        var result = new Group();
        result.Fill(name);
        return result;
    }

    private void Fill(string name)
    {
        var nameMathResult = GroupNameRegex.Match(name);
        if (!nameMathResult.Success)
            throw new Exception("Некорректный номер группы");
        IsCorrespondence = !string.IsNullOrEmpty(nameMathResult.Groups[CorrespondenceString].Value);
        DirectionShortName = nameMathResult.Groups[DirectionString].Value;
        IsAccelerated = !string.IsNullOrEmpty(nameMathResult.Groups[AcceleratedString].Value);

        var mainNumber = short.Parse(nameMathResult.Groups[MainNumberString].Value);
        var secondNumber = nameMathResult.Groups.TryGetValue(SecondNumberString, out var value)
            ? short.Parse(value.Value)
            : byte.MaxValue;
        IsCombined = secondNumber != 0;
        Numbers = IsCombined
            ? [mainNumber, secondNumber]
            : [mainNumber];
    }

    public override void Update(GroupUpdateDto dto)
        => Fill(dto.Name);
}

public record GroupUpdateDto(string Name) : UpdateDto;