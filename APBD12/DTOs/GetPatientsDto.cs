namespace APBD12.DTOs;

public static class GetPatientsDto
{
    public class PatientDto
    {
        public string Pesel { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Sex { get; set; } = string.Empty;
        public IEnumerable<AdmissionDto> Admissions { get; set; } = [];
        public IEnumerable<BedAssignmentDto> BedAssignments { get; set; } = [];
    }

    public class AdmissionDto
    {
        public int Id { get; set; }
        public DateTime AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public WardDto Ward { get; set; } = null!;
    }

    public class WardDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class BedAssignmentDto
    {
        public int Id { get; set; }
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
        public BedDto Bed { get; set; } = new BedDto();
    }

    public class BedDto
    {
        public int Id { get; set; }
        public BedTypeDto BedType { get; set; } = null!;
        public RoomDto Room { get; set; } = null!;
    }

    public class BedTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class RoomDto
    {
        public string Id { get; set; } = string.Empty;
        public bool HasTv { get; set; }
        public WardDto Ward { get; set; } = null!;
    }
}