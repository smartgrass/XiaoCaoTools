using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using XiaoCao;


public class XiaoCaoEexample_Pro : XiaoCaoWindow
{
    [CustomLabel("名字")]
    public string s = "aaa";
    public int i = 500;
    
    [CustomLabel("颜色")]
    public Color color = Color.red;

    [HorLayout(true)]
    public bool isAndroid = true;
    [HorLayout(false)]
    public bool isIOS = true;




    [MenuItem("Tools/XiaoCao/Eexample_Pro")]
    static void Open()
    {
        OpenWindow<XiaoCaoEexample_Pro>();
    }

    void OnValueChange()
    {
        Debug.Log("yns  OnValueChange");
    }

    [Button]
    private void Fun1()
    {
        Debug.Log("yns  button0");
    }
    [Button("Button2", 2)]
    private void Fun2()
    {
        Debug.Log("yns  button1");
    }

    //数字表示按钮插入的位置, 最小是-10
    [Button("Button3", 2)]
    private void Fun3()
    {
        Debug.Log("yns  button2");
    }

    [Button("Button4", -1)]
    private void Fun4()
    {
        Debug.Log("yns  button3");
    }

    [Button("Button5", -1)]
    private void fun5()
    {
        Debug.Log("yns  button4");
    }
}
