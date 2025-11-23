using PolicyNotesService.Models;
using PolicyNotesService.Repositories;

namespace PolicyNotesService.Services;

public class PolicyNotesService : IPolicyNotesService
{
    private readonly IPolicyNoteRepository _repository;

    public PolicyNotesService(IPolicyNoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<PolicyNote> AddNoteAsync(string policyNumber, string noteText)
    {
        if (string.IsNullOrWhiteSpace(policyNumber))
            throw new ArgumentException("Policy number is required!", nameof(policyNumber));

        if (string.IsNullOrWhiteSpace(noteText))
            throw new ArgumentException("Note text is required!", nameof(noteText));

        var entity = new PolicyNote
        {
            Policynumber = policyNumber,
            Note = noteText
        };

        return await _repository.AddAsync(entity);
    }

    public Task<List<PolicyNote>> GetAllNotesAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<PolicyNote?> GetNoteByIdAsync(int id)
    {
        return _repository.GetByIdAsync(id);
    }
}
