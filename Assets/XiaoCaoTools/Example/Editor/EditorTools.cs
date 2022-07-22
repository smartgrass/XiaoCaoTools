using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class EditorTools
{
    //[MenuItem("GameObject/Check/SelectChildImage", priority = 11)]
    //public static void SelectChildrenImage()
    //{
    //    if (Selection.activeObject == null)
    //    {
    //        return;
    //    }

    //    Transform parent = (Selection.activeObject as GameObject).transform;
        
    //    var allChild = parent.GetComponentsInChildren<Image>(true);
        
    //    List<GameObject> objList = new List<GameObject>();
    //    foreach (Image child in allChild)
    //    {
    //        objList.Add(child.gameObject);
    //        Debug.Log(child.name);
    //    }
    //    Select(objList.ToArray());
    //}

    //public static void Select(Object[] objs)
    //{
    //    Selection.objects = objs;
        
    //}

}
public class EditorReferenceTools
{
    [MenuItem("Assets/Check/获取预制体引用")]
    static void CheckDependeces()
    {
        List<string> prefabList = new List<string>();
        foreach (var item in Selection.objects)
        {
            if (item is GameObject)
            {
                prefabList.Add(AssetDatabase.GetAssetPath(item));
            }
        }
        GetPngDependeces(prefabList);
    }


    public static List<string> GetSelectsPath(Object[] objects)
    {
        List<string> prefabList = new List<string>();
        foreach (var item in objects)
        {
            prefabList.Add(AssetDatabase.GetAssetPath(item));
        }
        return prefabList;
    }

    public static List<string> GetPngDependeces(List<string> prefabList)
    {
        string[] allDependencies = AssetDatabase.GetDependencies(prefabList.ToArray(), true);
        List<string> allPngDependecies = new List<string>();
        foreach (var item in allDependencies)
        {
            if (item.EndsWith(".png"))
            {
                allPngDependecies.Add(item);
            }
        }
        return allPngDependecies;
    }
    public static List<string> GetAllDependeces(List<string> prefabList)
    {
        string[] allDependencies = AssetDatabase.GetDependencies(prefabList.ToArray(), true);
        List<string> allPngDependecies = new List<string>();
        foreach (var item in allDependencies)
        {
            allPngDependecies.Add(item);      
        }
        return allPngDependecies;
    }
    // "RawResources/Sprite/modelsImg"; 
    //        //string path = Path.Combine(Application.dataPath, myfolderPath);
    public static string[] KnowAllPicture(string dirFullPath)
    {
        List<string> liststring = new List<string>();

        var images = Directory.GetFiles(dirFullPath, ".", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg")); 

        foreach (var i in images)
        {
            var str = i.Replace(Application.dataPath, "");
            var strpath = str.Replace("\\", "/");
            strpath = "Assets" + strpath;
            liststring.Add(strpath);
        }
        return liststring.ToArray();
    }

    //endsWith  ".png"
    public static string[] KnowAllTypeFile(string dirFullPath,string[] endsWith)
    {
        List<string> liststring = new List<string>();

        var images = Directory.GetFiles(dirFullPath, ".", SearchOption.AllDirectories).
        Where(
            (s)=>
            {
                foreach (var item in endsWith)
                {
                    if (s.EndsWith(item))
                        return true;
                }
                return false;
            }
        );

        foreach (var i in images)
        {
            var str = i.Replace(Application.dataPath, "");
            var strpath = str.Replace("\\", "/");
            strpath = "Assets" + strpath;
            liststring.Add(strpath);
        }
        return liststring.ToArray();
    }


    //路径转全局路径
    public static string AssetPathToFullPath(string assestPath)
    {
        string shortPath = assestPath.Remove(0, "Assets/".Length);
        string dirPath = Path.Combine(Application.dataPath, shortPath);
        return dirPath;
    }

    public static void MoveTextureToUnPackage(List<string> moveList, string newPath)
    {
        for (int i = 0; i < moveList.Count; i++)
        {
            MoveTextureToUnPackage(moveList[i], newPath);
        }
    }

    public static void MoveTextureToUnPackage(string oldPath, string newPath)
    {
        string pngName = oldPath.Split('/').Last();

        string targetNewPath = $"{newPath}/{pngName}";

        if (File.Exists(targetNewPath))
        {
            int random = Random.Range(0, 1000);
            targetNewPath = $"{newPath}/Un{random}_{pngName}";
            Debug.Log($"yns  Exits");
        }

        AssetDatabase.MoveAsset(oldPath, targetNewPath);
    }



    [MenuItem("Assets/Check/查找当前使用")]
    public static void FindPngInAll()
    {
        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage == null)
        {
            FindPngInGame();
            return;
        }

        GameObject prefabRoot = prefabStage.prefabContentsRoot;
        if (prefabRoot == null)
            return;

        Debug.Log(prefabRoot);
        List<Object> finds = new List<Object>();

        //var prefab = PrefabStageUtility.GetCurrentPrefabStage();
        var sprites = prefabRoot.GetComponentsInChildren<Image>(true);
        //Debug.Log("yns  prefab " + prefab.name);
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);

