using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ET
{
    //示例代码 设置CustomEditor中需要绘制的类即可
    //[CustomEditor(typeof(MonoBehaviour), true)]
    public class MonoBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ComponentViewHelper.Draw(target);
        }
    }


    public static class ComponentViewHelper
    {
        private static readonly Dictionary<Type, ITypeDrawer> typeDrawers = new Dictionary<Type, ITypeDrawer>();
        private static readonly List<IOtherTypeDrawer> otherDrawers = new List<IOtherTypeDrawer>();
        private static readonly UnityObjectTypeDrawer objectDrawer = new UnityObjectTypeDrawer();

        private static Type IDictionaryType = typeof(IDictionary);
        private static Type IListType = typeof(IList);

        private static int maxlayer = 20;
        private static int curLayer = 0;
        private static bool isDebug;

        private static Dictionary<string, SavedBool> _foldouts = new Dictionary<string, SavedBool>();

        static ComponentViewHelper()
        {
            Assembly assembly = typeof(ComponentViewHelper).Assembly;
            Type drawType = typeof(ITypeDrawer);
            foreach (Type type in assembly.GetTypes())
            {
                if (!type.IsDefined(typeof(TypeDrawerAttribute)))
                {
                    continue;
                }
                if (drawType.IsAssignableFrom(type))
                {
                    ITypeDrawer typeDrawer = (ITypeDrawer)Activator.CreateInstance(type);
                    typeDrawers.Add(typeDrawer.HandlesType(), typeDrawer);
                }
                else
                {
                    IOtherTypeDrawer otherTypeDrawer = (IOtherTypeDrawer)Activator.CreateInstance(type);
                    otherDrawers.Add(otherTypeDrawer);
                }
            }



            curLayer = 0;
        }

        public static void Draw(object entity)
        {
            if (entity == null)
                return;
            try
            {
                FieldInfo[] fields = entity.GetType()
                        .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                if (curLayer == 0)
                {
                    isDebug = EditorGUILayout.Toggle("===Editor View===", (bool)isDebug);
                    if (!isDebug)
                    {
                        return;
                    }
                }

                EditorGUILayout.BeginVertical();

                foreach (FieldInfo fieldInfo in fields)
                {
                    Type type = fieldInfo.FieldType;

                    //if (fieldInfo.IsDefined(typeof (HideInInspector), false))
                    if (type.IsDefined(typeof(HideInInspector), false))
                    {
                        continue;
                    }

                    string fieldName = fieldInfo.Name;
                    object value = fieldInfo.GetValue(entity);
                    bool isDrawed = false;
                    //值类型绘制
                    if (typeDrawers.ContainsKey(type))
                    {
                        try
                        {
                            value = typeDrawers[type].DrawAndGetNewValue(type, fieldName, value, null);
                            isDrawed = true;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                        }
                        fieldInfo.SetValue(entity, value);
                        continue;
                    }


                    bool isArray = type.IsArray;
                    //派生类绘制, 如枚举
                    foreach (var item in otherDrawers)
                    {
                        if (item.HandlesType(type))
                        {
                            try
                            {
                                value = item.DrawAndGetNewValue(type, fieldName, value, null);
                                isDrawed = true;
                            }
                            catch (Exception e)
                            {
                                Debug.LogErrorFormat("{0}: {1}", type, e);
                            }
                            fieldInfo.SetValue(entity, value);
                            break;
                        }
                    }

                    //可展开类型绘制
                    if (!isDrawed && curLayer < maxlayer)
                    {
                        //绘制Object类型
                        if (CheckAndDrawObject(type, value, fieldInfo.Name))
                        {
                            continue;
                        }

                        bool isCollection = IsICollectionType(type, out Type elementType);
                        //绘制数组类型
                        if (isCollection)
                        {
                            if (value == null)
                            {
                                GUILayout.Label($"{fieldInfo.Name} null", EditorStyles.boldLabel);
                                continue;
                            }

                            if (value is ICollection listValue)
                            {
                                int count = listValue.Count;
                                if (IsFoldoutKey(type, fieldInfo.Name, count))
                                {
                                    bool isObjectType = objectDrawer.IsObjectType(elementType);
                                    bool isDicType = IDictionaryType.IsAssignableFrom(type);
                                    curLayer++;
                                    EditorGUI.indentLevel++;

                                    //展开数组类
                                    int i = 0;
                                    foreach (var sub in listValue)
                                    {
                                        if (isObjectType)
                                        {
                                            DrawObject(elementType, sub, i.ToString());
                                        }
                                        else
                                        {
                                            Draw(sub);
                                        }
                                        i++;
                                    }

                                    EditorGUI.indentLevel--;
                                    curLayer--;
                                }
                                continue;
                            }
                        }

                        //绘制普通类
                        if (IsFoldoutKey(type, fieldInfo.Name))
                        {
                            DrawChildren(value);
                        }
                    }
                }

                EditorGUILayout.EndVertical();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log($"component view error: {entity.GetType().FullName} {e}");
            }
        }

        private static bool CheckAndDrawObject(Type type, object value, string showName)
        {
            bool isObject = objectDrawer.IsObjectType(type);
            if (isObject)
            {
                DrawObject(type, value, showName);
            }
            return isObject;
        }

        private static void DrawObject(Type type, object value, string showName)
        {
            bool isNotNull = value != null;
            if (isNotNull)
            {
                type = value.GetType();
            }
            objectDrawer.DrawObject(type, showName, value);
            if (isNotNull && objectDrawer.IsDrawChildren(type))
            {
                if (IsFoldoutKey(type, showName))
                {
                    DrawChildren(value);
                }
            }
        }
        //是否展开
        private static bool IsFoldoutKey(Type type, string fieldInfoName, int chirldCount = -1)
        {
            string key = $"{type.Name}.{fieldInfoName}.{curLayer}";
            string label = fieldInfoName;
            if (chirldCount >= 0)
            {
                label = $"{fieldInfoName} (len = {chirldCount})";
            }
            if (!_foldouts.ContainsKey(key))
            {
                _foldouts[key] = new SavedBool(key, false);
            }
            _foldouts[key].Value = EditorGUILayout.Foldout(_foldouts[key].Value, label);

            return _foldouts[key].Value;
        }

        private static bool IsICollectionType(Type type, out Type elementType)
        {

            if (type.IsArray)
            {
                elementType = type.GetElementType();
                return true;
            }
            else if (type.IsGenericType && IListType.IsAssignableFrom(type))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
            else if (type.IsGenericType && IDictionaryType.IsAssignableFrom(type))
            {
                elementType = type;//.GetGenericArguments()[1];
                return true;
            }
            else
            {
                elementType = type;
                return false;
            }
        }

        private static void DrawChildren(object value)
        {
            curLayer++;
            EditorGUI.indentLevel++;
            Draw(value);
            EditorGUI.indentLevel--;
            curLayer--;
        }

    }

    internal class SavedBool
    {
        private bool _value;
        private string _name;

        public bool Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value == value)
                {
                    return;
                }

                _value = value;
                EditorPrefs.SetBool(_name, value);
            }
        }

        public SavedBool(string name, bool value)
        {
            _name = name;
            _value = EditorPrefs.GetBool(name, value);
        }
    }


    public class UnityObjectTypeDrawer
    {
        private Type objectType = typeof(UnityEngine.Object);
        public bool IsObjectType(Type type)
        {
            return type.IsSubclassOf(objectType) || type == objectType;
        }

        internal bool IsDrawChildren(Type type)
        {
            return type.IsSubclassOf(typeof(UnityEngine.Component));
        }

        public object DrawObject(Type memberType, string memberName, object value)
        {
            return EditorGUILayout.ObjectField(memberName, (UnityEngine.Object)value, memberType, true);
        }


    }
}