
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 强制输出一个Object
/// </summary>
namespace XiaoCao
{
    public static class LogObjectTool
    {
        public enum EnumTypeType
        {
            Object,
            Value,
            ToJson,
            IList, //Or Array
            IDic
        }

        private static ReflectionTypes types;

        public static ReflectionTypes Types
        {
            get
            {
                if (types == null)
                    types = new ReflectionTypes();
                return types;
            }
        }

        public class ReflectionTypes
        {
            public Type _string = typeof(string);
            public Type ValueType = typeof(ValueType);
            public Type ScriptableObject = typeof(ScriptableObject);
            public Type Enum = typeof(Enum);
            public Type IDictionary = typeof(IDictionary);
            public Type IList = typeof(IList);
        }

        public static EnumTypeType GetTypeType(Type type)
        {
            if (type == Types._string || type.IsSubclassOf(Types.ValueType)
                || type.IsSubclassOf(Types.Enum))
            {
                return EnumTypeType.Value;
            }
            else if (Types.IList.IsAssignableFrom(type) || type.IsArray)
            {
                return EnumTypeType.IList;
            }
            else if (Types.IDictionary.IsAssignableFrom(type))
            {
                return EnumTypeType.IDic;
            }//type.IsSerializable 属性是ToJson不了的
            else if (type.IsSubclassOf(Types.ScriptableObject) || (type.GetCustomAttribute<System.SerializableAttribute>() != null && type.GetFields().Length > 1))
            {
                return EnumTypeType.ToJson;
            }

            return EnumTypeType.Object;
        }

        public static string GetGenArgumentStr(Type type)
        {
            StringBuilder typesName = new StringBuilder(4);
            foreach (var item in type.GetGenericArguments())
            {
                typesName.Append(item.Name).Append(",");
            }
            typesName.Remove(typesName.Length - 1, 1);
            return typesName.ToString();
        }

        public static string GetObjString(object targetObj, Type type)
        {
            var typetype = GetTypeType(type);
            //typetype.ToString().LogStr();
            if (typetype == EnumTypeType.Value)
            {
                return (type.Name + " " + targetObj);
            }
            else if (typetype == EnumTypeType.ToJson)
            {
                // JsonUtility.ToJson(targetObj) 
                return type.Name + "_" + (JsonConvert.SerializeObject(targetObj));
            }
            else if (typetype == EnumTypeType.IList)
            {
                var items = targetObj as IList;
                string title = type.IsGenericType ? $"List<{GetGenArgumentStr(type)}>" : $"{type.GetElementType()}[]";
                return items.LogListStr(title, isLog: false);
            }
            else if (typetype == EnumTypeType.IDic)
            {
                StringBuilder sb = new StringBuilder(4);
                var items = targetObj as IDictionary;

                sb.Append("dic ").Append(GetGenArgumentStr(type)).Append(type.Name).Append("  ").Append(items.Count).Append("\n");

                foreach (var key in items.Keys)
                {
                    sb.Append(key).Append(": ").Append(items[key]).Append("\n");
                }
                return sb.ToString();
            }
            return "";
        }

        public static void LogObjectAll(object targetObj, Type type, int deep = 1, string proName = "")
        {
            bool isStatic = targetObj == null;

            if (!isStatic)
            {
                var typetype = GetTypeType(type);
                if (typetype != EnumTypeType.Object && deep == 1)
                {
                    if (string.IsNullOrEmpty(proName))
                    {
                        proName = "Value";
                    }
                    GetObjString(targetObj, type).LogStr(typetype.ToString() + "_" + proName);
                    return;
                }
            }

            Type ot = typeof(ObsoleteAttribute);
            var pros = isStatic ? type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) : type.GetProperties();

            //pros
            foreach (var pro in pros)
            {
                //if(pro.ReflectedType)
                var attributes = pro.GetCustomAttributes(ot, true);
                bool isOb = attributes.Length > 0;
                if (isOb)
                {
                    continue;
                }

                object curValue = null;
                try
                {
                    curValue = pro.GetValue(targetObj);

                }
                catch (Exception e)
                {
                    Debug.LogWarning(pro.Name + "  error " + e);
                    continue;
                }
                if (curValue == null)
                {
                    Debug.LogWarning(pro.Name + "  curValue null ");
                    continue;
                }

                Type baseType = pro.PropertyType;

                var typetype = GetTypeType(baseType);

                if (baseType.IsGenericType || baseType.IsArray)
                {
                    if (typetype != EnumTypeType.Object)
                        GetObjString(curValue, baseType).LogStr(pro.Name);
                    continue;
                }


                if (curValue != null)
                {
                    if (deep > 0)
                    {
                        if (typetype == EnumTypeType.Object)
                            Debug.Log($">>>deep{deep} {type}.{pro.Name} t:{baseType} value:{curValue}");
                        LogObjectAll(curValue, baseType, deep - 1, pro.Name);
                    }
                }
            }

            //Field
            foreach (var pro in type.GetFields())
            {
                var attributes = pro.GetCustomAttributes(ot, true);
                bool isOb = attributes.Length > 0;
                if (isOb)
                {
                    continue;
                }

                object curValue;

                curValue = pro.GetValue(targetObj);


                if (curValue == null)
                {
                    Debug.LogWarning(pro.Name + "  curValue null ");
                    return;
                }

                Type baseType = pro.FieldType;

                var typetype = GetTypeType(baseType);

                if (baseType.IsGenericType || baseType.IsArray)
                {
                    if (typetype != EnumTypeType.Object)
                        GetObjString(curValue, baseType).LogStr(pro.Name);
                    continue;
                }

                if (curValue != null)
                {
                    if (deep > 0)
                    {
                        if (typetype == EnumTypeType.Object)
                            Debug.Log($">>>deep{deep} {type}.{pro.Name} t:{baseType} value:{curValue}");
                        LogObjectAll(curValue, baseType, deep - 1, pro.Name);
                    }
                }
            }
        }

        public static string LogStr(this string str, string title = "Log")
        {
            if (!string.IsNullOrEmpty(str))
                Debug.LogFormat("{0}: {1}", title, str);
            return str;
        }

        public static string LogListStr(this IList ieStr, string title = "", bool isLog = true)
        {
            int len = ieStr.Count;
            title = $"{title} (len = " + len + ")\n";
            string res = "";
            for (int j = 0; j < len; j++)
            {
                res = $"{res}{ieStr[j].ToString()},";
            }
            string end = $"{title}{res}";
            if (isLog)
                Debug.Log(end);
            return end;
        }
    }

#if UNITY_EDITOR
    public static class LogObjectToolEditorExtend
    {
        [MenuItem("CONTEXT/Object/LogThis")]
        private static void LogThis(MenuCommand menuCommand)
        {
            var obj = menuCommand.context;
            LogObjectTool.LogObjectAll(obj, obj.GetType());
        }
    }
#endif
}