using APBD12.Data;
using APBD12.DTOs;
using APBD12.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace APBD12.Services;

public class DbService : IDbService
{
    private readonly Apbd10Context _context;
    public DbService(Apbd10Context context)
    {
        _context = context;
    }

    public async Task<List<GetPatientsDto.PatientDto>> GetPatients(string? search)
    {
        var query = _context.Patients.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.FirstName.Contains(search) || p.LastName.Contains(search));
        }
        var res = await query
            .Select(p => new GetPatientsDto.PatientDto
            {
                Pesel = p.Pesel,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Age = p.Age,
                Sex = p.Sex ? "Male" : "Female",
                Admissions = p.Admissions.Select(a => new GetPatientsDto.AdmissionDto
                {
                    Id = a.Id,
                    AdmissionDate = a.AdmissionDate,
                    DischargeDate = a.DischargeDate,
                    Ward = new GetPatientsDto.WardDto
                    {
                        Id = a.Ward.Id,
                        Name = a.Ward.Name,
                        Description = a.Ward.Description
                    }
                }).ToList(),
                BedAssignments = p.BedAssignments.Select(ba => new GetPatientsDto.BedAssignmentDto
                {
                    Id = ba.Id,
                    From = ba.From,
                    To = ba.To,
                    Bed = new GetPatientsDto.BedDto
                    {
                        Id = ba.Bed.Id,
                        BedType = new GetPatientsDto.BedTypeDto
                        {
                            Id = ba.Bed.BedType.Id,
                            Name = ba.Bed.BedType.Name,
                            Description = ba.Bed.BedType.Description
                        },
                        Room = new GetPatientsDto.RoomDto
                        {
                            Id = ba.Bed.Room.Id,
                            HasTv = ba.Bed.Room.HasTv,
                            Ward = new GetPatientsDto.WardDto
                            {
                                Id = ba.Bed.Room.Ward.Id,
                                Name = ba.Bed.Room.Ward.Name,
                                Description = ba.Bed.Room.Ward.Description
                            }
                        }
                    }
                }).ToList()
            }).ToListAsync();

        if (res == null)
        {
            throw new NotFoundException();
        }
        return res;
    }
}