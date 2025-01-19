using NeatPath.Data;
using NeatPath.Helpers;
using NeatPath.Interfaces;
using NeatPath.Models;
using Newtonsoft.Json.Bson;

namespace NeatPath
{
    public class Seed(DataContext context, IPasswordService passwordService, IUrlService urlService, IConfiguration configuration)
    {
        private readonly DataContext _context = context;
        private readonly IPasswordService _passwordService = passwordService;
        private readonly IUrlService _urlService = urlService;
        private readonly string? _redirectUrl = configuration["Urls:RedirectUrl"];

        public void SeedDataContext()
        {
            if (!_context.Users.Any())
            {
                var users = new List<User>() {
                    new()
                    {
                        Username = "user1234",
                        PasswordHash = _passwordService.HashPassword("QWErty1234"),
                        Role = Models.Enums.UserRole.Authorized
                    },
                    new()
                    {
                        Role = Models.Enums.UserRole.Anonymous
                    },
                    new()
                    {
                        Username = "admin",
                        PasswordHash = _passwordService.HashPassword("admin"),
                        Role = Models.Enums.UserRole.Admin
                    }
                };

                _context.Users.AddRange(users);

                if (!_context.Urls.Any())
                {
                    string url = "original.url/bjsvbsdkjvbqebjkrb1f3ob38924v2ion24872vbi2o";
                    var urls = new List<Url>()
                    {
                        new ()
                        {
                            OriginalUrl = url,
                            Hash = _urlService.HashUrl(url),
                            ShortUrl = _redirectUrl + _urlService.HashUrl(url),
                            User = users[0],
                        }
                    };

                    _context.Urls.AddRange(urls);
                }

                if (!_context.Sessions.Any())
                {

                    var sessions = new List<Session>()
                    {
                        new ()
                        {
                            Token = "Aghbjqs8",
                            User = users[0],
                            ExpiresAt = DateTime.Now + TimeSpan.FromHours(1)
                        }
                    };

                    _context.Sessions.AddRange(sessions);
                }

                _context.SaveChanges();
            }
        }
    }
}
