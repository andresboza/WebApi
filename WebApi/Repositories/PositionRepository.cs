using WebApi.DTOs.Filters;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private static readonly List<Position> _positions = new();
        public Task<IEnumerable<Position>> GetAllAsync() => Task.FromResult<IEnumerable<Position>>(_positions);
        public Task<Position?> GetByIdAsync(int id) => Task.FromResult(_positions.FirstOrDefault(p => p.Id == id));
        public Task<Position> CreateAsync(Position position)
        {
            position.Id = _positions.Count + 1;
            _positions.Add(position);
            return Task.FromResult(position);
        }
        public Task<bool> UpdateAsync(Position position)
        {
            var existing = _positions.FirstOrDefault(p => p.Id == position.Id);
            if (existing == null) return Task.FromResult(false);

            existing.Title = position.Title;
            existing.Description = position.Description;
            existing.Location = position.Location;
            existing.Status = position.Status;
            existing.RecruiterId = position.RecruiterId;
            existing.DepartmentId = position.DepartmentId;
            existing.Budget = position.Budget;
            existing.ClosingDate = position.ClosingDate;

            return Task.FromResult(true);
        }
        public Task<bool> DeleteAsync(int id)
        {
            var existing = _positions.FirstOrDefault(p => p.Id == id);

            if (existing == null) return Task.FromResult(false);

            _positions.Remove(existing);
            return Task.FromResult(true);
        }
        public Task<IEnumerable<Position>> GetFilteredAsync(PositionFilter filter)
        {
            var query = _positions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Status))
                query = query.Where(p => p.Status.Equals(filter.Status, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(filter.Location))
                query = query.Where(p => p.Location.Contains(filter.Location, StringComparison.OrdinalIgnoreCase));

            int skip = (filter.Page - 1) * filter.PageSize;

            var result = query
                .Skip(skip)
                .Take(filter.PageSize)
                .ToList();

            return Task.FromResult<IEnumerable<Position>>(result);
        }
    }
}
