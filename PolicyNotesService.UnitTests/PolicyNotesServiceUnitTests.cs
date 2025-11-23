using Microsoft.EntityFrameworkCore;
using PolicyNotesService.Data;
using PolicyNotesService.Repositories;

namespace PolicyNotesService.UnitTests;

public class PolicyNotesServiceUnitTests
{
    private PolicyNotesService.Services.PolicyNotesService CreateService()
    {
        var options = new DbContextOptionsBuilder<PolicyNotesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new PolicyNotesDbContext(options);
        var repo = new PolicyNoteRepository(context);
        return new PolicyNotesService.Services.PolicyNotesService(repo);
    }

    [Fact]
    public async Task AddNote_Should_Add_Successfully()
    {
        var service = CreateService();

        var created = await service.AddNoteAsync("POL123", "Customer requested address change");

        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("POL123", created.Policynumber);
    }

    [Fact]
    public async Task GetAllNotes_Should_Return_Added_Notes()
    {
        var service = CreateService();
        await service.AddNoteAsync("POL1", "First note");
        await service.AddNoteAsync("POL2", "Second note");

        var notes = await service.GetAllNotesAsync();

        Assert.Equal(2, notes.Count);
    }
}
