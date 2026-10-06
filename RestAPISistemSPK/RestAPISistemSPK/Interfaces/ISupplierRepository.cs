using RestAPISistemSPK.Models.DTOs;
using RestAPISistemSPK.Models.Entities;

namespace RestAPISistemSPK.Interfaces
{
    public interface ISupplierRepository
    {
        Task<List<SupplierDto>> GetAllAsync();
        Task<SupplierDto?> GetByIdAsync(Guid id);
        Task<Supplier> CreateAsync(CreateSupplierDto dto);
        Task<Supplier?> UpdateAsync(UpdateSupplierDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<int> CountAsync();
    }
}
