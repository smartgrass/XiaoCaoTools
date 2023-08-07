using System.Collections.Generic;
using UnityEngine;

namespace XiaoCao
{
    [CreateAssetMenu(menuName = "JsonObjectUsing")]
    public class JsonObjectUsing : ScriptableObject
    {
        public List<JsonUsing> UsingList;
    }
	
	[System.Serializable]
    public class JsonUsing
    {
        public ConfigType configType;
        public TextAsset textAsset;
        
    }

    public enum ConfigType
    {
        ConfigB,
        ConfigA,
    }
}