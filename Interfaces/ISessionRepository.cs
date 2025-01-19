using NeatPath.Models;

namespace NeatPath.Interfaces
{
    public interface ISessionRepository
    {
        ICollection<Session> GetSessions();
        Session GetSession(int id);
        Session GetSessionByToken(string token);
        bool SessionExists(int id);
        bool SessionExpired(int id);
        bool CreateSession(Session session);
        bool UpdateSession(Session session);
        bool DeleteSession(Session session);
        bool Save();
    }
}
