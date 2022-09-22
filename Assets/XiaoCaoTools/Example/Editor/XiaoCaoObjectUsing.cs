using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using XiaoCao;
using ButtonAttribute = XiaoCao.ButtonAttribute;

public class XiaoCaoObjectUsing : XiaoCaoWindow
{
    [MenuItem("Tools/XiaoCao/Eexample_1")]
    static void Open()
    {
        OpenWindow<XiaoCaoEexample_1>("Eexample_1");
    }
    //[Label] [Dropdown] [ShowIf] [Button]
    public ObjectUsing objectUsing;


}
