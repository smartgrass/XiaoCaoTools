#if UNITY_EDITOR
//using AssetEditor.Editor.Window;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
[CreateAssetMenu(menuName = "SO/AssetsUsing")]
public  class AssetsUsingEditor :ScriptableObject
{
    public List<ObjectList> objectLists;  
}
[Serializable]
public class ObjectList
{
    public string name;
    public List<Object> list;
}
#endif