using BnipiTest.Models;
using BnipiTest.Models.InterfacesLib;
using BnipiTest.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _context.SaveChanges();

            var project1 = new Project { Cipher = "P1", Name = "Проект 1" };
            var project2 = new Project { Cipher = "P2", Name = "Проект 2" };

            _context.Projects.AddRange(project1, project2);
            _context.SaveChanges();

            var designObject11 = new DesignObject { Code = "1.1", Name = "Объект 1.1", ProjectId = project1.Id };
            var designObject12 = new DesignObject { Code = "1.2", Name = "Объект 1.2", ProjectId = project1.Id };
            var designObject21 = new DesignObject { Code = "2.1", Name = "Объект 2.1", ProjectId = project2.Id };

            _context.DesignObjects.AddRange(designObject11, designObject12, designObject21);
            _context.SaveChanges();

            var childDesignObject111 = new DesignObject
            {
                Code = "1.1.1",
                Name = "Дочерний объект 1.1.1",
                ParentDesignObjectId = designObject11.Id,
                ProjectId = designObject11.ProjectId
            };

            _context.DesignObjects.Add(childDesignObject111);
            _context.SaveChanges();

            var markTH = new Mark { ShortName = "ТХ", Name = "Технология производства" };
            var markAC = new Mark { ShortName = "АС", Name = "Архитектурно-строительные решения" };
            var markCM = new Mark { ShortName = "СМ", Name = "Сметная документация" };
            _context.Add(markTH);
            _context.Add(markAC);
            _context.Add(markCM);
            _context.SaveChanges();

            var docSet1 = new Document { MarkId = markAC.Id, Number = 0, DesignObjectId = designObject11.Id };
            var docSet2 = new Document { MarkId = markTH.Id, Number = 1, DesignObjectId = designObject11.Id };
            var docSet3 = new Document { MarkId = markCM.Id, Number = 3, DesignObjectId = childDesignObject111.Id };


            _context.Documents.AddRange(docSet1, docSet2, docSet3);
            _context.SaveChanges();

            return Ok();
        }


        [HttpGet("project/{projectId}/details")]
        public async Task<ActionResult<IEnumerable<DocumentsViewModel>>> GetProjectDetails(int projectId)
        {
            var project = await _context.Projects.Include(p => p.DesignObjects)
                                                 .ThenInclude(d => d.Documents)
                                                 .ThenInclude(d => d.Mark)
                                                 .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                return NotFound();
            }

            var details = new List<DocumentsViewModel>();

            foreach (var designObject in project.DesignObjects.Where(d => d.ParentDesignObjectId is null))
            {
                await LoadChildDesignObjects(designObject);
                AddDesignObjectDetails(designObject, details);
            }

            return Ok(details);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            var proj = await _context.Projects.Include(e => e.DesignObjects)
                                              .ToListAsync();
            foreach (var project in proj)
            {
                foreach (var designObject in project.DesignObjects)
                {
                    await LoadChildDesignObjects(designObject);
                }
            }
            return Ok(proj);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProjectById(int id)
        {
            var project = await _context.Projects.Include(e => e.DesignObjects)
                                                 .ThenInclude(e => e.Documents)
                                                 .ThenInclude(e => e.Mark)
                                                 .FirstOrDefaultAsync(e => e.Id == id);
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
            var designObject = await _context.DesignObjects.Include(e => e.Documents)
                                                           .FirstOrDefaultAsync(e => e.Id == id);
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


        private async Task LoadChildDesignObjects(DesignObject parent)
        {
            if (parent == null)
            {
                return;
            }
            var children = await _context.DesignObjects
                .Where(d => d.ParentDesignObjectId == parent.Id)
                .Include(d => d.Project)
                .Include(d => d.Documents)
                .ThenInclude(d => d.Mark)
                .ToListAsync();

            parent.ChildDesignObjects = children;

            foreach (var child in children)
            {
                await LoadChildDesignObjects(child);
            }
        }

        private void AddDesignObjectDetails(DesignObject designObject, List<DocumentsViewModel> details)
        {
            foreach (var document in designObject.Documents)
            {
                var detail = new DocumentsViewModel
                {
                    ProjectCipher = designObject.Project.Cipher,
                    DesignObjectCode = designObject.GetFullCode(),
                    Mark = document.Mark.Name,
                    Number = $"{document.Mark?.ShortName}{document.Number}",
                    FullDocumentCipher = designObject.GetFullDocumentCode(document)
                };
                details.Add(detail);
            }
            if(designObject.ChildDesignObjects != null)
            {
                foreach (var child in designObject.ChildDesignObjects)
                {
                    AddDesignObjectDetails(child, details);
                }
            }

        }
    }
}
