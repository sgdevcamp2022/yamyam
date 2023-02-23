
using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public enum CryptoType {
    AccessToken,
    RefreshToken
}

public class Crypto : MonoBehaviour
{

    private static readonly string s_key = Encoding.UTF8.GetString(FileIO.GetKeyFile()).Substring(0,16);

    public static string AESEncrypt128(string target , CryptoType type)
    {
        byte[] _tokenBytes = Encoding.UTF8.GetBytes(target);
        RijndaelManaged rijndael = new RijndaelManaged();
        rijndael.Mode = CipherMode.CBC;
        rijndael.Padding = PaddingMode.PKCS7;
        rijndael.KeySize = 128;
        MemoryStream _memoryStream = new MemoryStream();
        ICryptoTransform _encryptor = rijndael.CreateEncryptor(Encoding.UTF8.GetBytes(s_key), Encoding.UTF8.GetBytes(s_key));

        CryptoStream _cryptoStream = new CryptoStream(_memoryStream, _encryptor, CryptoStreamMode.Write);
        _cryptoStream.Write(_tokenBytes, 0, _tokenBytes.Length);
        _cryptoStream.FlushFinalBlock();

        byte[] _encryptBytes = _memoryStream.ToArray();
        string _encryptString = Convert.ToBase64String(_encryptBytes);

        _cryptoStream.Close();
        _memoryStream.Close();

        switch(type)
        {
            case CryptoType.AccessToken:
                FileIO.SaveTokenFile(_encryptString);
                break;
            case CryptoType.RefreshToken:
                FileIO.SaveReTokenFile(_encryptString);
                break;
        }
      


        return _encryptString;

    }
  

    public static string AESDecrypt128(CryptoType type)
    {
        byte[] _encryptBytes= null;
        switch (type)
        {
            case CryptoType.AccessToken:
                _encryptBytes = Convert.FromBase64String(FileIO.GetTokenFile());
                break;
            case CryptoType.RefreshToken:
                _encryptBytes = Convert.FromBase64String(FileIO.GetReTokenFile());
                break;
        }

        RijndaelManaged rijndael = new RijndaelManaged();
        rijndael.Mode = CipherMode.CBC;
        rijndael.Padding = PaddingMode.PKCS7;
        rijndael.KeySize = 128;

        MemoryStream _memoryStream = new MemoryStream(_encryptBytes);
        ICryptoTransform _decryptor = rijndael.CreateDecryptor(Encoding.UTF8.GetBytes(s_key), Encoding.UTF8.GetBytes(s_key));
        CryptoStream _cryptoStream = new CryptoStream(_memoryStream, _decryptor, CryptoStreamMode.Read);

        byte[] _decryptBytes = new byte[_encryptBytes.Length];
        int _decryptCount = _cryptoStream.Read(_decryptBytes, 0, _decryptBytes.Length);
        string _decryptString = Encoding.UTF8.GetString(_decryptBytes, 0, _decryptCount);

        _cryptoStream.Close();
        _memoryStream.Close();

        return _decryptString;
    }
    
}
