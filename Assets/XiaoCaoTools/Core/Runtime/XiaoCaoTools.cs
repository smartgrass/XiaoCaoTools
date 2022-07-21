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

public static class StringTool{

    #region 扩展方法
    public static string LogStr(this string str,string title = "Log")
    {
        if(!str.IsEmpty())
            Debug.LogFormat("{0}: {1}",title ,str);
        return str;
    }

    public static string IELogListStr(this IList IEStrs, string title = "" ,bool isLog = true)
    {
        string res = "";
        int i = 0;
        foreach (var item in IEStrs)
        {
            res += item.ToString() + ",";
            i++;
            if (i % 10 == 0)
            {
                res += "\n";
            }
        }
        title += " (len = " + i + ")\n";
        string end = string.Format("{0}{1}", title, res);
        if (isLog)
            Debug.Log(end);
        return end;
    }

    public static void IELogStr(this IEnumerable IEStrs, string title = "Log")
    {
        if (IEStrs == null)
        {
            Debug.Log($"yns IELogStr null");
            return;
        }
        string res = "";
        int i = 0;
        foreach (IEnumerable item in IEStrs)
        {
            if (item != null)
            {
                res += item.ToString() + "\n";
            }
            else
            {
                res += "null\n";
            }
            i++;
        }
        title += ": " + i + "\n";
        Debug.LogFormat("{0}{1}", title, res);
    }


    public static bool IsEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    #endregion

    public enum EnumTypeType
    {
        Object,
        Value,
        ToJson,
        IList, //Or Array
        IDic
    }

    private static ReflectionTypes types;

    public static ReflectionTypes Types
    {
        get
        {
            if (types == null)
                types = new ReflectionTypes();
            return types;
        }
    }

    public class ReflectionTypes
    {
       public Type _string = typeof(string);
       public Type ValueType = typeof(ValueType);
       public Type ScriptableObject = typeof(ScriptableObject);
       public Type Enum = typeof(Enum);
       public Type IDictionary = typeof(IDictionary);
       public Type IList = typeof(IList);
    }

    public static EnumTypeType GetTypeType(Type type)
    {
        if(type == Types._string || type.IsSubclassOf(Types.ValueType) 
            || type.IsSubclassOf(Types.Enum))
        {
            return EnumTypeType.Value;
        }
        else if (Types.IList.IsAssignableFrom(type)|| type.IsArray)
        {
            return EnumTypeType.IList;
        }
        else if (Types.IDictionary.IsAssignableFrom(type))
        {
            return EnumTypeType.IDic;
        }
        else if (type.IsSubclassOf(Types.ScriptableObject) || type.IsSerializable)
        {
            return EnumTypeType.ToJson;
        }


        return EnumTypeType.Object;
    }

    public static string GetGenArgumentStr(Type type)
    {
        StringBuilder typesName = new StringBuilder(4);
        foreach (var item in type.GetGenericArguments())
        {
            typesName.Append(item.Name).Append(",");
        }
        typesName.Remove(typesName.Length - 1, 1);
        return typesName.ToString();
    }

    public static string GetObjString(object targetObj, Type type)
    {
        var typetype = GetTypeType(type);
        //typetype.ToString().LogStr();
        if (typetype == EnumTypeType.Value)
        {
            return (type.Name + " " + targetObj);
        }
        else if (typetype == EnumTypeType.ToJson )
        {
            return type.Name +"_"+(JsonUtility.ToJson(targetObj));
        }
        else if (typetype == EnumTypeType.IList)
        {
            var items = targetObj as IList;
            string title =type.IsGenericType ? $"List<{GetGenArgumentStr(type)}>" : $"{type.GetElementType()}[]";
            return  items.IELogListStr(title, isLog: false);
        }
        else if(typetype == EnumTypeType.IDic)
        {
            StringBuilder sb = new StringBuilder(4);
            var items = targetObj as IDictionary;

            sb.Append("dic ").Append( GetGenArgumentStr(type)).Append(type.Name).Append("  ").Append(items.Count).Append("\n");

            foreach (var key in items.Keys)
            {
                sb.Append(key).Append(": ").Append(items[key]).Append("\n");
            }
            return sb.ToString();
        }
        return "";
    }

    public static void LogObjectAll(object targetObj, Type type,int deep = 1,string proName ="")
    {
        bool isStatic = targetObj == null;

        if (!isStatic)
        {
            var typetype = GetTypeType(type);
            //typetype.ToString().LogStr(type.Name);
            if(typetype != EnumTypeType.Object)
            {
                if (proName.IsEmpty())
                {
                    proName = "Value";
                }
                GetObjString(targetObj, type).LogStr(proName );
                return;
            }
        }

        Type ot = typeof(ObsoleteAttribute);
        var pros = isStatic ? type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) : type.GetProperties();

        //pros
        foreach (var pro in pros)
        {
            //if(pro.ReflectedType)
            var attributes = pro.GetCustomAttributes(ot, true);
            bool isOb = attributes.Length > 0;
            if (isOb)
            {
                continue;
            }

            object curValue = null;
            try
            {
                curValue = pro.GetValue(targetObj);

            }
            catch(Exception e)
            {
                Debug.LogWarning(pro.Name+ "  error "+ e);
                continue;
            }
            if (curValue == null)
            {
                Debug.LogWarning(pro.Name + "  curValue null ");
                continue;
            }

            Type baseType = pro.PropertyType;

            var typetype = GetTypeType(baseType);

            if (baseType.IsGenericType || baseType.IsArray)
            {
                if (typetype != EnumTypeType.Object)
                    GetObjString(curValue, baseType).LogStr(pro.Name);
                continue;
            }


            if (curValue != null)
            {
                if (deep > 0)
                {
                    if (typetype == EnumTypeType.Object)
                        Debug.Log($">>>deep{deep} {type}.{pro.Name} t:{baseType} value:{curValue}");
                    LogObjectAll(curValue, baseType, deep - 1, pro.Name);
                }
            }
        }

        //Field
        foreach (var pro in type.GetFields())
        {
            var attributes = pro.GetCustomAttributes(ot, true);
            bool isOb = attributes.Length > 0;
            if (isOb)
            {
                continue;
            }

            var curValue = pro.GetValue(targetObj);

            if(curValue == null)
            {
                Debug.LogWarning(pro.Name + "  curValue null ");
                return;
            }

            Type baseType = pro.FieldType;

            var typetype = GetTypeType(baseType);

            if (baseType.IsGenericType || baseType.IsArray)
            {
                if (typetype != EnumTypeType.Object)
                    GetObjString(curValue, baseType).LogStr(pro.Name);
                    continue;
            }

            if (curValue != null)
            {         
                if(deep > 0)
                {
                    if (typetype == EnumTypeType.Object)
                        Debug.Log($">>>deep{deep} {type}.{pro.Name} t:{baseType} value:{curValue}");
                    LogObjectAll(curValue, baseType, deep - 1,pro.Name);
                }
            }
        }
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
    public static string DownloadUrlText(string url,string localfilePath)
    {
        string str = ReadFileWebUrl(url);
        WriteToFile(str, localfilePath);
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
