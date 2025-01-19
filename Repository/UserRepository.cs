using Microsoft.EntityFrameworkCore;
using NeatPath.Data;
using NeatPath.Interfaces;
using NeatPath.Models;
using System.Linq;

namespace NeatPath.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public User GetUser(int id)
        {
            return _context.Users.Where(u => u.Id == id).FirstOrDefault();
        }
        public ICollection<User> GetUsers()
        {
            return _context.Users.OrderBy(u => u.Id).ToList();
        }
        public bool UserExists(int id)
        {
            return _context.Users.Any(u => u.Id == id);
        }
        public ICollection<Session> GetUserSessions(int userId)
        {
            return _context.Sessions
                .Include(s => s.User)
                .Where(s => s.User.Id == userId)
                .ToList();
        }
        public ICollection<Url> GetUserUrls(int userId)
        {
            return _context.Urls
                .Include(url => url.User)
                .Where(url => url.User.Id == userId)
                .ToList();
        }
        public bool CreateUser(User user)
        {
            _context.Add(user);
            return Save();
        }
        public bool UpdateUser(User user)
        {
            _context.Update(user);
            return Save();
        }
        public bool DeleteUser(User user)
        {
            _context.Remove(user);
            return Save();
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
