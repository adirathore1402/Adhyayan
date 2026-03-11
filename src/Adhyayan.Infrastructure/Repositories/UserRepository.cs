using Adhyayan.Core.Interfaces;
using Adhyayan.Core.Models;
using Adhyayan.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Adhyayan.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AdhyayanDbContext _db;

    public UserRepository(AdhyayanDbContext db) => _db = db;

    public async Task<Parent?> GetParentByEmailAsync(string email)
        => await _db.Parents.FirstOrDefaultAsync(p => p.Email == email);

    public async Task<Parent?> GetParentByIdAsync(int parentId)
        => await _db.Parents.Include(p => p.Children).FirstOrDefaultAsync(p => p.Id == parentId);

    public async Task<Parent> CreateParentAsync(Parent parent)
    {
        _db.Parents.Add(parent);
        await _db.SaveChangesAsync();
        return parent;
    }

    public async Task<IEnumerable<Child>> GetChildrenByParentAsync(int parentId)
        => await _db.Children.Include(c => c.Board).Where(c => c.ParentId == parentId).ToListAsync();

    public async Task<Child?> GetChildByIdAsync(int childId)
        => await _db.Children.Include(c => c.Board).FirstOrDefaultAsync(c => c.Id == childId);

    public async Task<Child> CreateChildAsync(Child child)
    {
        _db.Children.Add(child);
        await _db.SaveChangesAsync();
        return child;
    }

    public async Task UpdateChildAsync(Child child)
    {
        _db.Children.Update(child);
        await _db.SaveChangesAsync();
    }
}
