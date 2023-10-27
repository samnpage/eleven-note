using System.Security.Claims;
using ElevenNote.Data.Entities;
using ElevenNote.Models.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ElevenNote.Services.Token;

public class TokenService : ITokenService
{
    // Fields and constructor added to access our UserEntity information.
    private readonly IConfiguration _configuration;
    private readonly UserManager<UserEntity> _userManager;

    public TokenService(IConfiguration configuration, UserManager<UserEntity> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<TokenResponse?> GetTokenAsync(TokenRequest model)
    {
        UserEntity? entity = await GetValidUserAsync(model);

        if (entity is null)
            return null;

        return await GenerateTokenAsync(entity);
        // throw new NotImplementedException();
    }

    private async Task<UserEntity?> GetValidUserAsync(TokenRequest model)
    {
        var userEntity = await _userManager.FindByNameAsync(model.UserName);

        if (userEntity is null)
            return null;

        var isValidPassword = await _userManager.CheckPasswordAsync(userEntity, model.Password);
        if (isValidPassword == false)
            return null;
        
        return userEntity;
        // throw new NotImplementedException();
    }

    private async Task<TokenResponse> GenerateTokenAsync(UserEntity entity)
    {
        throw new NotImplementedException();
    }

    private async Task<List<Claim>> GetUserClaimsAsync(UserEntity entity)
    {
        throw new NotImplementedException();
    }

    private SecurityTokenDescriptor GetTokenDescriptor(List<Claim> claims)
    {
        throw new NotImplementedException();
    }
}
