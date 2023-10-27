using ElevenNote.Data;
using ElevenNote.Data.Entities;
using ElevenNote.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ElevenNote.Services.User;

public class UserService : IUserService
{
    // Fields
    private readonly ApplicationDbContext _context;
    private readonly UserManager<UserEntity> _userManager;
    private readonly SignInManager<UserEntity> _signInManager;

    // Constructor that applies ApplicationDbContext's value to a readonly field above^.
    public UserService(ApplicationDbContext context,
                        UserManager<UserEntity> userManager,
                        SignInManager<UserEntity> signInManager)

    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    

    // public UserService(ApplicationDbContext context)
    // {
    //     _context = context;
    // }
    public async Task<bool> RegisterUserAsync(UserRegister model)
    {
        //  checks the returned value from both methods. If either return anything but null, we'll know it's invalid data.
        if (await CheckEmailAvailability(model.Email) == false || await CheckUserNameAvailability(model.UserName) == false)
            return false;

        // Calls our UserEntity entity and applys each property value collected to its respective property.
        UserEntity entity = new()
        {
            Email = model.Email,
            UserName = model.UserName,
            DateCreated = DateTime.Now
        };

        IdentityResult registerResult = await _userManager.CreateAsync(entity, model.Password);

        return registerResult.Succeeded;

        //Checks if username exists in the database or not.      
        // Adds our new entity object to _context.Users DbSet. This will add the entity to the Users table.
        // _context.Users.Add(entity);
        // Returns number of rows changed in the db and stores it into a variable.
        // int numberOfChanges = await _context.SaveChangesAsync();

        // returns a boolean value of true because we are expecting at least a single change.
        // return numberOfChanges == 1;

    }

    // Gets user info by id. Returns null if it does not exist.
    public async Task<UserDetail?> GetUserByIdAsync(int userId)
    {
        UserEntity? entity = await _context.Users.FindAsync(userId);
        if (entity is null)
            return null;

        UserDetail detail = new()
        {
            Id = entity.Id,
            Email = entity.Email!,
            UserName = entity.UserName!,
            FirstName = entity.FirstName!,
            LastName = entity.LastName,
            DateCreated = entity.DateCreated
        };

        return detail;
    }

    // Helper Methods
    // Checks whether the user's email is unique
    private async Task<bool> CheckEmailAvailability(string email)
    {
        UserEntity? existingUser = await _userManager.FindByEmailAsync(email);
        return existingUser is null;
    }

    // Checks whether the user's username is unique
    private async Task<bool> CheckUserNameAvailability(string userName)
    {
        UserEntity? existingUser = await _userManager.FindByNameAsync(userName);
        return existingUser is null;
    }
}