        var cur = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        var tex = AssetDatabase.LoadAssetAtPath<Texture>(path);
        bool isFind = false;
        foreach (var item in sprites)
        {
            //Debug.Log("yns  ite" + item.name);
            if (item.sprite == cur)
            {
                finds.Add(item.gameObject);
                isFind = true;
            }
            else if(item.material!=null)
            {
                if(item.material.mainTexture == tex)
                {
                    finds.Add(item.gameObject);
                    isFind = true;
                }
            }
        }

        var rawImages = prefabRoot.GetComponentsInChildren<RawImage>(true);

        foreach (var item in rawImages)
        {
            if (item.material.mainTexture == tex)
            {
                finds.Add(item.gameObject);
                isFind = true;
            }
        }

        if (!isFind)
        {
            Debug.Log($"yns no Find Using");
        }
        Selection.objects = finds.ToArray();
 
    }

    private static void FindPngInGame()
    {
        var Images = GameObject.FindObjectsOfType<Image>(true);
        List<Sprite> spriteList = new List<Sprite>();

        foreach (var item in Selection.objects)
        {
            if (item is Texture2D)
            {
                spriteList.Add(AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(item)));
            }
        }

        List<Object> finds = new List<Object>();

        bool isFind = false;
        foreach (var item in Images)
        {
            if (spriteList.Contains(item.sprite))
            {
                isFind = true;
                Debug.Log("yns  " + item.name);
                finds.Add(item.gameObject);
            }
        }
        if (isFind)
        {
            Debug.Log("yns findCount  " + finds.Count);
            Selection.objects = finds.ToArray();
        }
        else
        {
            Debug.Log("yns no find ");
        }
    }
    [MenuItem("Assets/Check/标记为UnUsing")]
    private static void RenameUnusing()
    {
        var allPath =  GetSelectsPath(Selection.objects);
        foreach (var item in allPath)
        {
            if (!item.Contains("(UnUsing)")&& item.EndsWith(".png"))
            {
                FileInfo info = new FileInfo(AssetPathToFullPath(item));
                info.Name.LogStr();
                AssetDatabase.RenameAsset(item, "(UnUsing)" + info.Name);
            }
        }
    }

}


public class AssetExtend
{

    [MenuItem("Assets/SavaThisAssets")]
    private static void SavaAsset()
    {
        var obj = Selection.objects;
        EditorUtility.SetDirty(obj[0]);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Check/SelectMaxSize>360")]
    private static void SelectMaxSize()
    {
        var obj = Selection.objects;
        int count = 0;
        List<Object> finds = new List<Object>();
        foreach (var item in obj)
        {
            if (item.GetType() == typeof(Texture2D))
            {
                var path = AssetDatabase.GetAssetPath(item);
                try
                {
                    var tx = ((Texture2D)item);
                    int max = Mathf.Max(tx.width, tx.height);

                    if (max > 360)
                    {
                        finds.Add(item);
                        count++;
                    }
                }
                catch
                {
                    Debug.LogError("yns  path " + path);
                }
            }
        }
        Selection.objects = finds.ToArray();
    }


}

#region 右键复制 InspectorExtent
public class InspectorExtent
{
    [MenuItem("CONTEXT/Transform/LogUITFPos")]
    private static void LogUITFPos(MenuCommand menuCommand)
    {
        var tf = menuCommand.context as Transform;
        Debug.Log("yns pos = " + tf.position);
        Debug.Log("Screen w " + Screen.width + " h " + Screen.height);
    }
    // 示例 (MenuCommand menuCommand)
    [MenuItem("CONTEXT/Transform/复制Grid-小")]
    private static void CopyGrid(MenuCommand menuCommand)
    {
        var tf = menuCommand.context as Transform;
        SetContentCopy("grid", tf);
    }
    [MenuItem("CONTEXT/Transform/复制Show-大")]
    private static void CopyShow(MenuCommand menuCommand)
    {
        var tf = menuCommand.context as Transform;
        SetContentCopy("show", tf);
    }

    private static string SetContentCopy(string title, Transform tf)
    {
        string Position = string.Format("\"{0}Position\" : [{1}, {2}, {3}],", title, tf.localPosition.x, tf.localPosition.y, tf.localPosition.z);
        string Rotation = string.Format("\"{0}Rotation\" : [{1}, {2}, {3}],", title, tf.localEulerAngles.x, tf.localEulerAngles.y, tf.localEulerAngles.z);
        string Scale = string.Format("\"{0}Scale\" : [{1}, {2}, {3}],", title, tf.localScale.x, tf.localScale.y, tf.localScale.z);

        string res = string.Format("\t  {0}\n\t  {1}\n\t  {2}", Position, Rotation, Scale);
        CopyStr(res);

        EditorUtility.DisplayDialog("复制", res, "确定");
        return res;
    }

    public static void CopyStr(string value)
    {
        GUIUtility.systemCopyBuffer = value;
    }

}
#endregion

