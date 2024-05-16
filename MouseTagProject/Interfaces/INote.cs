using MouseTagProject.DTOs;
using MouseTagProject.Models;

namespace MouseTagProject.Interfaces
{
    public interface INote
    {
        List<Note> GetNotes(string jwt);
        List<Category> GetCategories(string jwt);
        void AddNote(Note note);
        void UpdateNote(int id, Note note);
        void DeleteNote(int id);
        void AddCategory(Category category);
        void UpdateCategory(int id, Category category);
        void DeleteCategory(int id);
        string decodeJWT(string jwt);
        Task<bool> DeleteByEmail(string email);
        Task<bool> DeleteByEmailList(List<UsersDeleteDto> users);
    }
}
