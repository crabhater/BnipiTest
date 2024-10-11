using BnipiTest.Models;
using BnipiTest.Models.InterfacesLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace BnipiTest.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectContext _context;

        public ProjectsController(ProjectContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> CreateDataDB()
        {

            _context.Projects.RemoveRange(_context.Projects);
            _context.DesignObjects.RemoveRange(_context.DesignObjects);
            _context.DocumentationSets.RemoveRange(_context.DocumentationSets);
            _context.SaveChanges();

            // Создание проектов
            var project1 = new Project { Code = "P1", Name = "Проект 1" };
            var project2 = new Project { Code = "P2", Name = "Проект 2" };

            _context.Projects.AddRange(project1, project2);
            _context.SaveChanges();

            // Создание объектов проектирования
            var designObject11 = new DesignObject { Code = "1.1", Name = "Объект 1.1", ProjectId = project1.Id };
            var designObject12 = new DesignObject { Code = "1.2", Name = "Объект 1.2", ProjectId = project1.Id };
            var designObject21 = new DesignObject { Code = "2.1", Name = "Объект 2.1", ProjectId = project2.Id };

            _context.DesignObjects.AddRange(designObject11, designObject12, designObject21);
            _context.SaveChanges();

            // Создание дочерних объектов
            var childDesignObject111 = new DesignObject
            {
                Code = "1.1.1",
                Name = "Дочерний объект 1.1.1",
                ParentDesignObjectId = designObject11.Id,
                ProjectId = project1.Id
            };

            _context.DesignObjects.Add(childDesignObject111);
            _context.SaveChanges();

            // Создание комплектов документации
            var docSet1 = new Document { Mark = "A", Number = 0, DesignObjectId = designObject11.Id };
            var docSet2 = new Document { Mark = "B", Number = 1, DesignObjectId = designObject11.Id };

            _context.DocumentationSets.AddRange(docSet1, docSet2);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.Include(e=>e.DesignObjects)
                                          .ThenInclude(e=>e.Documents)
                                          .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProjectById(int id)
        {
            var project = await _context.Projects.Include(e=> e.DesignObjects).ThenInclude(e=>e.Documents).FirstOrDefaultAsync(e=> e.Id == id);
            if (project == null)
            {
                var errorMsg = new ErrorMessage("Проект не найден!");
                return NotFound(errorMsg);
            }

            return Ok(project);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<DesignObject>> GetDesignObject(int id)
        {
            var designObject = await _context.DesignObjects.Include(e => e.Documents).FirstOrDefaultAsync(e => e.Id == id);
            if (designObject == null)
            {
                var errorMsg = new ErrorMessage("Объект проектирования не найден!");
                return NotFound(errorMsg);
            }

            return Ok(designObject);
        }

        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, Project project)
        {
            if (id != project.Id)
            {
                var errorMsg = new ErrorMessage("Неверный Id");
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
