using Microsoft.AspNetCore.Mvc;
using MouseTagProject.Models;
using MouseTagProject.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MouseTagProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NoteController: Controller
    {
        private readonly INote _noteRepository;

        public NoteController(INote noteRepository)
        {
            _noteRepository = noteRepository;
        }

        [HttpPost("/api/Category/Get")]
        public IActionResult GetCategories(string jwt)
        {
            var categories = _noteRepository.GetCategories(jwt);
            if(categories.Count == 0)
            {
                return NotFound();
            }
            return Ok(categories);
        }

        [HttpPost("Get")]
        public IActionResult GetNotes(string jwt)
        {

            var notes = _noteRepository.GetNotes(jwt);
            if (notes.Count == 0)
            {
                return NotFound();
            }
            return Ok(notes);
        }

        [HttpPost("/api/Category")]
        public IActionResult AddCategory(string title, string jwt, string color)
        {
            string userId = _noteRepository.decodeJWT(jwt);
            Category category1 = new Category()
            {
                Title =title,
                userID=userId,
                color = color
            };

            _noteRepository.AddCategory(category1);
            return Ok();
        }

        [HttpPost]
        public IActionResult AddNote(string content, int categoryID, string jwt, string color)
        {
            string userId = _noteRepository.decodeJWT(jwt);
            Note note1 = new Note()
            {
                Content = content,
                CategoryId = categoryID,
                userID=userId,
                color = color
            };
            _noteRepository.AddNote(note1);
            return Ok();
        }

        [HttpDelete("/api/Category/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            _noteRepository.DeleteCategory(id);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteNote(int id)
        {
            _noteRepository.DeleteNote(id);
            return Ok();
        }

        [HttpPatch("/api/Category/{id}")]
        public IActionResult UpdateCategory(int id, string title, string color)
        {
            Category category1 = new Category()
            {
                Title = title,
                color = color
            };

            _noteRepository.UpdateCategory(id, category1);
            return Ok();
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateNote(int id, Note note)
        {
            Note note1 = new Note()
            {
                Content=note.Content,
                CategoryId= note.CategoryId,
                color = note.color
            };

            _noteRepository.UpdateNote(id, note1);
            return Ok();
        }
    }
}