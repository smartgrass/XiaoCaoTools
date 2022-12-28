using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using XiaoCao;
using Object = UnityEngine.Object;

public class XiaoCaoEexample_0 : XiaoCaoWindow
{
    [MenuItem("Tools/XiaoCao/Eexample_0")]
    static void Open()
    {
        OpenWindow<XiaoCaoEexample_0>("窗口名字");
    }

    [Button("执行事件")]
    private void DoEvent()
    {
        Debug.Log($"yns do Event");
    }

}
