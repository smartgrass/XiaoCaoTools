using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XiaoCao;

public class XiaoCaoEexample_0 : XiaoCaoWindow
{
    [MenuItem("Tools/XiaoCao/Eexample_0")]
    static void Open()
    {
        OpenWindow<XiaoCaoEexample_0>("窗口名字");
    }
    public string str;

    [Button("执行事件")]
    private void DoEvent()
    {
        Debug.Log($"yns do Event");
    }
}
