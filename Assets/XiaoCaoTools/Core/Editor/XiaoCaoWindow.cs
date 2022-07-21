
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using uei = UnityEngine.Internal;

namespace XiaoCao
{

    /// <summary>
    /// 小草做的编辑器窗口  特性只用于编辑器
    /// </summary>
    public class XiaoCaoWindow : EditorWindow
    {
        private Vector2 m_ScrollPosition;
        private string m_CreateButton = "Create";
        private string m_OtherButton = "";


        private MethodInfo updateMethod;
        private IEnumerable<MethodInfo> methods;
        private Dictionary<int, List<ButtonAttribute>> methodDic;
        private List<ButtonAttribute> noPosMethod;  //无顺序方法放最后
        private DrawInfo drawInfo = new DrawInfo();

        

        public static T OpenWindow<T>(string title = "") where T : XiaoCaoWindow
        {
            if (title.IsEmpty())
            {
                title = typeof(T).Name;
            }
            T win = GetWindow<T>(title);
            win.Show();
            return win;
        }

        public virtual void OnEnable()
        {
            //获取按钮方法
            methodDic = new Dictionary<int, List<ButtonAttribute>>();
            noPosMethod = new List<ButtonAttribute>();
            methods = this.GetType()
           .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                BindingFlags.NonPublic).Where(o => Attribute.IsDefined(o, typeof(ButtonAttribute)));
            foreach (var item in methods)
            {
                var att = item.GetCustomAttribute<ButtonAttribute>();
                att.method = item;
                if (string.IsNullOrEmpty(att.name))
                    att.name = item.Name;
                if (att.pos > -10)
                {
                    if (!methodDic.ContainsKey(att.pos))
                    {
                        methodDic.Add(att.pos, new List<ButtonAttribute>() { att });
                    }
                    else
                    {
                        methodDic[att.pos].Add(att);
                    }
                }
                else
                    noPosMethod.Add(att);
            }

            //获取OnValueChange方法
            const BindingFlags kInstanceInvokeFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
            updateMethod = GetType().GetMethod("OnValueChange", kInstanceInvokeFlags);
        }


        private void OnGUI()
        {

            EditorGUIUtility.labelWidth = 150;
            //, "OL Box"
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition,GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar);
            GUIUtility.GetControlID(645789, FocusType.Passive);

            bool modified = DrawInspector();

            EditorGUILayout.EndScrollView();

