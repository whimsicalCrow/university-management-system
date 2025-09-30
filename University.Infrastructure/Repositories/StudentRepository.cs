using Microsoft.EntityFrameworkCore;
using University.Domain.Entities;
using University.Infrastructure.Persistence;

namespace University.Infrastructure.Repositories;

public class StudentRepository
{
    private readonly UniversityDbContext _db;
    public StudentRepository(UniversityDbContext db) => _db = db;

    public async Task<Student> AddAsync(Student s, CancellationToken ct = default)
    {
        _db.Students.Add(s);
        await _db.SaveChangesAsync(ct);
        return s;
    }

    public Task<Student?> GetAsync(Guid id, CancellationToken ct = default) =>
        _db.Students.FirstOrDefaultAsync(x => x.Id == id, ct);
}
