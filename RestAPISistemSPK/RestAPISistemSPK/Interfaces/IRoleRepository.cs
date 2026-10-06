using RestAPISistemSPK.Models.Entities;

namespace RestAPISistemSPK.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int id);
        Task AddAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(int id);
        Task<bool> HasUsersAsync(int roleId);
        Task<int> CountAsync();
        Task SaveChangesAsync();
    }
}
