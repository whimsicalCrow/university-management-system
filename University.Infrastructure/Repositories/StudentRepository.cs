using System.Linq;
using Microsoft.EntityFrameworkCore;
using University.Domain.Entities;
using University.Domain.Interfaces;
using University.Infrastructure.Persistence;

namespace University.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly UniversityDbContext _db;
    public StudentRepository(UniversityDbContext db) => _db = db;

    public async Task<Student> AddAsync(Student student, CancellationToken ct = default)
    {
        _db.Students.Add(student);
        await _db.SaveChangesAsync(ct);
        return student;
    }

    public Task<Student?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Students.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Student>> GetBySupervisorAsync(Guid professorId, CancellationToken cancellationToken = default)
    {
        return await _db.Students
            .Where(s => s.SupervisorId == professorId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Student student, CancellationToken cancellationToken = default)
    {
        _db.Students.Update(student);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
