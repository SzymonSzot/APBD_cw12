using APBD12.Data;

namespace APBD12.Services;

public class DbService : IDbService
{
    private readonly Apbd10Context _context;
    public DbService(Apbd10Context context)
    {
        _context = context;
    }
}