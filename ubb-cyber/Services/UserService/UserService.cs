﻿using Microsoft.EntityFrameworkCore;
using PasswordGenerator;
using System.Security.Claims;
using ubb_cyber.Database;
using ubb_cyber.Models;
using ubb_cyber.Services.PrincipalProvider;
using ubb_cyber.Utils;

namespace ubb_cyber.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ClaimsPrincipal? _user;

        public UserService(IPrincipalProvider principalProvider, AppDbContext context)
        {
            _user = principalProvider.User;
            _context = context;
        }

        public bool IsLoggedIn()
        {
            return _user?.Identity?.IsAuthenticated != null && _user.Identity.IsAuthenticated;
        }

        public async Task<User?> GetUserFromRequest()
        {
            var userId = _user?.GetUserId();
            if (userId == null) return null;
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<User?> GetUserFromRequest(CancellationToken cancellationToken)
        {
            var userId = _user?.GetUserId();
            if (userId == null) return null;
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        }

        public async Task<bool> IsUserLocked(string login, CancellationToken cancellationToken)
        {
            return (await GetUserByLogin(login, cancellationToken))?.Locked ?? false;
        }

        public async Task<bool> IsUserLocked(string login)
        {
            return (await GetUserByLogin(login))?.Locked ?? false;
        }

        public async Task<bool> IsUserByLogin(string login, CancellationToken cancellationToken)
        {
            return await GetUserByLogin(login, cancellationToken) != null;
        }

        public async Task<bool> IsUserByLogin(string login)
        {
            return await GetUserByLogin(login) != null;
        }

        public async Task<bool> IsUserById(int id, CancellationToken cancellationToken)
        {
            return await GetUserById(id, cancellationToken) != null;
        }

        public async Task<bool> IsUserById(int id)
        {
            return await GetUserById(id) != null;
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserById(int id, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<User?> GetUserByLogin(string login)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Login == login);
        }
        public async Task<User?> GetUserByLogin(string login, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Login == login, cancellationToken);
        }

        public async Task<User> GetUserByLoginSingle(string login)
        {
            return await _context.Users.SingleAsync(x => x.Login == login);
        }

        public async Task<User?> GetUserByKey(string key, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.ResetPasswordKey == key, cancellationToken);
        }

        public async Task<User?> GetUserByKey(string key)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.ResetPasswordKey == key);
        }

        public async Task<User> GetUserByKeySingle(string key)
        {
            return await _context.Users.SingleAsync(x => x.ResetPasswordKey == key);
        }


        public string GeneratePasswordHash(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public bool ValidatePasswordHash(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
        }

        public string GenerateResetPasswordKey()
        {
            var resetKeyGenerator = new Password()
                .IncludeLowercase()
                .IncludeUppercase()
                .IncludeNumeric()
                .LengthRequired(128);
            return resetKeyGenerator.Next();
        }
    }
}
