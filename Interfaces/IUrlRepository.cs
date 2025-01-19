using NeatPath.Models;

namespace NeatPath.Interfaces
{
    public interface IUrlRepository
    {
        ICollection<Url> GetUrls();
        Url GetUrl(int id);
        Url GetUrlByHash(string hash);
        Url GetUrlByOriginalUrl(string originalUrl);
        bool UrlExists(int id);
        bool CreateUrl(Url url);
        bool UpdateUrl(Url url);
        bool DeleteUrl(Url url);
        bool Save();
    }
}
