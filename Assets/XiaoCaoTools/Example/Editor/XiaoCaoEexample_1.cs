using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XiaoCao;

public class XiaoCaoEexample_1 : XiaoCaoWindow
{

    [MenuItem("Tools/XiaoCao/Eexample_1")]
    static void Open()
    {
        OpenWindow<XiaoCaoEexample_1>("Eexample_1");
    }

    public Object[] assets;

    public string path = "Assets/RawResources/Sprite";

    [NaughtyAttributes.Dropdown("dirList")]
    public string dirName="";

    string[] dirList = {"1","2" };


    [Button("Fun1", 1)]
    private void Fun1()
    {
        Debug.Log($"yns {dirName}");
    }


}
