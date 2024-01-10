using NaughtyAttributes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XiaoCao;
using XiaoCaoEditor;
/// <summary>
/// 收藏夹 窗口
/// </summary>
public class ObjectsWindow : XiaoCaoWindow
{
    [Expandable()]
    public EditorObjects _objects;

    public override Object DrawTarget => _objects;

    private static ObjectsWindow instance;
    public static ObjectsWindow Instance
    {
        get
        {
            if (instance == null)
                Open();
            return instance;
        }
    }
    public override void OnEnable()
    {
        string path = "Assets/Ignore/Editor/XCObjectUsing.asset";
        _objects = XCAseetTool.GetOrNewSO<EditorObjects>(path);
        base.OnEnable();
    }

    [MenuItem(XCEditorTools.ObjectsWindow)]
    static void Open()
    {
        instance = OpenWindow<ObjectsWindow>("对象收藏夹");
    }



}


public static class XiaoCaoObjectUsingExtend
{
    [MenuItem("Assets/XiaoCao/添加到收藏", priority = 100)]
    private static void AddToFavor()
    {
        var objs = Selection.objects;
        var win = ObjectsWindow.Instance;
        if (win._objects.ObjectList == null)
        {
            win._objects.ObjectList = new List<Object>();
        }

        foreach (var item in objs)
        {
            win._objects.ObjectList.Add(item);
            Debug.Log($" add {item.name}");
        }
    }
}
