using System.Security.Claims;
using AutoMapper;
using ElevenNote.Data;
using ElevenNote.Data.Entities;
using ElevenNote.Models.Note;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace ElevenNote.Services.Note;

public class NoteService : INoteService
{
    private readonly int _userId;
    private readonly IMapper? _mapper;
    private readonly ApplicationDbContext _dbContext;

    public NoteService(IHttpContextAccessor httpContextAccessor, IMapper mapper,
                        ApplicationDbContext dbContext)
    {
        var userClaims = httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
        var value = userClaims?.FindFirst("Id")?.Value;
        var validId = int.TryParse(value, out _userId);

        if (!validId)
            throw new Exception("Attempted to build NoteService without Id Claim.");

        _dbContext = dbContext;
    }

    // public NoteService(UserManager<UserEntity> userManager,
    //                     SignInManager<UserEntity> signInManager,
    //                     ApplicationDbContext dbContext)
    // {
    //     var currentUser = signInManager.Context.User;
    //     var userIdClaim = userManager.GetUserId(currentUser);
    //     var hasValidId = int.TryParse(userIdClaim, out _userId);

    //     if (!hasValidId)
    //         throw new Exception("Attempted to build NoteService without Id Claim.");

    //     _dbContext = dbContext;
    // }

    public async Task<NoteListItem?> CreateNoteAsync(NoteCreate request)
    {
        var noteEntity = _mapper.Map<NoteCreate, NoteEntity>(request, opt =>
            opt.AfterMap((src, dest) => dest.OwnerId = _userId)
        );

        _dbContext.Notes.Add(noteEntity);
        var numberOfChanges = await _dbContext.SaveChangesAsync();

        if (numberOfChanges == 1)
        {
            NoteListItem response = _mapper.Map<NoteEntity, NoteListItem>(noteEntity);
            return response;
        }

        return null;
    }

    // public async Task<NoteListItem?> CreateNoteAsync(NoteCreate request)
    // {
    //     NoteEntity entity = new()
    //     {
    //         Title = request.Title,
    //         Content = request.Content,
    //         OwnerId = _userId,
    //         CreatedUtc = DateTimeOffset.Now
    //     };

    //     _dbContext.Notes.Add(entity);
    //     var numberOfChanges = await _dbContext.SaveChangesAsync();

    //     if (numberOfChanges != 1)
    //     {
    //         return null;
    //     }

    //     NoteListItem response = new()
    //     {
    //         Id = entity.Id,
    //         Title = entity.Title,
    //         CreatedUtc = entity.CreatedUtc
    //     };
        
    //     return response;
    // }

    public async Task<IEnumerable<NoteListItem>> GetAllNotesAsync()
    {
        var notes = await _dbContext.Notes
            .Where(entity => entity.OwnerId == _userId)
            .Select(entity => _mapper.Map<NoteEntity, NoteListItem>(entity))
            .ToListAsync();
        
        return notes;
    }

    // public async Task<IEnumerable<NoteListItem>> GetAllNotesAsync()
    // {
    //     List<NoteListItem> notes = await _dbContext.Notes
    //         .Where(entity => entity.OwnerId == _userId)
    //         .Select(entity => new NoteListItem
    //         {
    //             Id = entity.Id,
    //             Title = entity.Title,
    //             CreatedUtc = entity.CreatedUtc
    //         })
    //         .ToListAsync();
        
    //     return notes;
    // }

    public async Task<NoteDetail?> GetNoteByIdAsync(int noteId)
    {
        // Find the first note that has the given Id
        // an an OwnerId that matches the requesting _userId
        var noteEntity = await _dbContext.Notes
            .FirstOrDefaultAsync(e =>
                e.Id == noteId && e.OwnerId == _userId
            );

        // If the note entity is null then return null
        // Otherwise initialize and return a new NoteDetail
        return noteEntity is null ? null : _mapper.Map<NoteEntity, NoteDetail>(noteEntity);
    }

    // public async Task<NoteDetail?> GetNoteByIdAsync(int noteId)
    // {
    //     // Find the first note that has the given Id
    //     // an an OwnerId that matches the requesting _userId
    //     NoteEntity? entity = await _dbContext.Notes
    //         .FirstOrDefaultAsync(e =>
    //             e.Id == noteId && e.OwnerId == _userId
    //         );

    //     // If the note entity is null then return null
    //     // Otherwise initialize and return a new NoteDetail
    //     return entity is null ? null : new NoteDetail
    //     {
    //         Id = entity.Id,
    //         Title = entity.Title,
    //         Content = entity.Content,
    //         CreatedUtc = entity.CreatedUtc,
    //         ModifiedUtc = entity.ModifiedUtc
    //     };
    // }

    public async Task<bool> UpdateNoteAsync(NoteUpdate request)
    {
        // Check the database to see if there's a entity that matches the request info
        // Return true if any entity exists
        var noteIsUserOwned = await _dbContext.Notes.AnyAsync(note =>
            note.Id == request.Id && note.OwnerId == _userId
        );

        // If the Any check returns false then we know the note either does not exist
        // Or the note is not owned by the requesting user
        if (!noteIsUserOwned)
            return false;

        // Map from update to Entity and set the OwnerId again
        var newEntity = _mapper.Map<NoteUpdate, NoteEntity>(request, opt =>
            opt.AfterMap((src,dest) => dest.OwnerId = _userId)
        );

        // Update the Entry state, which is another way to tell the DbContext something has changed
        _dbContext.Entry(newEntity).State = EntityState.Modified;

        // Because we currently don't have access to our CreatedUtc value, we'll just mark it as not modified
        _dbContext.Entry(newEntity).Property(e => e.CreatedUtc).IsModified = false;

        // Save the changes to the database and capture how many rows were updated
        var numberOfChanges = await _dbContext.SaveChangesAsync();

        // numberOfChanges is stated to be equal to 1 because only one row is updated.
        return numberOfChanges == 1;
    }

    // public async Task<bool> UpdateNoteAsync(NoteUpdate request)
    // {
    //     // Find the note and validate it's owned by the user
    //     NoteEntity? entity = await _dbContext.Notes.FindAsync(request.Id);

    //     // By using the null conditional operator we can check if it's null
    //     // and at the same time we check the OwnerId vs the _userId
    //     if (entity?.OwnerId != _userId)
    //         return false;

    //     // Now we update the entity's properties
    //     entity.Title = request.Title;
    //     entity.Content = request.Content;
    //     entity.ModifiedUtc = DateTimeOffset.Now;

    //     // Save the changes to the database and capture how many rows were updated
    //     int numberOfChanges = await _dbContext.SaveChangesAsync();

    //     // numberOfChanges is stated to be equal to 1 because only one row is updated
    //     return numberOfChanges == 1;
    // }

    public async Task<bool> DeleteNoteAsync(int noteId)
    {
        // Find the note by the given Id
        var noteEntity = await _dbContext.Notes.FindAsync(noteId);

        // Validate the note exists and is owned by the user
        if (noteEntity?.OwnerId != _userId)
            return false;

        // Remove the note from the DBContext and assert that the one change was saved
        _dbContext.Notes.Remove(noteEntity);
        return await _dbContext.SaveChangesAsync() == 1;
    }
}