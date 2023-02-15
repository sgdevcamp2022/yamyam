using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class FileIO : MonoBehaviour
{
    static string _filePath = "Assets/Script/Common/secret.txt";
    static string _fileRePath = "Assets/Script/Common/secretR.txt";
    static string _filekeyPath = "Assets/Script/Common/secretK.txt";
    static StreamWriter _sw;
    static private StreamReader _fileReader;
    static string _content;
    public static void SaveTokenFile(string encrypt)
    {
            _sw = new StreamWriter(_filePath);
            _sw.WriteLine(encrypt);

        _sw.Flush();
        _sw.Close();
    }

    public static void SaveReTokenFile(string encrypt)
    {
        _sw = new StreamWriter(_fileRePath);
        _sw.WriteLine(encrypt);

        _sw.Flush();
        _sw.Close();
    }

    public static string GetTokenFile()
    {
        _fileReader = new StreamReader(_filePath);
        _content = _fileReader.ReadLine();
        _fileReader.Close();
        return _content;
    }
    public static string GetReTokenFile()
    {
        _fileReader = new StreamReader(_fileRePath);
        _content = _fileReader.ReadLine();
        _fileReader.Close();
        return _content;
    }

    public static byte[] GetKeyFile()
    {
        _fileReader = new StreamReader(_filekeyPath);
        _content = _fileReader.ReadLine();
        _fileReader.Close();
        return System.Convert.FromBase64String(_content);
    }
}
