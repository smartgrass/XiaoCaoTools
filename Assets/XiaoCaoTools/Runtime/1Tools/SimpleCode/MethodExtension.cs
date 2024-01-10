using System.Text.RegularExpressions;
using UnityEngine;

public static class MethodExtension
{
    static public T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T ret = go.GetComponent<T>();
        if (null == ret)
            ret = go.AddComponent<T>();
        return ret;
    }

}
