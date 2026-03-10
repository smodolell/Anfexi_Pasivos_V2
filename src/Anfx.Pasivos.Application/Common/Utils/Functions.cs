using System.Security.Cryptography;
using System.Text;

namespace Anfx.Pasivos.Application.Common.Utils;

public static class Functions
{
    private static readonly byte[] bytes = Encoding.ASCII.GetBytes("Anfexi12");
    public static string Encrypt(string originalString)
    {
        if (String.IsNullOrEmpty(originalString))
        {
            return "";
        }
        var cryptoProvider = new DESCryptoServiceProvider();
        var memoryStream = new MemoryStream();
        var cryptoStream = new CryptoStream(memoryStream,
            cryptoProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);
        var writer = new StreamWriter(cryptoStream);
        writer.Write(originalString);
        writer.Flush();
        cryptoStream.FlushFinalBlock();
        writer.Flush();
        return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
    }



    public static string Decrypt(string cryptedString)
    {
        if (String.IsNullOrEmpty(cryptedString))
        {
            return "";
        }

        var cryptoProvider = new DESCryptoServiceProvider();
        var memoryStream = new MemoryStream
                (Convert.FromBase64String(cryptedString));
        var cryptoStream = new CryptoStream(memoryStream,
            cryptoProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
        var reader = new StreamReader(cryptoStream);
        return reader.ReadToEnd();
    }
}
