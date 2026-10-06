using RestAPISistemSPK.Models.DTOs;
using RestAPISistemSPK.Models.Entities;

namespace RestAPISistemSPK.Interfaces
{
    public interface IKeputusanRepository
    {
        Task<CalculateKeputusanResponseDto> CalculateAsync(CalculateKeputusanRequestDto request);
        Task<Keputusan> SaveAsync(SaveKeputusanRequestDto dto);
        Task<List<KeputusanListDto>> GetAllAsync(string? status = null);
        Task<KeputusanDetailResponseDto?> GetByIdAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();
    }
}