            //GUILayout.FlexibleSpace();
            if (modified)
            {
             
                this.Repaint();
                InvokeWizardUpdate();
            }
            GUILayout.Space(8);
        }
        internal bool DrawInspector()
        {
            var obj = new SerializedObject(new UnityEngine.Object[] { this }, this);
            EditorGUI.BeginChangeCheck();
            obj.UpdateIfRequiredOrScript();
            SerializedProperty property = obj.GetIterator();
            DrawContent(property);
            obj.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }
        //开始绘制
        private void DrawContent(SerializedProperty property)
        {
            bool isExpend = true; //第一次是整个类, 需要展开子项
            while (property.NextVisible(isExpend))
            {
                if (!isExpend)
                {
                    CheckHorBegin(property);
                    EditorGUILayout.PropertyField(property, true);
                    CheckHorEnd(property);
                    if (!drawInfo.isBeginHor)
                    {
                        DrawButtonPos(drawInfo.index);
                        drawInfo.index++;
                    }

                }
                isExpend = false; //关闭展开子项
            }
            EditorGUILayout.Separator();
            CheckAutoHorEnd();
            drawInfo.index = 0;
            for (int i = -1; i > -10; i--)
            {
                DrawButtonPos(i);
            }

            DrawEndBtn();
        }


        private void CheckAutoHorEnd()
        {
            if (drawInfo.isBeginHor)
                EditorGUILayout.EndHorizontal();
        }
        private void CheckHorEnd(SerializedProperty property)
        {
            var att = GetAttribute<HorLayoutAttribute>(property);
            if (att != null)
            {
                if (!att.isHor)
                {
                    EditorGUILayout.EndHorizontal();
                    drawInfo.isBeginHor = false;
                }
            }
        }
        private void CheckHorBegin(SerializedProperty property)
        {
            var att = GetAttribute<HorLayoutAttribute>(property);
            if (att != null)
            {
                if (att.isHor)
                {
                    EditorGUILayout.BeginHorizontal();
                    drawInfo.isBeginHor = true;
                }
            }
        }
        public static T GetAttribute<T>(SerializedProperty property) where T : class
        {
            T[] attributes = PropertyUtility.GetAttributes<T>(property);
            return (attributes.Length > 0) ? attributes[0] : null;
        }

        void DrawEndBtn()
        {
            foreach (var item in noPosMethod)
            {
                if (GUILayout.Button(item.name))
                {
                    item.method.Invoke(this, null);
                }
            }
        }

        void DrawButtonPos(int index)
        {
            if (methodDic.ContainsKey(index))
            {
                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                foreach (var att in methodDic[index])
                {
                    if (GUILayout.Button(att.name))
                    {
                        att.method.Invoke(this, null);
                    }

                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
            }
        }

        private void InvokeWizardUpdate()
        {
            if (updateMethod != null)
                updateMethod.Invoke(this, null);
        }

        public void UpdateView()
        {
            this.Repaint();
        }
    }


    #region Attributes
    public class HorLayoutAttribute : Attribute
    {
        public bool isHor;
        /// <summary>
        /// 使字段在Inspector中显示自定义的名称。
        /// </summary>
        /// <param name="name">自定义名称</param>
        public HorLayoutAttribute(bool isHor = true)
        {
            this.isHor = isHor;
        }
    }
    public class ButtonAttribute : Attribute
    {
        public string name;

        public int pos;

        //public bool isHor;

        public MethodInfo method;
        /// <summary>
        /// 使字段在Inspector中显示自定义的名称。
        /// </summary>
        /// <param name="name">自定义名称</param>
        public ButtonAttribute(string name = "", int pos = -10)
        {
            this.name = name;
            this.pos = pos;
        }
    }

    public class CustomLabelAttribute : PropertyAttribute
    {
        public string name;

        /// <summary>
        /// 使字段在Inspector中显示自定义的名称。
        /// </summary>
        /// <param name="name">自定义名称</param>
        public CustomLabelAttribute(string name)
        {
            this.name = name;
        }
    }

    [CustomPropertyDrawer(typeof(CustomLabelAttribute))]
    public class CustomLabelDrawer : PropertyDrawer
    {
        private GUIContent _label = null;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_label == null)
            {
                string name = (attribute as CustomLabelAttribute).name;
                _label = new GUIContent(name);
            }

            EditorGUI.PropertyField(position, property, _label);
        }
    }

    #endregion

    public class DrawInfo
    {
        public int index = 0;
        public bool isBeginHor = false;
    }

    #region Utility
    public static class ReflectionUtility
    {
        public static IEnumerable<FieldInfo> GetAllFields(object target, Func<FieldInfo, bool> predicate)
        {
            if (target == null)
            {
                Debug.LogError("The target object is null. Check for missing scripts.");
                yield break;
            }

            List<Type> types = new List<Type>()
            {
                target.GetType()
            };

            while (types.Last().BaseType != null)
            {
                types.Add(types.Last().BaseType);
            }

            for (int i = types.Count - 1; i >= 0; i--)
            {
                IEnumerable<FieldInfo> fieldInfos = types[i]
                    .GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(predicate);

                foreach (var fieldInfo in fieldInfos)
                {
                    yield return fieldInfo;
                }
            }
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(object target, Func<PropertyInfo, bool> predicate)
        {
            if (target == null)
            {
                Debug.LogError("The target object is null. Check for missing scripts.");
                yield break;
            }

            List<Type> types = new List<Type>()
            {
                target.GetType()
            };

            while (types.Last().BaseType != null)
            {
                types.Add(types.Last().BaseType);
            }

            for (int i = types.Count - 1; i >= 0; i--)
            {
                IEnumerable<PropertyInfo> propertyInfos = types[i]
                    .GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(predicate);

                foreach (var propertyInfo in propertyInfos)
                {
                    yield return propertyInfo;
                }
            }
        }

        public static IEnumerable<MethodInfo> GetAllMethods(object target, Func<MethodInfo, bool> predicate)
        {
            if (target == null)
            {
                Debug.LogError("The target object is null. Check for missing scripts.");
                return null;
            }

            IEnumerable<MethodInfo> methodInfos = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(predicate);

            return methodInfos;
        }

        public static FieldInfo GetField(object target, string fieldName)
        {
            return GetAllFields(target, f => f.Name.Equals(fieldName, StringComparison.InvariantCulture)).FirstOrDefault();
        }

        public static PropertyInfo GetProperty(object target, string propertyName)
        {
            return GetAllProperties(target, p => p.Name.Equals(propertyName, StringComparison.InvariantCulture)).FirstOrDefault();
        }

        public static MethodInfo GetMethod(object target, string methodName)
        {
            return GetAllMethods(target, m => m.Name.Equals(methodName, StringComparison.InvariantCulture)).FirstOrDefault();
        }

        public static Type GetListElementType(Type listType)
        {
            if (listType.IsGenericType)
            {
                return listType.GetGenericArguments()[0];
            }
            else
            {
                return listType.GetElementType();
            }
        }
    }

    public static class PropertyUtility
    {
        public static T[] GetAttributes<T>(SerializedProperty property) where T : class
        {
            FieldInfo fieldInfo = ReflectionUtility.GetField(GetTargetObjectWithProperty(property), property.name);
            if (fieldInfo == null)
            {
                return new T[] { };
            }

            return (T[])fieldInfo.GetCustomAttributes(typeof(T), true);
        }
        public static object GetTargetObjectWithProperty(SerializedProperty property)
        {
            string path = property.propertyPath.Replace(".Array.data[", "[");
            object obj = property.serializedObject.targetObject;
            string[] elements = path.Split('.');

            for (int i = 0; i < elements.Length - 1; i++)
            {
                string element = elements[i];
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            return obj;
        }
        private static object GetValue_Imp(object source, string name, int index)
        {
            IEnumerable enumerable = GetValue_Imp(source, name) as IEnumerable;
            if (enumerable == null)
            {
                return null;
            }

            IEnumerator enumerator = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext())
                {
                    return null;
                }
            }

            return enumerator.Current;
        }
        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
            {
                return null;
            }

            Type type = source.GetType();

            while (type != null)
            {
                FieldInfo field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    return field.GetValue(source);
                }

                PropertyInfo property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null)
                {
                    return property.GetValue(source, null);
                }

                type = type.BaseType;
            }

            return null;
        }
    }

    #endregion
}
