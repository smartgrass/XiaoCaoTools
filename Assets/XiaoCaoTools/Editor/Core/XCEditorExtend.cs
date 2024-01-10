using System.IO;
using UnityEditor;
using UnityEngine;

public static class XCEditorExtend
{
    [MenuItem("Assets/Check/输出Type")]
    private static void LogType()
    {
        Selection.activeObject.GetType().Name.LogStr(Selection.activeObject.name);
    }

    [MenuItem("CONTEXT/Object/LogThis")]
    private static void LogThis(MenuCommand menuCommand)
    {
        var obj = menuCommand.context;
        LogObjectTool.LogObjectAll(obj, obj.GetType());
    }
    
    [MenuItem("Assets/Open By Default", false, 2)]
    public static void OpenByVsCode()
    {
        string path = Path.GetFullPath(AssetDatabase.GetAssetPath(Selection.activeObject));
        Debug.Log($"yns {path}");
        EditorUtility.OpenWithDefaultApp(path);
    }

}