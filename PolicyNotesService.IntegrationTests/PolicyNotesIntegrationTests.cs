using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using PolicyNotesService.Models;
using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PolicyNotesService.Data;

namespace PolicyNotesService.IntegrationTests;

public class PolicyNotesIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private const string InMemoryDbName = "PolicyNotesTestDb";

    public PolicyNotesIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClientWithInMemoryDb(bool clearDatabase = true)
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<PolicyNotesDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<PolicyNotesDbContext>(options =>
                    options.UseInMemoryDatabase(InMemoryDbName));
            });
        });

        var client = factory.CreateClient();

        if (clearDatabase)
        {
            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PolicyNotesDbContext>();
            db.PolicyNotes.RemoveRange(db.PolicyNotes);
            db.SaveChanges();
        }

        return client;
    }

    [Fact]
    public async Task PostNotes_ReturnsCreated()
    {
        var client = CreateClientWithInMemoryDb();
        var request = new { PolicyNumber = "POL999", Note = "Integration test note" };

        var response = await client.PostAsJsonAsync("/notes", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<PolicyNote>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
    }

    [Fact]
    public async Task GetNotes_ReturnsOk()
    {
        var client = CreateClientWithInMemoryDb();
        await client.PostAsJsonAsync("/notes", new { PolicyNumber = "POL1", Note = "Test note" });

        var response = await client.GetAsync("/notes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<List<PolicyNote>>();
        Assert.NotNull(list);
        Assert.NotEmpty(list!);
    }

    [Fact]
    public async Task GetNoteById_WhenFound_ReturnsOk()
    {
        var client = CreateClientWithInMemoryDb();

        var postResponse = await client.PostAsJsonAsync("/notes",
            new { PolicyNumber = "POLX", Note = "Specific note" });

        var created = await postResponse.Content.ReadFromJsonAsync<PolicyNote>();
        Assert.NotNull(created);

        var getResponse = await client.GetAsync($"/notes/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetched = await getResponse.Content.ReadFromJsonAsync<PolicyNote>();
        Assert.Equal(created.Id, fetched!.Id);
    }

    [Fact]
    public async Task GetNoteById_WhenMissing_ReturnsNotFound()
    {
        var client = CreateClientWithInMemoryDb();

        var response = await client.GetAsync("/notes/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
