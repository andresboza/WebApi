using WebApi.DTOs;
using WebApi.DTOs.Filters;
using WebApi.Models;
using WebApi.Repositories;
using WebApi.Repositories.Interfaces;

namespace WebApi.Services.Queries
{
    public class PositionQueryService : IPositionQueryService
    {
        private readonly IPositionRepository _repository;
        public PositionQueryService(IPositionRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<PositionDTO>> GetAllAsync()
        {
            var positions = await _repository.GetAllAsync();
            return positions.Select(p => MapToDTO(p));
        }
        public async Task<PositionDTO?> GetByIdAsync(int id)
        {
            var p = await _repository.GetByIdAsync(id);
            return p == null ? null : MapToDTO(p);
        }
        public async Task<IEnumerable<PositionDTO>> GetFilteredAsync(PositionFilter filter)
        {
            var positions = await _repository.GetFilteredAsync(filter);
            return positions.Select(p => new PositionDTO
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Location = p.Location,
                Status = p.Status,
                RecruiterId = p.RecruiterId,
                DepartmentId = p.DepartmentId,
                Budget = p.Budget,
                ClosingDate = p.ClosingDate
            });
        }
        private static PositionDTO MapToDTO(Position p)
        {
            return new PositionDTO
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Location = p.Location,
                Status = p.Status,
                RecruiterId = p.RecruiterId,
                DepartmentId = p.DepartmentId,
                Budget = p.Budget,
                ClosingDate = p.ClosingDate
            };
        }
    }
}
