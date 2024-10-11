﻿using SocialNetwork.Core.Models;

namespace SocialNetwork.Core.Interfaces.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user);
        Task<Guid> DeleteAsync(Guid id);
        Task<List<User>> GetAsync(Guid? userId = null, string? userName = null, string? email = null);
        Task<User> UpdateAsync(Guid id, User updatedUser);
    }
}