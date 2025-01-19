using Microsoft.EntityFrameworkCore;
using NeatPath.Data;
using NeatPath.Interfaces;
using NeatPath.Models;

namespace NeatPath.Repository
{
    public class UrlRepository : IUrlRepository
    {
        private readonly DataContext _context;
        public UrlRepository(DataContext context)
        {
            _context = context;
        }
        
        public Url GetUrl(int id)
        {
            return _context.Urls
                .Where(url => url.Id == id)
                .Include(url => url.User)
                .FirstOrDefault();
        }
        public Url GetUrlByHash(string hash)
        {
            return _context.Urls
                .Where(url => url.Hash == hash)
                .Include(url => url.User)
                .FirstOrDefault();
        }
        public Url GetUrlByOriginalUrl(string originalUrl)
        {
            return _context.Urls
                .Where(url => url.OriginalUrl == originalUrl)
                .Include(url => url.User)
                .FirstOrDefault();
        }
        public ICollection<Url> GetUrls()
        {
            return _context.Urls
                .OrderBy(url => url.Id)
                .Include(url => url.User)
                .ToList();
        }
        public bool UrlExists(int id)
        {
            return _context.Urls.Any(url => url.Id == id);
        }
        public bool CreateUrl(Url url)
        {
            _context.Add(url);
            return Save();
        }
        public bool UpdateUrl(Url url)
        {
            _context.Update(url);
            return Save();
        }
        public bool DeleteUrl(Url url)
        {
            _context.Remove(url);
            return Save();
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
