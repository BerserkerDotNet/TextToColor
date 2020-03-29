using System;
using System.Buffers;
using System.Security.Cryptography;
using System.Text;

namespace TextToColor
{
    public class MD5HashProvider : IHashProvider
    {
        public ulong Hash(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            using (var md5Hash = MD5.Create())
            {
                var textBytesCount = Encoding.UTF8.GetByteCount(text);
                var textBytes = ArrayPool<byte>.Shared.Rent(textBytesCount);
                Encoding.UTF8.GetBytes(text, 0, text.Length, textBytes, 0);

                var hashBytes = md5Hash.ComputeHash(textBytes, 0, textBytesCount);
                ArrayPool<byte>.Shared.Return(textBytes);
                var hashOffset0 = BitConverter.ToUInt64(hashBytes, 0);
                var hashOffset8 = BitConverter.ToUInt64(hashBytes, 8);
                return hashOffset0 ^ hashOffset8;
            }
        }
    }
}
