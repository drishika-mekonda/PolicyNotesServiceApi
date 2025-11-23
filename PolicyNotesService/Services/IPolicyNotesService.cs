using PolicyNotesService.Models;

namespace PolicyNotesService.Services
{
    public interface IPolicyNotesService
    {
        Task<PolicyNote> AddNoteAsync(string policyNumber, string noteText);
        Task<List<PolicyNote>> GetAllNotesAsync();
        Task<PolicyNote?> GetNoteByIdAsync(int id);
    }
}
