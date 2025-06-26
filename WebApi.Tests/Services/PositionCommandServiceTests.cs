using Moq;
using WebApi.DTOs;
using WebApi.Models;
using FluentAssertions;
using WebApi.Services.Commands;
using WebApi.Repositories.Interfaces;

namespace WebApi.Tests.Services
{
    public class PositionCommandServiceTests
    {
        private readonly Mock<IPositionRepository> _mockRepo;
        private readonly PositionCommandService _service;

        public PositionCommandServiceTests()
        {
            _mockRepo = new Mock<IPositionRepository>();
            _service = new PositionCommandService(_mockRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedPosition()
        {
            var dto = new PositionDTO
            {
                Title = "Backend Developer",
                Description = "Create APIs",
                Location = "Remote",
                Status = "open",
                RecruiterId = 1,
                DepartmentId = 1,
                Budget = 10000
            };
            var created = new Position { Id = 1, Title = dto.Title };
            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Position>())).ReturnsAsync(created);
            var result = await _service.CreateAsync(dto);
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Title.Should().Be("Backend Developer");
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnTrue_WhenPositionIsUpdated()
        {
            var dto = new PositionDTO
            {
                Title = "Updated Title",
                Description = "Updated Description",
                Location = "Remote",
                Status = "open",
                RecruiterId = 1,
                DepartmentId = 2,
                Budget = 12000
            };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Position { Id = 1 });
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Position>())).ReturnsAsync(true);
            var result = await _service.UpdateAsync(1, dto);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenPositionDoesNotExist()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Position?)null);
            var result = await _service.UpdateAsync(99, new PositionDTO());
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenPositionIsDeleted()
        {
            _mockRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);
            var result = await _service.DeleteAsync(1);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenPositionNotFound()
        {
            _mockRepo.Setup(r => r.DeleteAsync(99)).ReturnsAsync(false);
            var result = await _service.DeleteAsync(99);
            result.Should().BeFalse();
        }
    }
}
