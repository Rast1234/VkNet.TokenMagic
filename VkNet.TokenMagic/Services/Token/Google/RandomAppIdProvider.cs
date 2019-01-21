using System;
using System.Text;

namespace VkNet.TokenMagic.Services.Token.Google
{
    public class RandomAppIdProvider
    {
        public RandomAppIdProvider()
        {
            AppId = GenerateRandomString(11);
        }

        public string AppId { get; }

        private string GenerateRandomString(int length)
        {
            var sb = new StringBuilder(length);
            for (var i = 0; i < length; i++)
            {
                sb.Append(Alphabet[_random.Next(Alphabet.Length)]);
            }
            return sb.ToString();
        }

        private readonly Random _random = new Random();
        private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_-";
    }
}