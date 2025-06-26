using WebApi.DTOs;
using WebApi.Models;

namespace WebApi.Services.Commands
{
    public interface IPositionCommandService
    {
        Task<PositionDTO> CreateAsync(PositionDTO dto);
        Task<bool> UpdateAsync(int id, PositionDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
