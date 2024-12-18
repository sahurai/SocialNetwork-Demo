﻿using SocialNetwork.Core.Enums;
using SocialNetwork.Core.Models;

namespace SocialNetwork.Core.Interfaces.Services.UserService
{
    public interface IUserService
    {
        Task<(User? User, string Error)> CreateUserAsync(string username, string email, UserRole role, string password);
        Task<(Guid Id, string Error)> DeleteUserAsync(Guid id);
        Task<(List<User> Users, string Error)> GetUsersAsync(Guid? userId = null, string? username = null, string? email = null, UserRole? role = null);
        Task<(User? User, string Error)> UpdateUserAsync(Guid id, string? username, string? email, string? password);
    }
}