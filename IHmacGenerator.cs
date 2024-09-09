using System.Security.Cryptography;
using System.Text;

// Interface for generating HMAC
interface IHmacGenerator
{
    string GenerateKey();
    string ComputeHmac(string key, string message);
}

// Class for generating HMAC using SHA256
class Sha256HmacGenerator : IHmacGenerator
{

    public string GenerateKey()
    {
        var key = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }
        return BitConverter.ToString(key).Replace("-", "").ToLower();
    }

    public string ComputeHmac(string key, string message)
    {
        var keyBytes = Enumerable.Range(0, key.Length / 2)
            .Select(x => Convert.ToByte(key.Substring(x * 2, 2), 16))
            .ToArray();
        using (var hmac = new HMACSHA256(keyBytes))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}