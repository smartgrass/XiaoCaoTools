using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Events;
using XiaoCao;
using static UnityEditor.SearchableEditorWindow;
using Object = UnityEngine.Object;

public class XiaoCaoEexample_Pro : XiaoCaoWindow
{
    public List<Object> assets;

    public string search;

    [MenuItem("Tools/XiaoCao/Eexample_Pro")]
    static void Open()
    {
        OpenWindow<XiaoCaoEexample_Pro>();
    }

    [Button]
    private void FindWindow()
    {
        string[] nameList = {
            "UnityEditor.InspectorWindow",
            "UnityEditor.ProjectBrowser",
            "UnityEditor.ConsoleWindow",
            "UnityEditor.SceneHierarchyWindow",
            "UnityEditor.GameView",
            "UnityEditor.SceneView"
        };
        var editorAssembly = typeof(EditorGUIUtility).Assembly;
        assets = new List<Object>();
        foreach (var item in nameList)
        {
            Type winType = editorAssembly.GetType(item);
            assets.Add(EditorWindow.GetWindow(winType));
        }
    }
    [Button("查看assets[0]的所有方法")]
    private void fun1()
    {
        BindingFlags bindingFlag = (System.Reflection.BindingFlags.SuppressChangeType - 1)| System.Reflection.BindingFlags.SuppressChangeType;
        var pros = assets[0].GetType().GetMethods(bindingFlag);
        foreach (var item in pros)
        {
            Debug.Log($"{item} {GetGetParametersStr(item.GetParameters())}");
        }
    }

    private string GetGetParametersStr(ParameterInfo[] pars)
    {
        string res = "";
        if(pars.Length > 1)
        {
            res += "len = " + pars.Length + "\n";
        }
        else
        {
            res += "\n";
        }

        foreach (var item in pars)
        {
            res += item.ToString() + ",";
        }
        return res;
    }


    [Button("测试查询")]
    private void SetSearchFilter()
    {
        //Void SetSearchFilter(System.String, SearchMode, Boolean, Boolean)
        BindingFlags bindingFlag = (System.Reflection.BindingFlags.SuppressChangeType - 1) | System.Reflection.BindingFlags.SuppressChangeType;

        var searchwin = assets.Find(a => a.GetType().ToString() == "UnityEditor.SceneHierarchyWindow");

        var method = searchwin.GetType().GetMethod("SetSearchFilter", bindingFlag);

        method.Invoke(assets[0], new object[]{ search, SearchMode.All, true, true});
    }
}
