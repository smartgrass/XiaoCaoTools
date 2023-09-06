using NaughtyAttributes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

using UnityEngine;
using UnityEngine.UI;
using XiaoCao;



public class XiaoCaoReferenceWindow2 : XiaoCaoWindow
{
    private static XiaoCaoReferenceWindow2 instance;
    public static XiaoCaoReferenceWindow2 Instance
    {
        get
        {
            if (instance == null)
                Open();
            return instance;
        }
    }

    [MenuItem("Tools/XiaoCao/图片引用查询窗口2")]
    static void Open()
    {
        instance = OpenWindow<XiaoCaoReferenceWindow2>("图片引用查询窗口2");
    }

    [Header("选中图片")]
    public List<Object> selects = new List<Object>();
    private List<string> selectPngPaths = new List<string>();

    [Header("预制体文件夹范围")]
    public List<Object> checkFloder = new List<Object>();


    public List<Object> assets = new List<Object>();
    private List<string> findPrefabPaths = new List<string>();

    public int lessTime = 0;
    public List<Object> unUseAsset = new List<Object>();

    private List<string> AllAssetsPaths = new List<string>();
    private List<string> RefPngPaths = new List<string>();

    public Object MoveToDir;

    private void OnSelectionChange()
    {
        bool isChange = false;
        foreach (var item in Selection.objects)
        {
            if (item is Texture)
            {
                if (!isChange)
                {
                    isChange = true;
                    selects.Clear();
                }
                selects.Add(item);
            }
        }
    }

    Dictionary<string, List<string>> prefabUsingPngsDic = new Dictionary<string, List<string>>();

    [Button("1获取所有预制体引用")]
    private void GetAllPrefab()
    {
        List<string> checkDirPaths = new List<string>();
        foreach (var item in checkFloder)
        {
            checkDirPaths.Add(AssetDatabase.GetAssetPath(item));
        }
        AllAssetsPaths.Clear();
        RefPngPaths.Clear();
        string[] guids = AssetDatabase.FindAssets("t:GameObject", checkDirPaths.ToArray());
        List<string> paths = new List<string>();
        new List<string>(guids).ForEach(m => paths.Add(AssetDatabase.GUIDToAssetPath(m)));
        paths.ForEach(p =>
        {
            if (!prefabUsingPngsDic.ContainsKey(p))
            {
                AllAssetsPaths.Add(p);
                var list = new string[] { p };

                prefabUsingPngsDic.Add(p, EditorReferenceTools.GetPngDependeces(new List<string>(list)));
            }
        });
        //获取所有Ref图片路径
        //RefPngPaths = EditorReferenceTools.GetPngDependeces(AllAssetsPaths);
    }

    [Button("2查找图片引用")]
    private void CheckUn()
    {
        if (prefabUsingPngsDic.Count == 0)
        {
            GetAllPrefab();
        }
        prefabUsingPngsDic.Count.ToString().LogStr("Dic Count");


        findPrefabPaths.Clear();
        assets.Clear();
        Dictionary<string, int> HasUsingPngDic = new Dictionary<string, int>();

        //prefabUsingPngsDic["Assets/Resources/Prefabs/pop/SongBoxPop.prefab"].IELogListStr();

        selectPngPaths = EditorReferenceTools.GetSelectsPath(selects.ToArray());

        foreach (var pngPath in selectPngPaths)
        {
            FindOnePngUsingPrefab(HasUsingPngDic, pngPath);
        }

        //return;

        foreach (var item in findPrefabPaths)
        {
            assets.Add(AssetDatabase.LoadAssetAtPath<Object>(item));
        }

        unUseAsset.Clear();

        //findSprite.IELogStr();
        //selectPngPaths.IELogListStr();
        Debug.Log("Assets Count " + assets.Count);

        if (HasUsingPngDic.Count > 0)
            LogToStringTool.LogObjectAll(HasUsingPngDic, HasUsingPngDic.GetType());

        var unFindSprete = selectPngPaths.FindAll(p => !HasUsingPngDic.ContainsKey(p)
        || HasUsingPngDic[p] <= lessTime);



        foreach (var item in unFindSprete)
        {
            unUseAsset.Add(AssetDatabase.LoadAssetAtPath<Object>(item));
        }


        //string[] allDependencies = AssetDatabase.GetDependencies(, true);
        //allDependencies.IELogListStr();
    }

    private void FindOnePngUsingPrefab(Dictionary<string, int> HasUsingPngDic, string pngPath)
    {
        foreach (var item in prefabUsingPngsDic)
        {
            //判断prefab的图片里是否包含图片 pngPath
            //包含则加入预制体礼包
            if (item.Value.Contains(pngPath))
            {
                if (!HasUsingPngDic.ContainsKey(pngPath))
                {
                    HasUsingPngDic.Add(pngPath, 1);
                }
                else
                {
                    HasUsingPngDic[pngPath] = HasUsingPngDic[pngPath] + 1;
                }

                if (!findPrefabPaths.Contains(item.Key))
                {
                    findPrefabPaths.Add(item.Key);
                }
                //continue;
            }

        }
    }

