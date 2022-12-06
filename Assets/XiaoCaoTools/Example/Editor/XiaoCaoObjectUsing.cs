using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using XiaoCao;

public class XiaoCaoObjectUsing : XiaoCaoWindow
{
    [Expandable()]
    public ObjectUsing objectUsing;

    [Expandable()]
    public ScriptableObject so;

    private static XiaoCaoObjectUsing instance;
    public static XiaoCaoObjectUsing Instance
    {
        get
        {
            if (instance == null)
                Open();
            return instance;
        }
    }

    [MenuItem("Tools/XiaoCao/对象收藏夹")]
    static void Open()
    {
        instance= OpenWindow<XiaoCaoObjectUsing>("对象收藏夹");
    }


    public override void OnEnable()
    {
        base.OnEnable();

        string path = "Assets/Editor/Ignore/XCObjectUsing.asset";

        objectUsing = XCEditorTool.GetOrNewSO(path);
    }
}


public static class XiaoCaoObjectUsingExtend
{
    [MenuItem("Assets/Xiaocao/添加到收藏")]
    private static void AddToFavor()
    {
        var objs = Selection.objects;
        var win = XiaoCaoObjectUsing.Instance;

        foreach (var item in objs)
        {
            win.objectUsing.ObjectList.Add(item);
            Debug.Log($"yns add {item.name}");
        }
    }
}

