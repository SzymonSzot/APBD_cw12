namespace APBD12.DTOs;

public class BedAssignDto
{
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
    public string BedType { get; set; } = string.Empty;
    public string Ward { get; set; } = string.Empty;
}