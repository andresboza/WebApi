using WebApi.DTOs;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Services.Commands
{
    public class PositionCommandService : IPositionCommandService
    {
        private readonly IPositionRepository _repository;

        public PositionCommandService(IPositionRepository repository)
        {
            _repository = repository;
        }
        public async Task<PositionDTO> CreateAsync(PositionDTO dto)
        {
            var position = new Position
            {
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                Status = dto.Status,
                RecruiterId = dto.RecruiterId,
                DepartmentId = dto.DepartmentId,
                Budget = dto.Budget,
                ClosingDate = dto.ClosingDate
            };
            var created = await _repository.CreateAsync(position);
            return MapToDTO(created);
        }
        public async Task<bool> UpdateAsync(int id, PositionDTO dto)
        {
            var position = await _repository.GetByIdAsync(id);
            if (position == null) return false;
            position.Title = dto.Title;
            position.Description = dto.Description;
            position.Location = dto.Location;
            position.Status = dto.Status;
            position.RecruiterId = dto.RecruiterId;
            position.DepartmentId = dto.DepartmentId;
            position.Budget = dto.Budget;
            position.ClosingDate = dto.ClosingDate;
            return await _repository.UpdateAsync(position);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
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
