using UnityEngine;
#if UNITY_EDITOR
#endif

public static class PlayerPrefsTool 
{
    public static void SetKeyBool(this string key,bool isOn)
    {
        PlayerPrefs.SetInt(key,isOn.ToInt());
    }
    public static bool GetKeyBool(this string key,bool isOn)
    {
        return PlayerPrefs.GetInt(key).ToBool();
    }

    public static bool ToBool(this int num)
    {
        return num != 0;
    }

    public static int ToInt(this bool value)
    {
        return value ? 1 : 0;
    }
}
