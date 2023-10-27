using ElevenNote.Data;
using ElevenNote.Data.Entities;
using ElevenNote.Services.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Adds connection string to variable.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Adds connection string to our DbContext.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// Leverages Identity framework
builder.Services.AddDefaultIdentity<UserEntity>(options =>
{
    // Password configuration, optional
    options.Password.RequiredLength = 4;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddRoles<IdentityRole<int>>() // Enable Roles, optional
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllers();
// Add User Service/Interface for Dependancy Injection here
builder.Services.AddScoped<IUserService, UserService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

// Adds AuthenticationMiddleware to the IApplicationBuilder
// This enables authentication capabilities
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
