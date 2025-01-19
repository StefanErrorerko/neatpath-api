namespace NeatPath.Interfaces
{
    public interface IUrlService
    {
        public string HashUrl(string url);
        public bool VerifyUrl(string longUrl, string shortUrl);
    }
}
