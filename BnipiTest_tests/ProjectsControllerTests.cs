using BnipiTest.Controllers;
using BnipiTest.Extensions;
using BnipiTest.Models;
using BnipiTest.Models.InterfacesLib;
using BnipiTest.ViewModels;
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
        private readonly ProjectsController _controller;
        private readonly ProjectContext _context;

        public ProjectsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ProjectContext>()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BnipiTestDB;Trusted_Connection=True;")
                .Options;

            _context = new ProjectContext(options);
            _controller = new ProjectsController(_context);
        }

        [Fact]
        public async Task GetProjectDetails()
        {
            //arrange
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var concreteId = 1;

            _context.Projects.Add(new Project { Cipher = "P1", Name = "Проект 1" });
            _context.Projects.Add(new Project { Cipher = "P2", Name = "Проект 2" });

            await _context.SaveChangesAsync();

            _context.DesignObjects.Add(new DesignObject { Code = "1.1", Name = "Объект 1.1", ProjectId = concreteId });
            _context.DesignObjects.Add(new DesignObject { Code = "1.2", Name = "Объект 1.2", ProjectId = concreteId });
            var childDesignObject111 = new DesignObject
            {
                Code = "1.1.1",
                Name = "Дочерний объект 1.1.1",
                ParentDesignObjectId = concreteId,
                ProjectId = concreteId
            };
            _context.Add(childDesignObject111);

            await _context.SaveChangesAsync();


            await _context.SaveChangesAsync();

            var markTH = new Mark { ShortName = "ТХ", Name = "Технология производства" };
            var markAC = new Mark { ShortName = "АС", Name = "Архитектурно-строительные решения" };
            var markCM = new Mark { ShortName = "СМ", Name = "Сметная документация" };
            _context.Add(markTH);
            _context.Add(markAC);
            _context.Add(markCM);
            _context.SaveChanges();

            var docSet1 = new Document { MarkId = markAC.Id, Number = 0, DesignObjectId = concreteId};
            var docSet2 = new Document { MarkId = markTH.Id, Number = 1, DesignObjectId = concreteId };
            var docSet3 = new Document { MarkId = markCM.Id, Number = 3, DesignObjectId = childDesignObject111.Id };


            _context.Documents.AddRange(docSet1, docSet2, docSet3);
            _context.SaveChanges();

            //act
            var result = await _controller.GetProjectDetails(concreteId);

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnProjects = Assert.IsType<List<DocumentsViewModel>>(okResult.Value);
            Assert.Equal(3, returnProjects.Count);
        }


        [Fact]
        public async Task GetProjects_ShouldReturnAllProjects()
        {
            //arrange
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var concreteId = 1;

            _context.Projects.Add(new Project { Cipher = "P1", Name = "Проект 1" });
            _context.Projects.Add(new Project { Cipher = "P2", Name = "Проект 2" });

            await _context.SaveChangesAsync();

            _context.DesignObjects.Add(new DesignObject { Code = "1.1", Name = "Объект 1.1", ProjectId = concreteId });
            _context.DesignObjects.Add(new DesignObject { Code = "1.2", Name = "Объект 1.2", ProjectId = concreteId });

            await _context.SaveChangesAsync();

            var childDesignObject111 = new DesignObject
            {
                Code = "1.1.1",
                Name = "Дочерний объект 1.1.1",
                ParentDesignObjectId = concreteId,
                ProjectId = concreteId
            };

            await _context.SaveChangesAsync();

            //act
            var result = await _controller.GetProjects();

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnProjects = Assert.IsType<List<Project>>(okResult.Value);
            Assert.Equal(2, returnProjects.Count);
        }

        [Fact]
        public async Task GetProjectById_ShouldReturnconcreteProject()
        {
            //arrange
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            var concreteId = 1;

            _context.Projects.Add(new Project { Cipher = "P1", Name = "Проект 1" });
            _context.Projects.Add(new Project { Cipher = "P2", Name = "Проект 2" });

            await _context.SaveChangesAsync();

            _context.DesignObjects.Add(new DesignObject { Code = "1.1", Name = "Объект 1.1", ProjectId = concreteId });
            _context.DesignObjects.Add(new DesignObject { Code = "1.2", Name = "Объект 1.2", ProjectId = concreteId });

            await _context.SaveChangesAsync();

            var childDesignObject111 = new DesignObject
            {
                Code = "1.1.1",
                Name = "Дочерний объект 1.1.1",
                ParentDesignObjectId = concreteId,
                ProjectId = concreteId
            };

            await _context.SaveChangesAsync();

            var concreteProj = await _context.Projects.Include(e => e.DesignObjects)
                                                      .FirstOrDefaultAsync(e => e.Id == concreteId);

            //act
            var result = await _controller.GetProjectById(concreteId);

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnObj = Assert.IsType<Project>(okResult.Value);
            Assert.Equal(concreteProj, returnObj);
        }
        
        [Fact]
        public async Task CreateProject_shouldAddProjectAndReturnCreatedResult()
        {
            // Arrange
            _context.Database.EnsureCreated();
            var newProject = new Project { Cipher = "P3", Name = "Проект 3" };

            // Act
            var result = await _controller.CreateProject(newProject);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.GetProjectById), createdAtActionResult.ActionName);
            Assert.Equal(newProject.Id, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(newProject, createdAtActionResult.Value);
            Assert.Equal(3, _context.Projects.Count());
        }
        [Fact]
        public async Task UpdateProject_ShouldUpdateExistingProject()
        {
            // Arrange
            _context.Database.EnsureCreated();
            var existingProject = new Project { Cipher = "P1-Updated", Name = "Проект 1 (обновлен)" };
            _context.Projects.Add(existingProject);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.UpdateProject(existingProject.Id, existingProject);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedProject = await _context.Projects.FindAsync(existingProject.Id);
            Assert.Equal("P1-Updated", updatedProject.Cipher);
            Assert.Equal("Проект 1 (обновлен)", updatedProject.Name);
        }
        [Fact]
        public async Task DeleteProject_ShouldRemoveProjectAndReturnNoContent()
        {
            // Arrange
            _context.Database.EnsureCreated();
            var projectToDelete = new Project {Cipher = "P1", Name = "Проект 1" };
            _context.Projects.Add(projectToDelete);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteProject(projectToDelete.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.Projects.FindAsync(projectToDelete.Id));
        }

    }
}
