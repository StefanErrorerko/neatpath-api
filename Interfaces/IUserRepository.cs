using NeatPath.Models;

namespace NeatPath.Interfaces
{
    public interface IUserRepository
    {
        ICollection<User> GetUsers();
        User GetUser(int id);
        User GetUserByUsername(string username);
        bool UserExists(int id);
        bool UpdateUser(User user);
        bool CreateUser(User user);
        bool DeleteUser(User user);
        ICollection<Session> GetUserSessions(int userId);
        ICollection<Url> GetUserUrls(int userId);
        bool Save();
    }
}
