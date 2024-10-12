using SocialNetwork.ApplicationLogic.Services;
using SocialNetwork.DataAccess.Repositories;
using SocialNetwork.DataAccess;

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Core.Interfaces.Repositories.PostRepository;
using SocialNetwork.Core.Interfaces.Services.UserService;
using SocialNetwork.Core.Interfaces.Repositories.UserRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SocialNetwork.ApplicationLogic.Services.Auth;
using SocialNetwork.DataAccess.Repository.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Create Swagger documents for Admin and User areas
    c.SwaggerDoc("Admin", new OpenApiInfo { Title = "Admin API", Version = "v1" });
    c.SwaggerDoc("User", new OpenApiInfo { Title = "User API", Version = "v1" });

    // Enable grouping by ApiExplorerSettings GroupName
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo))
            return false;

        var versions = methodInfo.DeclaringType
            .GetCustomAttributes(true)
            .OfType<ApiExplorerSettingsAttribute>()
            .Select(attr => attr.GroupName);

        return versions.Any(v => v == docName);
    });

    // Optionally include XML comments (if you have them)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// HttpClient registration
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

// Auth
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

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

// Auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Doesn't work without it
    options.UseSecurityTokenValidators = true;

    // Params
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["AccessToken"];
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine(context.Exception);
            return Task.CompletedTask;
        }
    };
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
        // Add Swagger endpoints for both Admin and User areas
        c.SwaggerEndpoint("/swagger/User/swagger.json", "User API");
        c.SwaggerEndpoint("/swagger/Admin/swagger.json", "Admin API");

        // Set the Swagger UI at the application's root
        c.RoutePrefix = string.Empty;
    });
}

// Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();

// Auth
app.UseAuthentication();
app.UseAuthorization();

// Map controller endpoints
app.MapControllers();

// Run the application
app.Run();