    [Button("无用图转移")]
    private void MoveUnsing()
    {
        string newPath = "Assets/RawResources/UnUsingByToolFInd";
        if (MoveToDir != null)
        {
            newPath = AssetDatabase.GetAssetPath(MoveToDir);
        }

        var list = EditorReferenceTools.GetSelectsPath(unUseAsset.ToArray());
        EditorReferenceTools.MoveTextureToUnPackage(list, newPath);


    }



    [MenuItem("Assets/Check/查找图片引用")]
    public static void FindPngInAll()
    {
        var win = XiaoCaoReferenceWindow2.Instance;
        win.OnSelectionChange();
        win.CheckUn();
    }

    public Sprite newSprite;
    public Color newSpriteColor = Color.white; 
    [Button("替换图片")]
    public void ReplaceOld()
    {
        if (newSprite == null)
        {
            return;
        }
        var list = EditorReferenceTools.GetSelectsPath(assets.ToArray());

        Sprite OldSp = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(selects[0]));
        //string assetPath = list[0]; //ะธฤตฺาปธ๖
        foreach (var assetPath in list)
        {
            Modify(assetPath, OldSp, newSprite);
        }
    }

    [Button("查找当前使用")]
    private void CheckInCurrent()
    {
        Selection.objects = selects.ToArray();
        EditorReferenceTools.FindPngInAll();
    }

    private void Modify(string assetPath, Sprite OldSp, Sprite NewSp)
    {
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
        if (prefabRoot == null)
            return;

        var sprites = prefabRoot.GetComponentsInChildren<Image>(true);
        //Debug.Log("yns  prefab " + prefab.name);

        var cur = OldSp;
        bool isFind = false;
        foreach (var item in sprites)
        {
            if (item.sprite == cur)
            {
                Debug.Log(prefabRoot + " :image = " + item.name);
                item.sprite = NewSp;
                item.color = newSpriteColor;
                isFind = true;
            }
        }
        if (isFind)
        {
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
            PrefabUtility.UnloadPrefabContents(prefabRoot);
            //AssetDatabase.SaveAssets();  	//ฑฃดๆธฤถฏตฤืสิด
        }
    }
}

public class EditorReferenceTools
{

    [MenuItem("Assets/Check/查找Sprite在当前的引用")]
    public static void FindPngInAll()
    {
        SceneManagement.PrefabStage prefabStage = SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
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
            else if (item.material != null)
            {
                if (item.material.mainTexture == tex)
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

    [MenuItem("Assets/Check/查找预制体引用的图片")]
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
    public static string[] KnowAllTypeFile(string dirFullPath, string[] endsWith)
    {
        List<string> liststring = new List<string>();

        var images = Directory.GetFiles(dirFullPath, ".", SearchOption.AllDirectories).
        Where(
            (s) =>
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

    public static void MoveTextureToUnPackage(List<string> moveList, string newPath)
    {
        for (int i = 0; i < moveList.Count; i++)
        {
            MoveTextureToUnPackage(moveList[i], newPath);
        }
    }

    //将资源移动到别处 并标记为unsing
    public static void MoveTextureToUnPackage(string oldPath, string newPath)
    {
        string pngName = oldPath.Split('/').Last();

        string targetNewPath = $"{newPath}/{pngName}";

        if (oldPath == targetNewPath)
        {
            Debug.Log($"yns path same {oldPath}");
            return;
        }

        if (File.Exists(targetNewPath))
        {
            int random = Random.Range(0, 1000);
            targetNewPath = $"{newPath}/Un{random}_{pngName}";
            Debug.Log($"yns  Exits");
        }

        AssetDatabase.MoveAsset(oldPath, targetNewPath);
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

}

#region 弃用
//public class XiaoCaoReferenceWindow : XiaoCaoWindow
//{

//    //[MenuItem("Tools/XiaoCao/图片引用查询窗口")]
//    public static void Open()
//    {
//        OpenWindow<XiaoCaoReferenceWindow>("图片引用查询窗口");
//    }
//    public bool isLockSelect = false;

//    [Header("选中预制体")]
//    public Object[] selects;

//    public List<Object> assets = new List<Object>();


//    public List<Object> unUseAsset = new List<Object>();


//    //public Object CheckPngFolder;

//    private void OnSelectionChange()
//    {
//        if (!isLockSelect)
//            selects = Selection.objects;
//    }

//    private List<string> findPngPaths = new List<string>();


//    [Button("查询")]
//    private void FindPngRef()
//    {
//        var findPngPaths = EditorReferenceTools.GetPngDependeces(EditorReferenceTools.GetSelectsPath(selects));
//        assets.Clear();
//        foreach (var item in findPngPaths)
//        {
//            assets.Add(AssetDatabase.LoadAssetAtPath<Object>(item));
//        }

//    }
//    public Object MoveToDir;

//    [Button("assets图转移")]
//    private void MoveUnsing()
//    {
//        if (MoveToDir == null)
//        {
//            return;
//        }
//        var newPath = AssetDatabase.GetAssetPath(MoveToDir);
//        var list = EditorReferenceTools.GetSelectsPath(assets.ToArray());
//        EditorReferenceTools.MoveTextureToUnPackage(list, newPath);


//    }
//}
#endregion