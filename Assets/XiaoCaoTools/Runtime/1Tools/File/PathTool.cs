using System.IO;
using XiaoCao;
using UnityEngine;
using Path = System.IO.Path;
#if UNITY_EDITOR
using UnityEditor;
#endif

//目录
///<see cref="MathLayoutTool"/>  矩形排列, 圆形排列
///<see cref="MathTool"/> 旋转角度相关   
///<see cref="StringMatchTool"/> 字符串截取相关   
///<see cref="PathTool"/> 路径处理相关   
///<see cref="FileTool"/>  File IO相关
///<see cref="PlayerPrefsTool"/>  PlayerPrefs
///<see cref="LogObjectTool"/> Log相关


public static class PathTool
{
    public static string GetDirName(string selectedPath)
    {
        DirectoryInfo info = new DirectoryInfo(selectedPath);
        return info.Name;
    }
    /// <summary>
    /// 获取上级级目录
    /// </summary>
    public static string GetUpperDir(string path)
    {
        var upperPath = Directory.GetParent(path)?.FullName;
        return upperPath;
    } 


    public static string FullPathToAssetsPath(string fullPath)
    {
        return Path.Combine("Assets", fullPath.RemoveHead(Application.dataPath));
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
