using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//目录
///<see cref="MathLayoutTool"/>  矩形排列, 圆形排列
///<see cref="MathTool"/> 旋转角度相关   
///<see cref="StringTool"/> 字符串截取相关   
///<see cref="PathTool"/> 路径处理相关   
///<see cref="FileTool"/>  File IO相关
///<see cref="PlayerPrefsTool"/>  PlayerPrefs
///<see cref="LogObjectTool"/> Log相关


//矩形排列, TODO: 圆形排列
public static class MathLayoutTool 
{
    #region   矩形排列
    public enum Alignment
    {
        Left,
        Center,
        Right
    }
    private const int objectWidth = 0;
    private const int objectHeight = 0;
    //矩形排列
    public static Vector2Int GetRectPos(int xIndex, int yIndex, int xCount, Alignment alignment,int spacingX = 10,int spacingY = 10)
    {
        int startX = 0;
        int startY = 0;
        
        startY = yIndex * (objectHeight + spacingY);
        
        int x = startX + xIndex * (objectWidth + spacingX);
        switch (alignment)
        {
            case Alignment.Center:
                x += (objectWidth - spacingX * (xCount - 1)) / 2;
                break;
            case Alignment.Right:
                x += (objectWidth - spacingX) * (xCount - 1);
                break;
        }
        //Left不需要任何处理
        return new Vector2Int(x, startY);
    }
    
    //空心矩形排列
    public class RectangularArrangement
    {
        public static List<(int, int)> ArrangeObjects(int rectangleWidth, int rectangleHeight, int objectSize, int margin)
        {
            List<(int, int)> coordinates = new List<(int, int)>();

            // Calculate the number of objects that can fit on each side
            int objectsOnTop = (rectangleWidth - 2 * margin) / objectSize;
            int objectsOnSide = (rectangleHeight - 2 * margin) / objectSize;

            // Calculate the coordinates for each side
            for (int i = 0; i < objectsOnTop; i++)
            {
                int x = margin + i * objectSize;
                int y = margin;
                coordinates.Add((x, y));
            }

            for (int i = 0; i < objectsOnSide; i++)
            {
                int x = rectangleWidth - margin - objectSize;
                int y = margin + i * objectSize;
                coordinates.Add((x, y));
            }

            for (int i = objectsOnTop - 1; i >= 0; i--)
            {
                int x = margin + i * objectSize;
                int y = rectangleHeight - margin - objectSize;
                coordinates.Add((x, y));
            }

            for (int i = objectsOnSide - 1; i > 0; i--)
            {
                int x = margin;
                int y = margin + i * objectSize;
                coordinates.Add((x, y));
            }

            return coordinates;
        }
    }

    
    #endregion
    
    #region 圆形排列 TODO
    
    #endregion
}

public static class MathTool
{
    public static Vector3 ChangeDir(Vector3 dir, float angle)
    {
        if (angle == 0)
            return dir;
        //angle旋转角度 axis围绕旋转轴 position自身坐标 自身坐标 center旋转中心
        //Quaternion.AngleAxis(angle, axis) * (position - center) + center;
        return Quaternion.AngleAxis(angle, Vector3.up) * (dir);
    }

    //先慢后快 t -> [0,1]
    public static float RLerp(float start,float end,float t)
    {
        return (end - start) * t;
    }
    
    //角度转二维向量
    //从正右方开始计算,逆时针,90度为正上方
    public static Vector2 AngleToVector(float angleInDegrees)
    {
        double angleInRadians = angleInDegrees * (Math.PI / 180.0f);
        double xComponent = Math.Cos(angleInRadians);
        double yComponent = Math.Sin(angleInRadians);

        return new Vector2((float)xComponent, (float)yComponent);
    }

    //二维向量转角度
    static float GetAngleFromVector(Vector2 vector)
    {
        // 计算向量相对于 x 轴的角度（弧度）
        double angleRadians = Math.Atan2(vector.y, vector.x);
        // 将弧度转换为角度
        double angleDegrees = angleRadians * 180.0 / Math.PI;
        // 确保角度在 0 到 360 范围内
        if (angleDegrees < 0)
        {
            angleDegrees += 360;
        }
        return (float)angleDegrees;
    }
}

public static class StringTool
{
    //匹配头尾中的内容
    //如"abcd[xxx]efg",匹配[],输出"xxx"
    //withSide为True 输出"[xxx]" 
    public static string MatchSide(string input, string head, string end,bool withSide = false)
    {
        //Regex.Escape 用于转义字符串中的正则表达式特殊字符，以确保这些字符被当作普通字符而不是正则表达式元字符来处理。
        string pattern = $"{Regex.Escape(head)}(.*?){Regex.Escape(end)}";
        Match match = Regex.Match(input, pattern);
        if (match.Success)
        {
            if (withSide)
            {
                return match.Groups[0].Value;
            }
            return match.Groups[1].Value;
        }
        return null;
    }
    //移除开头
    public static string RemoveHead(this string str, string removeStr)
    {
        if (str.StartsWith(removeStr))
        {
            return str.Remove(0, removeStr.Length);
        }
        else
        {
            Debug.LogError(str + "no StartsWith" + removeStr);
            return str;
        }
    }
    //移除结尾
    public static string RemoveEnd(this string str, string removeStr)
    {
        if (str.EndsWith(removeStr))
        {
            int len = str.Length;
            return str.Remove(len - removeStr.Length, removeStr.Length);
        }
        else
        {
            Debug.LogError(str + "no EndsWith " + removeStr);
            return str;
        }
    }
}

public static class PathTool
{
    /// <summary>
    /// 获取上级级目录
    /// </summary>
    public static string GetUpperDir(string path)
    {
        var upperPath = Directory.GetParent(path)?.FullName;
        return upperPath;
    }
    

#if UNITY_EDITOR
    //获取Asset的ResourcePath
    public static string GetAssetResourcePath(UnityEngine.Object asset)
    {
        if (asset == null)
            return "";
        string path = AssetDatabase.GetAssetPath(asset);
        string str = path.RemoveHead("Assets/Resources/");
        return str.RemoveEnd(".prefab");
    }
#endif
}

public static class FileTool
{
#if UNITY_EDITOR
    public static void OpenDir(string path ,bool isAssetPath = false)
    {
        EditorUtility.RevealInFinder(PathTool.GetUpperDir(path));
    }
#endif
    
    public static void WriteToFile(string str, string filePath)
    {
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

    public static void WriteToFile(byte[] by, string filePath ,string checkDir = null)
    {
        if(checkDir!=null &&!Directory.Exists(checkDir))
            Directory.CreateDirectory(checkDir);
        File.WriteAllBytes(filePath, by);
        Debug.LogFormat("WriteToFile {0}",filePath);
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
    public static string DownloadUrlText(string url,string localFilePath)
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
            Debug.Log("yns Delete " + path);
        }
    }
    //读取贴图
    public static Texture2D LoadTexture(string path,int w =180,int h=180)
    {
        if (!IsFileExist(path))
        {
            Debug.LogFormat("yns no path {0}" , path);
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

public static class PlayerPrefsTool 
{
    public static void SetKeyBool(this string key,bool isOn)
    {
        PlayerPrefs.SetInt(key,isOn.ToInt());
    }
    public static bool GetKeyBool(this string key,bool isOn)
    {
        return PlayerPrefs.GetInt(key).ToBool();
    }

    public static bool ToBool(this int num)
    {
        return num != 0;
    }

    public static int ToInt(this bool value)
    {
        return value ? 1 : 0;
    }
}