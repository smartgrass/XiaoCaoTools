using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using OdinSerializer;
using SerializationUtility = OdinSerializer.SerializationUtility;
using UnityEngine.XR;
using XiaoCao;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class FileTool
{
#if UNITY_EDITOR
    public static void OpenDir(string path, bool isAssetPath = false)
    {
        EditorUtility.RevealInFinder(PathTool.GetUpperDir(path));
    }
#endif

    public static void WriteToFile(string str, string filePath, bool autoCreatDir = false)
    {
        if (autoCreatDir)
        {
            CheckFilePathDir(filePath);
        }
        using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            sw.Write(str);
            sw.Close();
        }
    }

    public static void WriteLineToFile(List<string> strList, string filePath)
    {
        string tempfile = Path.GetTempFileName();
        using (var writer = new StreamWriter(tempfile))
        {
            foreach (var str in strList)
            {
                writer.WriteLine(str);
            }
        }
        File.Copy(tempfile, filePath, true);
        //删除临时文件
        if (File.Exists(tempfile))
        {
            Debug.Log("删除临时文件: " + tempfile);
            File.Delete(tempfile);
        }
    }

    public static void WriteToFile(byte[] by, string filePath, bool autoCreatDir = false)
    {
        if (autoCreatDir)
        {
            CheckFilePathDir(filePath);
        }

        File.WriteAllBytes(filePath, by);
        Debug.LogFormat("WriteToFile {0}", filePath);
    }

    public static void WriteAllBytes(string filePath, byte[] by, bool autoCreatDir = false)
    {
        if (autoCreatDir)
        {
            CheckFilePathDir(filePath);
        }
        File.WriteAllBytes(filePath, by);
    }

    public static void CheckFilePathDir(string filePath)
    {
        string dirPath = PathTool.GetUpperDir(filePath);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
    }

    public static void SerializeWrite<T>(string path, T data)
    {
        byte[] bytes = SerializationUtility.SerializeValue<T>(data, DataFormat.Binary);
        FileTool.WriteAllBytes(path, bytes, true);
    }

    public static T DeserializeRead<T>(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        T data = OdinSerializer.SerializationUtility.DeserializeValue<T>(bytes, DataFormat.Binary);
        return data;
    }


    public static byte[] ReadByte(string filePath)
    {
        return File.ReadAllBytes(filePath);
    }

    public static List<string> ReadFileLines(string filePath)
    {
        List<string> strList = new List<string>();
        try
        {
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    strList.Add(reader.ReadLine());
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        return strList;
    }

    public static string ReadFileString(string filePath)
    {
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(filePath);
        }
        catch (Exception)
        {
            Debug.LogError(filePath);
            return "";
        }
        return sr.ReadToEnd();
    }
    //读取Url内容
    public static string ReadFileWebUrl(string url)
    {
        WebClient client = new WebClient();
        byte[] buffer = client.DownloadData(new Uri(url));
        string res = Encoding.UTF8.GetString(buffer);
        return res;
    }
    //下载Url内容
    public static string DownloadUrlText(string url, string localFilePath)
    {
        string str = ReadFileWebUrl(url);
        WriteToFile(str, localFilePath);
        return str;
    }

    public static bool IsFileExist(string path)
    {
        return File.Exists(path);
    }

    // 删除文件夹
    public static void DeleteDirectory(string path)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        if (dir.Exists)
        {
            dir.Delete(true);
            Debug.Log(" Delete " + path);
        }
    }
    //读取贴图
    public static Texture2D LoadTexture(string path, int w = 180, int h = 180)
    {
        if (!IsFileExist(path))
        {
            Debug.LogFormat(" no path {0}", path);
            return null;
        }
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        fs.Seek(0, SeekOrigin.Begin);
        byte[] bytes = new byte[fs.Length];
        fs.Read(bytes, 0, (int)fs.Length);
        fs.Close();
        fs.Dispose();

        Texture2D t = new Texture2D(w, h);
        t.LoadImage(bytes);
        return t;
    }

}
