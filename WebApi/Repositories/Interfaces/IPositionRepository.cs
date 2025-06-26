using WebApi.DTOs.Filters;
using WebApi.Models;

namespace WebApi.Repositories.Interfaces
{
    public interface IPositionRepository
    {
        Task<IEnumerable<Position>> GetAllAsync();
        Task<Position?> GetByIdAsync(int id);
        Task<Position> CreateAsync(Position position);
        Task<bool> UpdateAsync(Position position);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Position>> GetFilteredAsync(PositionFilter filter);
    }
}
