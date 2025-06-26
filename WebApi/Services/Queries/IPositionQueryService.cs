using WebApi.DTOs;
using WebApi.DTOs.Filters;
using WebApi.Models;

namespace WebApi.Services.Queries
{
    public interface IPositionQueryService
    {
        Task<IEnumerable<PositionDTO>> GetAllAsync();
        Task<PositionDTO?> GetByIdAsync(int id);
        Task<IEnumerable<PositionDTO>> GetFilteredAsync(PositionFilter filter);
    }
}
