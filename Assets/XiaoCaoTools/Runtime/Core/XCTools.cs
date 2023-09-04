using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

//MathLayoutTool
///<see cref="MathLayoutTool"/>  排队  
///<see cref="MathTool"/>    
///<see cref=""/>  反射相关
///<see cref="PlayerPrefsTool"/>  PlayerPrefs
///<see cref="FileTool"/>  File IO相关
///
///
///


public static class MathLayoutTool 
{

    //直线排列,矩形排列, 圆形排列
    public enum Alignment
    {
        Left,
        Center,
        Right
    }

    #region   矩形排列
    
    private const int objectWidth = 0;
    private const int objectHeight = 0;
    private const int spacingX = 10;
    private const int spacingY = 10;
    //矩形排列
    public static Vector2Int GetRectPos(int xIndex, int yIndex, int xCount, Alignment alignment)
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
    
    /// <summary>
    ///  角度转二维向量
    ///  从正右方开始计算,逆时针,90度为正上方
    /// </summary>
    /// <param name="angleInDegrees"></param>
    /// <returns></returns>
    public static Vector2 AngleToVector(float angleInDegrees)
    {
        double angleInRadians = angleInDegrees * (Math.PI / 180.0f);
        double xComponent = Math.Cos(angleInRadians);
        double yComponent = Math.Sin(angleInRadians);

        return new Vector2((float)xComponent, (float)yComponent);
    }
    /// <summary>
    /// 二维向量转角度
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
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


public static class MathTool
{
    public static Vector3 ChageDir(Vector3 dir, float angle)
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
}

public static class StringTool
{
    public static string LogStr(this string str,string title = "Log")
    {
        if(!string.IsNullOrEmpty(str))
            Debug.LogFormat("{0}: {1}",title ,str);
        return str;
    }

    public static string LogListStr(this IList ieStr, string title = "" ,bool isLog = true)
    {
        int len = ieStr.Count;
        string res = "";
        for (int j = 0; j < len; j++)
        {
            res += ieStr[j].ToString();
            res += res + ",";
        }
        title += " (len = " + len + ")\n";
        string end = string.Format("{0}{1}", title, res);
        if (isLog)
            Debug.Log(end);
        return end;
    }
    
    public static int ToAnimatorHash(this string name)
    {
        return Animator.StringToHash(name);
    }

}

public static class PlayerPrefsTool
{

    public static void SetIntKeyBool(this string key,bool isOn)
    {
        PlayerPrefs.SetInt(key,isOn?1:0);
    }

    public static void SetIntKeyNum(this string key, int num)
    {
        PlayerPrefs.SetInt(key, num);
    }

    public static int GetIntKeyNum(this string key)
    {
        return PlayerPrefs.GetInt(key,0);
    }
    public static string GetKeyStr(this string key)
    {
        return PlayerPrefs.GetString(key, "");
    }
}


public static class PathTool
{
}

public static class FileTool
{
    public static void OpenDir(string path ,bool isAssetPath = false)
    {
#if UNITY_EDITOR
        EditorUtility.RevealInFinder(CheckUpperDir(path));     
#endif
    }
        
    public static string CheckUpperDir(string path)
    {
        if (Directory.Exists(path))
        {
            return path;
        }
        else
        {
            //路径无效查找上一层
            path = path.Substring(0, path.LastIndexOf("/") + 1);
        }
        return path;
    }

    public static void WriteToFile(byte[] by, string filePath ,string checkDir = null)
    {
        if(checkDir!=null &&!Directory.Exists(checkDir))
            Directory.CreateDirectory(checkDir);
        File.WriteAllBytes(filePath, by);
        Debug.LogFormat("WriteToFile {0}",filePath);
    }

    public static void WriteToFile(string str, string filePath)
    {
        using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            sw.Write(str);
            sw.Close();
        }
    }

    public static void WriteToFile(List<string> strList, string filePath)
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
            Debug.LogFormat("yns  no path {0}" , path);
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


public static class TaskAsyncHelper
{
    /// <summary>
    /// 将一个方法function异步运行，在执行完毕时执行回调callback
    /// </summary>
    /// <param name="function">异步方法，该方法没有参数，返回类型必须是void</param>
    /// <param name="callback">异步方法执行完毕时执行的回调方法，该方法没有参数，返回类型必须是void</param>
    public static async void RunAsync(Action function, Action callback)
    {
        Func<Task> taskFunc = () =>
        {
            return Task.Run(() =>
            {
                function();
            });
        };
        await taskFunc();
        if (callback != null)
            callback();
    }

    /// <summary>
    /// 将一个方法function异步运行，在执行完毕时执行回调callback
    /// </summary>
    /// <typeparam name="TResult">异步方法的返回类型</typeparam>
    /// <param name="function">异步方法，该方法没有参数，返回类型必须是TResult</param>
    /// <param name="callback">异步方法执行完毕时执行的回调方法，该方法参数为TResult，返回类型必须是void</param>
    public static async void RunAsync<TResult>(Func<TResult> function, Action<TResult> callback)
    {
        Func<Task<TResult>> taskFunc = () =>
        {
            return Task.Run(() =>
            {
                return function();
            });
        };
        TResult rlt = await taskFunc();
        if (callback != null)
            callback(rlt);
    }
}


public static class EditorStringTool
{
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

    //hasExtendName 是后包括文件扩展名
    //获取路径的文件名
    public static string GetFileNameByPath(this string str, bool hasExtendName = true)
    {
        int last = str.LastIndexOf("/");
        if (last < 0)
        {
            Debug.LogError("yns  no / ");
        }
        str = str.Remove(0, last + 1);
        if (hasExtendName)
            return str;
        else
        {
            int pointIndex = str.LastIndexOf(".");
            if (pointIndex < 0)
            {
                Debug.LogError("yns  no .");
                return str;
            }
            str = str.Substring(0, pointIndex);
            return str;
        }
    }
    //将Asset路径转为ResourcePath
    public static string AssetPathToResPath(this string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("yns path empty!");
            return "";
        }
        string str = path.RemoveHead("Assets/Resources/");
        return str.RemoveEnd(".prefab");
    }
}