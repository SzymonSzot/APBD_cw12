using APBD12.Data;
using APBD12.DTOs;
using APBD12.Exceptions;
using APBD12.Models;
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

    public async Task AssignBed(string pesel, BedAssignDto request)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try {
             var patientExists = await _context.Patients.AnyAsync(p => p.Pesel == pesel);
                if (!patientExists)
                {
                    throw new NotFoundException($"Nie znaleziono pacjenta o numerze PESEL: {pesel}");
                }
                
                var wardExists = await _context.Wards.AnyAsync(w => w.Name == request.Ward);
                if (!wardExists)
                {
                    throw new NotFoundException($"Nie znaleziono w systemie oddziału o nazwie: '{request.Ward}'");
                }
                
                var bedTypeExists = await _context.BedTypes.AnyAsync(bt => bt.Name == request.BedType);
                if (!bedTypeExists)
                {
                    throw new NotFoundException($"Nie znaleziono w systemie typu łóżka o nazwie: '{request.BedType}'");
                }
            
                var availableBed = await _context.Beds
                    .Where(b => b.BedType.Name == request.BedType && b.Room.Ward.Name == request.Ward)
                    .Where(b => !_context.BedAssignments.Any(ba => 
                        ba.BedId == b.Id && (
                            (ba.To != null && request.From < ba.To && (!request.To.HasValue || request.To.Value > ba.From)) 
                            ||
                            (ba.To == null && (!request.To.HasValue || request.To.Value > ba.From))
                        )
                    ))
                    .FirstOrDefaultAsync();
                
                if (availableBed == null)
                {
                    throw new NotFoundException($"Brak wolnego łóżka typu '{request.BedType}' na oddziale '{request.Ward}' w wybranym przedziale czasowym ({request.From} - {(request.To.HasValue ? request.To.Value.ToString() : "brak daty końcowej")}).");
                }
                
                var newAssignment = new BedAssignment
                {
                    PatientPesel = pesel,
                    BedId = availableBed.Id,
                    From = request.From,
                    To = request.To
                };

                await _context.BedAssignments.AddAsync(newAssignment);
                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();
        }
        catch (NotFoundException ex)
        {
            await transaction.RollbackAsync();
            throw new NotFoundException(ex.Message);
        }
    }
}