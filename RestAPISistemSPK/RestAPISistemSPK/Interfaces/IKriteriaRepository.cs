using RestAPISistemSPK.Models.DTOs;
using RestAPISistemSPK.Models.Entities;

namespace RestAPISistemSPK.Interfaces
{
    public interface IKriteriaRepository
    {
        // ASPEK
        Task<List<AspekDto>> GetAllAspekAsync();
        Task<AspekDto?> GetAspekByIdAsync(Guid id);
        Task<Aspek> CreateAspekAsync(CreateAspekDto dto);
        Task<Aspek?> UpdateAspekAsync(UpdateAspekDto dto);
        Task<bool> DeleteAspekAsync(Guid id);
        Task<int> CountAspekAsync();

        // PARAMETER
        Task<List<ParameterDto>> GetAllParameterAsync(Guid? aspekId = null);
        Task<ParameterDto?> GetParameterByIdAsync(Guid id);
        Task<Parameter> CreateParameterAsync(CreateParameterDto dto);
        Task<Parameter?> UpdateParameterAsync(UpdateParameterDto dto);
        Task<bool> UpdateBatchBobotAsync(List<ParameterBobotItemDto> items);
        Task<bool> DeleteParameterAsync(Guid id);
        Task<int> CountParameterAsync();
    }
}
