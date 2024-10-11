using BnipiTest.Controllers;
using BnipiTest.Extensions;
using BnipiTest.Models;
using BnipiTest.Models.InterfacesLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnipiTest_tests
{
    public class ProjectsControllerTests
    {
        private readonly Mock<ProjectContext> _mockContext;
        private readonly ProjectsController _controller;

        public ProjectsControllerTests()
        {
        }

        [Fact]
        public async Task GetProjects_ShouldReturnAllProjects()
        {
            // Arrange
           

            // Act
            var result = await _controller.GetProjects();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnProjects = Assert.IsType<List<Project>>(okResult.Value);
            Assert.Equal(2, returnProjects.Count);
        }
    }
}
