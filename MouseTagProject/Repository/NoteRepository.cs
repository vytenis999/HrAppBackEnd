using Microsoft.AspNetCore.Identity;
using MouseTagProject.Context;
using MouseTagProject.DTOs;
using MouseTagProject.Interfaces;
using MouseTagProject.Models;
using System.IdentityModel.Tokens.Jwt;

namespace MouseTagProject.Repository
{
    public class NoteRepository : INote
    {
        private readonly MouseTagProjectContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NoteRepository(MouseTagProjectContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public string decodeJWT(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(jwt) as JwtSecurityToken;

            var userID = token.Claims.First(claim => claim.Type == "Id").Value;
            
            return userID;
        }
        public List<Note> GetNotes(string jwt)
        {
            var result = _context.Notes.Where(c => c.userID == decodeJWT(jwt)).ToList();
            return result;// _context.Notes.ToList();
        }

        public List<Category> GetCategories(string jwt)
        {
            var result = _context.Categories.Where(c => c.userID == decodeJWT(jwt)).ToList();
            return result;//_context.Categories.ToList();

        }

        public void AddNote(Note note)
        {
            _context.Notes.Add(note);
            _context.SaveChanges();
            
        }

        public void UpdateNote(int id, Note note)
        {
            var noteEntity = _context.Notes.FirstOrDefault(c => c.Id == id);

            noteEntity.Content = note.Content;
            noteEntity.CategoryId= note.CategoryId;
            noteEntity.color = note.color;
            _context.SaveChanges();
        }

        public void DeleteNote(int id)
        {
            var note = _context.Notes.FirstOrDefault(x => x.Id == id);
            _context.Notes.Remove(note);
            _context.SaveChanges();
        }

        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void UpdateCategory(int id, Category category)
        {
            var categoryEntity = _context.Categories.FirstOrDefault(c => c.Id == id);

            categoryEntity.Title = category.Title;
            categoryEntity.color = category.color;
            _context.SaveChanges();
        }

        public void DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(x => x.Id == id);
            _context.Categories.Remove(category);
            _context.SaveChanges();
        }

        public async Task<bool> DeleteByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var categories = _context.Categories.Where(c => c.userID == user.Id).ToList();
            foreach(var category in categories)
            {
                if (category != null)
                {
                    _context.Categories.Remove(category);
                    _context.SaveChanges();
                }
            }

            return true;
        }

        public async Task<bool> DeleteByEmailList(List<UsersDeleteDto> users)
        {
            foreach (var x in users)
            {
                var user = await _userManager.FindByEmailAsync(x.Email);
                var categories = _context.Categories.Where(c => c.userID == user.Id).ToList();
                foreach (var category in categories)
                {
                    if (category != null)
                    {
                        _context.Categories.Remove(category);
                        _context.SaveChanges();
                    }
                }
            }
            return true;
        }
    }
}
