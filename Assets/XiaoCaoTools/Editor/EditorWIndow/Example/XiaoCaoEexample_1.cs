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
using XiaoCaoEditor;
using Object = UnityEngine.Object;

public class XiaoCaoEexample_1 : XiaoCaoWindow
{
    [MenuItem(XCEditorTools.ExampleWindow_1)]
    static void Open()
    {
        OpenWindow<XiaoCaoEexample_1>("窗口名字");
    }


    //1.========== XCLabel 显示别名 ==========
    [NaughtyAttributes.BoxGroup("1Group")]
    [Label("这是str1")]
    public string str1;


    //2.========== OnValueChanged 值变化监听 ==========
    [OnValueChanged(nameof(OnColorChange))]
    [Range(0, 1)]
    public float value;

    public Color color = Color.red;

    private void OnColorChange()
    {
        Debug.Log($" value = {value}");
        color = Color.blue * value;
    }

    //3.========== Dropdown 下拉列表 ==========
    [Dropdown(nameof(GetBoolValues))]
    public bool isShow = false;

    private DropdownList<bool> GetBoolValues()
    {
        return new DropdownList<bool>()
        {
            { "显示内容",   true},
            { "隐藏内容",  false },
        };
    }

    //下拉列表简化版本
    [Dropdown(nameof(dirList))]
    public string select = "1";
    string[] dirList = new[] { "1", "2", "3" };

    //4.========== ShowIf 隐藏/显示 字段,按钮 ==========
    [ShowIf(nameof(isShow))]
    public List<Object> assets;


    //5.========== Button 显示按钮 ==========
    //[Button( 按钮名字 , 按钮位置)]
    // -6 表示放插入字段的位置的第六个位置
    [ShowIf(nameof(isShow))]
    [Button("OnShowBtn", -6)]
    private void OnShowBtn()
    {
        Debug.Log($" Button6 ");
    }

    // 如果数字相同,则按钮挤在同一行
    [Button("ButtonA-1", -1)]
    private void Fun4()
    {
        Debug.Log($" ButtonA-1");
    }
    [Button("ButtonB-1", -1)]
    private void fun3()
    {
        Debug.Log($" ButtonB-1");
    }

    //6.========== 利用UnityEvent 动态注册按钮事件 ==========
    public UnityEvent unityEvent;

    [Button("执行事件")]
    private void DoUnityEvent()
    {
        Debug.Log($" do Event");
        unityEvent?.Invoke();
    }

    //7.========== ScriptableObject 查看 ==========
    //加上[SerializeField]标记为可展开对象
    [SerializeField]
    public Object findObject;

}
