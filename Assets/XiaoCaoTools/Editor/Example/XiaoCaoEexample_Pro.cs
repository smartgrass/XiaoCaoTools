using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using XiaoCao;
using static UnityEditor.SearchableEditorWindow;
using Object = UnityEngine.Object;

public class XiaoCaoEexample_Pro : XiaoCaoWindow
{
    public List<Object> assets;
    public string search;

    [MenuItem("Tools/XiaoCao/反射查看Unity窗口")]
    static void Open()
    {
        OpenWindow<XiaoCaoEexample_Pro>();
    }

    [Button("获取Unity基本窗口")]
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

    [Button("获取当前所有窗口")]
    private void FindAllWindow()
    {
        var wins = Resources.FindObjectsOfTypeAll<EditorWindow>();
        assets = new List<Object>();
        foreach (var win in wins)
        {
            assets.Add(win);
        }
    }

    [Button("查看assets[0]的所有方法")]
    private void GetMethods()
    {
        BindingFlags bindingFlag = ((BindingFlags)(-1));
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


    [Button("查询Hierarchy")]
    private void SetSearchFilter()
    {
        //Void SetSearchFilter(System.String, SearchMode, Boolean, Boolean)
        FindWindow();

        BindingFlags bindingFlag = ((BindingFlags)(-1));

        var searchwin = assets.Find(a => "UnityEditor.SceneHierarchyWindow".EndsWith(a.GetType().Name));

        var method = searchwin.GetType().GetMethod("SetSearchFilter", bindingFlag);

        method.Invoke(searchwin, new object[]{ search, SearchMode.All, true, true});
    }

}
