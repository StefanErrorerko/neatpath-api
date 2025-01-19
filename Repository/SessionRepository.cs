using Microsoft.EntityFrameworkCore;
using NeatPath.Data;
using NeatPath.Interfaces;
using NeatPath.Models;

namespace NeatPath.Repository
{
    public class SessionRepository : ISessionRepository
    {
        private readonly DataContext _context;
        public SessionRepository(DataContext context)
        {
            _context = context;
        }
        public Session GetSession(int id)
        {
            return _context.Sessions
                .Where(s => s.Id == id)
                .Include(s => s.User)
                .FirstOrDefault();
        }
        public ICollection<Session> GetSessions()
        {
            return _context.Sessions
                .Include(s => s.User)
                .OrderBy(s => s.Id)
                .ToList();
        }
        public bool SessionExists(int id)
        {
            return _context.Sessions.Any(s => s.Id == id);
        }
        public bool CreateSession(Session session)
        {
            _context.Add(session);
            return Save();
        }
        public bool UpdateSession(Session session)
        {
            _context.Update(session);
            return Save();
        }
        public bool DeleteSession(Session session)
        {
            _context.Remove(session);
            return Save();
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
