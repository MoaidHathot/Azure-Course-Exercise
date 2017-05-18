﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CodeTweet.IdentityDal
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationIdentityContext _context;

        public UserRepository(ApplicationIdentityContext context)
        {
            _context = context;
        }

        public Task<string[]> GetUsersWithNotificationsAsync()
        {
            return _context.Users.Where(user => user.Notifications).Select(user => user.UserName).ToArrayAsync();
        }
    }
}