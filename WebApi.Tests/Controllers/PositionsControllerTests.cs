using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using WebApi.DTOs;
using WebApi.Services.Commands;
using WebApi.Services.Queries;

namespace WebApi.Tests.Controllers
{
    public class PositionsControllerTests
    {
        private readonly Mock<IPositionCommandService> _mockCommand;
        private readonly Mock<IPositionQueryService> _mockQuery;
        private readonly PositionsController _controller;

        public PositionsControllerTests()
        {
            _mockCommand = new Mock<IPositionCommandService>();
            _mockQuery = new Mock<IPositionQueryService>();
            _controller = new PositionsController(_mockQuery.Object,_mockCommand.Object);

            // este es necesario para que FluentValidation valide en el controlador
            _controller.ModelState.Clear();
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenPositionExists()
        {
            var position = new PositionDTO { Id = 1, Title = "Dev", Description = "Desc", Location = "CR", Status = "open", RecruiterId = 1, DepartmentId = 1, Budget = 1000 };
            _mockQuery.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(position);
            var result = await _controller.GetById(1);
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(position);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenPositionDoesNotExist()
        {
            _mockQuery.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((PositionDTO?)null);
            var result = await _controller.GetById(99);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenSuccessful()
        {
            var dto = new PositionDTO
            {
                Title = "New Position",
                Description = "Desc",
                Location = "CR",
                Status = "open",
                RecruiterId = 1,
                DepartmentId = 1,
                Budget = 1000
            };
            var created = new PositionDTO
            {
                Id = 1,
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                Status = dto.Status,
                RecruiterId = dto.RecruiterId,
                DepartmentId = dto.DepartmentId,
                Budget = dto.Budget,
                ClosingDate = dto.ClosingDate
            };
            _mockCommand.Setup(x => x.CreateAsync(dto)).ReturnsAsync(created);
            var result = await _controller.Create(dto);
            var createdResult = result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult!.StatusCode.Should().Be(201);
            createdResult.Value.Should().BeEquivalentTo(created);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenUpdateSuccessful()
        {
            var dto = new PositionDTO { Title = "Updated", Description = "Desc", Location = "CR", Status = "open", RecruiterId = 1, DepartmentId = 1, Budget = 1000 };
            _mockCommand.Setup(x => x.UpdateAsync(1, dto)).ReturnsAsync(true);
            var result = await _controller.Update(1, dto);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenPositionNotFound()
        {
            var dto = new PositionDTO { Title = "Updated", Description = "Desc", Location = "CR", Status = "open", RecruiterId = 1, DepartmentId = 1, Budget = 1000 };
            _mockCommand.Setup(x => x.UpdateAsync(1, dto)).ReturnsAsync(false);
            var result = await _controller.Update(1, dto);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleteSuccessful()
        {
            _mockCommand.Setup(x => x.DeleteAsync(1)).ReturnsAsync(true);
            var result = await _controller.Delete(1);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenPositionNotFound()
        {
            _mockCommand.Setup(x => x.DeleteAsync(1)).ReturnsAsync(false);
            var result = await _controller.Delete(1);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenModelIsInvalid()
        {
            var invalidDto = new PositionDTO
            {
                Title = "",
                Description = "Desc",
                Location = "CR",
                Status = "open",
                RecruiterId = 1,
                DepartmentId = 1,
                Budget = 1000
            };
            _controller.ModelState.AddModelError("Title", "El título es obligatorio");
            var result = await _controller.Create(invalidDto);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetAll_ThrowsException_ReturnsInternalServerError()
        {
            _mockQuery.Setup(x => x.GetAllAsync()).ThrowsAsync(new Exception("Error inesperado"));
            Func<Task> act = async () => await _controller.GetAll();
            // este assert debe   verificar que al llamar al controlador se lance la excepción que es atrapada por el middleware global
            await act.Should().ThrowAsync<Exception>().WithMessage("Error inesperado");
        }
    }
}
