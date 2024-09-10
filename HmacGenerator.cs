using System.Security.Cryptography;
using System.Text;
using RPSGame;

interface IHmacGenerator
{
    string GenerateKey();
    string ComputeHmac(string key, string message);
}

class Sha256HmacGenerator : IHmacGenerator
{

    // Generates a cryptographically secure key for HMAC operations.
    // 
    // Returns:
    //     A hexadecimal string representation of the generated key.
    public string GenerateKey()
{
    var key = new byte[Constants.HmacKeyLength];
    using (var rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(key);
    }
    return BitConverter.ToString(key).Replace("-", "").ToLower();
}

        /// <summary>
        /// Computes the HMAC hash of the given message using the provided key.
        /// </summary>
        /// <param name="key">The key used for HMAC computation.</param>
        /// <param name="message">The message to be hashed.</param>
        /// <returns>The hexadecimal string representation of the computed HMAC hash.</returns>
    public string ComputeHmac(string key, string message)
    {
        var keyBytes = Enumerable.Range(0, key.Length / 2)
            .Select(x => Convert.ToByte(key.Substring(x * 2, 2), 16))
            .ToArray();
        using var hmac = new HMACSHA256(keyBytes);
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}