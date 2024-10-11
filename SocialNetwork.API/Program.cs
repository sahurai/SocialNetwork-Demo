using SocialNetwork.ApplicationLogic.Services;
using SocialNetwork.DataAccess.Repositories;
using SocialNetwork.DataAccess;

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Core.Interfaces.Repositories.PostRepository;
using SocialNetwork.Core.Interfaces.Services.UserService;
using SocialNetwork.Core.Interfaces.Repositories.UserRepository;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add a single HttpClient registration
builder.Services.AddHttpClient();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Configure PostgreSQL Database
builder.Services.AddDbContext<SocialNetworkDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("SocialNetworkDbContext"));
});

// Register application services and repositories
// Comments 
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

// Likes 
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();

// Posts
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IPostRepository, PostRepository>();

// Friendships
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
builder.Services.AddScoped<IFriendshipRepository, FriendshipRepository>();

// User Blocks
builder.Services.AddScoped<IUserBlockService, UserBlockService>();
builder.Services.AddScoped<IUserBlockRepository, UserBlockRepository>();

// Users 
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Messages 
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

// Groups 
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();

// Group Blocks 
builder.Services.AddScoped<IGroupBlockService, GroupBlockService>();
builder.Services.AddScoped<IGroupBlockRepository, GroupBlockRepository>();

// Group User Roles
builder.Services.AddScoped<IGroupUserRoleService, GroupUserRoleService>();
builder.Services.AddScoped<IGroupUserRoleRepository, GroupUserRoleRepository>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5000",
                "https://localhost:5001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Build the application
var app = builder.Build();

// Enable CORS
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SocialNetwork API V1");
        c.RoutePrefix = string.Empty;
    });
}

// Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();

// Map controller endpoints
app.MapControllers();

// Run the application
app.Run();
