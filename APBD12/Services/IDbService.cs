using APBD12.DTOs;
using APBD12.Models;

namespace APBD12.Services;

public interface IDbService
{
    Task<List<GetPatientsDto.PatientDto>> GetPatients(string? search);
}