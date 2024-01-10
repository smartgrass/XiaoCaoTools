using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 获取全局节点
/// </summary>
public class DontDestroyTransfrom
{
    private static readonly Dictionary<string, Transform> dic = new Dictionary<string, Transform>();
    static public Transform Get(string name)
    {
        if (dic.ContainsKey(name))
        {
            return dic[name];
        }

        GameObject go = new GameObject(name);
        Transform ret = go.transform;
        dic[name] = ret;
        Object.DontDestroyOnLoad(go);
        return ret;
    }
}