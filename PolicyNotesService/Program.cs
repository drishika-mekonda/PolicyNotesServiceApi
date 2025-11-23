using Microsoft.EntityFrameworkCore;
using PolicyNotesService.Data;
using PolicyNotesService.Models;
using PolicyNotesService.Repositories;
using PolicyNotesService.Services;

var builder = WebApplication.CreateBuilder(args);

// For EF Core InMemory
builder.Services.AddDbContext<PolicyNotesDbContext>(options =>
    options.UseInMemoryDatabase("PolicyNotesDb"));

// Dependency Injection
builder.Services.AddScoped<IPolicyNoteRepository, PolicyNoteRepository>();
builder.Services.AddScoped<IPolicyNotesService, PolicyNotesService.Services.PolicyNotesService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// POST policy notes
app.MapPost("/notes", async (PolicyNote request, IPolicyNotesService service) =>
{
    var created = await service.AddNoteAsync(request.Policynumber, request.Note);

    return Results.Created($"/notes/{created.Id}", created);
});

// GET all pocily notes
app.MapGet("/notes", async (IPolicyNotesService service) =>
{
    var notes = await service.GetAllNotesAsync();
    return Results.Ok(notes);
});

// GET policy notes by ID
app.MapGet("/notes/{id:int}", async (int id, IPolicyNotesService service) =>
{
    var note = await service.GetNoteByIdAsync(id);

    if (note is null)
    {
        return Results.NotFound(new { Message = $"Note with ID {id} was not found." });
    }

    return Results.Ok(note);
});

app.Run();


// Make Program discoverable for integration tests

public partial class Program { }