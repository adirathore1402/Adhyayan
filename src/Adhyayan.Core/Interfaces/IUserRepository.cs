using Adhyayan.Core.Models;

namespace Adhyayan.Core.Interfaces;

public interface IUserRepository
{
    // Parents
    Task<Parent?> GetParentByEmailAsync(string email);
    Task<Parent?> GetParentByIdAsync(int parentId);
    Task<Parent> CreateParentAsync(Parent parent);

    // Children
    Task<IEnumerable<Child>> GetChildrenByParentAsync(int parentId);
    Task<Child?> GetChildByIdAsync(int childId);
    Task<Child> CreateChildAsync(Child child);
    Task UpdateChildAsync(Child child);
}
