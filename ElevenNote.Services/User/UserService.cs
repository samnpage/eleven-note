using ElevenNote.Data;
using ElevenNote.Data.Entities;
using ElevenNote.Models.User;

namespace ElevenNote.Services.User;

public class UserService : IUserService 
{
    // Field
    private readonly ApplicationDbContext _context;

    // Constructor that applies ApplicationDbContext's value to a readonly field above^.
    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> RegisterUserAsync(UserRegister model)
    {
        // Calls our UserEntity entity and applys each property value collected to its respective property.
        UserEntity entity = new()
        {
            Email = model.Email,
            Username = model.UserName,
            Password = model.Password,
            DateCreated = DateTime.Now
        };

        // Adds our new entity object to _context.Users DbSet. This will add the entity to the Users table.
        _context.Users.Add(entity);
        // Returns number of rows changed in the db and stores it into a variable.
        int numberOfChanges = await _context.SaveChangesAsync();

        // returns a boolean value of true because we are expecting at least a single change.
        return numberOfChanges == 1;
    }
}