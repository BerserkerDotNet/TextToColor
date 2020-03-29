using System;
using System.Buffers;
using System.Security.Cryptography;
using System.Text;

namespace TextToColor
{
    public class SHA256HashProvider : IHashProvider
    {
        public ulong Hash(string text)
        {

            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            using (var hash = new SHA256CryptoServiceProvider())
            {
                var textBytesCount = Encoding.UTF8.GetByteCount(text);
                var textBytes = ArrayPool<byte>.Shared.Rent(textBytesCount);
                Encoding.UTF8.GetBytes(text, 0, text.Length, textBytes, 0);

                var hashBytes = hash.ComputeHash(textBytes, 0, textBytesCount);
                ArrayPool<byte>.Shared.Return(textBytes);
                
                var hashOffset0 = BitConverter.ToUInt64(hashBytes, 0);
                var hashOffset8 = BitConverter.ToUInt64(hashBytes, 8);
                var hashOffset16 = BitConverter.ToUInt64(hashBytes, 16);
                var hashOffset24 = BitConverter.ToUInt64(hashBytes, 24);
                return hashOffset0 ^ hashOffset8 ^ hashOffset16 ^ hashOffset24;
            }
        }
    }
}
