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

public class XC_ReadMe
{
    ///主要功能：<see cref="XiaoCaoWindow"/>
    ///作为窗口基类，用于快速搭建编辑器窗口
    ///见<see cref="XiaoCaoEexample_1">，顶部菜单打开：Tools/XiaoCao/Eexample_1
    ///主要特性[XCLabel] [Button] [OnValueChanged]  (From开源项目NaughtyAttributes)

    ///XiaoCaoWindow使用
    ///
    ///1.Unity对象收藏夹 
    ///from <see cref="XiaoCaoObjectUsing"/>
    ///打开位置: Tools/XiaoCao/对象收藏夹
    ///
    ///2.反射查看Unity窗口
    ///from <see cref="XiaoCaoEexample_Pro"/>
    ///打开位置: Tools/XiaoCao/反射查看Unity窗口
    ///
    ///3.Json编辑窗口
    ///from <see cref="XiaoCaoJsonWin"/>
    ///打开位置: Tools/XiaoCao/Json编辑窗口
    ///
    ///4.Mono类预制体查找
    ///from <see cref="XiaoCaoFindMonoPrefab"/>
    ///打开位置: Tools/XiaoCao/Mono类预制体查找


    #region Log工具

    ///几个Log工具，可用于Debug
    ///
    ///1.string扩展方法 <see cref="StringTool.LogStr"/> 
    ///使用：str.LogStr() 相当于 Debug.Log(str);
    ///
    ///2.数组Log <see cref="StringTool.ListToStr"/> 
    ///使用：list.IELogListStr() ，就不用自己写一遍遍历了
    ///
    ///3.任意类Log <see cref="LogToStringTool.LogObjectAll"/> 
    ///使用反射，详细输出对象的所有字段和属性。
    ///扩展：组件右键->LogThis,可以Log组件Inspector看不到的信息 <see cref="EditorAssetExtend.LogThis"/> 
    ///其他：对于Unity一些隐藏类，也可以利用反射看到其中字段的值 
    ///可以试试看<see cref="XC_ReadMe.LogType"/> 
    public static void LogType()
    {
        Type type = typeof(Type);
        LogToStringTool.LogObjectAll(type, type);
    }

    ///4.查询Asset类型 <see cref="XCEditorExtend.LogType()"/> 
    ///选中一个aseet右键->Check->输出Type

    #endregion

    #region 其他功能

    //1.Text转TMP
    ///from <see cref="XCEditorTool_TMP.OnTextToTmp"/>
    //使用方法:选中预制体->右键Check/TextToTmp
    //说明:将Text转为TextMeshProUGUI,并保持原大小位置

    //2.替换子物体TMP字体
    ///from <see cref="XCEditorTool_TMP.CheckEnTMPFont"/>
    //使用方法:选中物体->右键XiaoCao/替换子物体TMP字体

    ///from <see cref="XiaoCaoReferenceWindow2"/>
    ///from <see cref="EditorReferenceTools.FindPngInAll"/>
    ///from <see cref="EditorReferenceTools.CheckDependeces"/>
    //3.询图片引用 XiaoCaoReferenceWindow2

    #endregion
}