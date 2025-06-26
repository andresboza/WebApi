using Moq;
using WebApi.Models;
using WebApi.Repositories.Interfaces;
using WebApi.Services.Queries;
using WebApi.DTOs.Filters;
using FluentAssertions;

namespace WebApi.Tests.Services
{
    public class PositionQueryServiceTests
    {
        private readonly Mock<IPositionRepository> _mockRepo;
        private readonly PositionQueryService _service;

        public PositionQueryServiceTests()
        {
            _mockRepo = new Mock<IPositionRepository>();
            _service = new PositionQueryService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnPosition_WhenFound()
        {
            var position = new Position { Id = 1, Title = "Sample" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(position);
            var result = await _service.GetByIdAsync(1);
            result.Should().NotBeNull();
            result?.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Position?)null);
            var result = await _service.GetByIdAsync(99);
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetFilteredAsync_ShouldReturnFilteredPositions()
        {
            var list = new List<Position>
            {
                new Position { Id = 1, Title = "Dev", Status = "open", Location = "Remote" },
                new Position { Id = 2, Title = "QA", Status = "open", Location = "Onsite" }
            };
            _mockRepo.Setup(r => r.GetFilteredAsync(It.IsAny<PositionFilter>())).ReturnsAsync(list);
            var result = await _service.GetFilteredAsync(new PositionFilter());
            result.Should().HaveCount(2);
        }
    }
}
