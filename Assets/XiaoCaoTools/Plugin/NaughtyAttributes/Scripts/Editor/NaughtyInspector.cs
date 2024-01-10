using ET;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class NaughtyInspector : UnityEditor.Editor
    {
        private List<SerializedProperty> _serializedProperties = new List<SerializedProperty>();
        private IEnumerable<FieldInfo> _nonSerializedFields;
        private IEnumerable<PropertyInfo> _nativeProperties;
        //private IEnumerable<MethodInfo> _methods;
        private Dictionary<int, List<MethodInfo>> methodDic;
        private List<int> methPosSortList;

        private Dictionary<string, SavedBool> _foldouts = new Dictionary<string, SavedBool>();
        private Dictionary<Object, SerializedObject> SoDic = new Dictionary<Object, SerializedObject>();

        protected virtual void OnEnable()
        {
            _nonSerializedFields = ReflectionUtility.GetAllFields(
                target, f => f.GetCustomAttributes(typeof(ShowNonSerializedFieldAttribute), true).Length > 0);

            _nativeProperties = ReflectionUtility.GetAllProperties(
                target, p => p.GetCustomAttributes(typeof(ShowNativePropertyAttribute), true).Length > 0);
            GetSortMethods();
        }

        private void GetSortMethods()
        {
            IEnumerable<MethodInfo> allMethods = ReflectionUtility.GetAllMethods(target, m => true);
            methPosSortList = new List<int>();
            methodDic = new Dictionary<int, List<MethodInfo>>();

            foreach (var item in allMethods)
            {
                var ButtonAttributes = item.GetCustomAttributes(typeof(ButtonAttribute), true);
                if (ButtonAttributes.Length > 0)
                {

                    ButtonAttribute att = ButtonAttributes[0] as ButtonAttribute;
                    if (!methodDic.ContainsKey(att.Pos))
                    {
                        //nmae
                        methodDic.Add(att.Pos, new List<MethodInfo>() { item });
                        methPosSortList.Add(att.Pos);
                    }
                    else
                    {
                        methodDic[att.Pos].Add(item);
                    }
                }
            }
            methPosSortList.Sort();
        }


        protected virtual void OnDisable()
        {
            ReorderableListPropertyDrawer.Instance.ClearCache();
        }

        public override void OnInspectorGUI()
        {
            GetSerializedProperties(ref _serializedProperties);

            bool anyNaughtyAttribute = _serializedProperties.Any(p => PropertyUtility.GetAttribute<INaughtyAttribute>(p) != null);
            if (!anyNaughtyAttribute && methPosSortList.Count == 0)
            {
                DrawDefaultInspector();
            }
            else
            {
                DrawSerializedProperties();
            }

            DrawNonSerializedFields();
            DrawNativeProperties();
            DrawButtons();

            ComponentViewHelper.Draw(target);
        }
        protected void DrawButtons(bool drawHeader = true)
        {
            foreach (var item in methPosSortList)
            {
                if (item >= 0)
                {
                    NaughtyEditorGUI.ButtonList(serializedObject.targetObject, methodDic[item], item != 0);
                }
            }
        }

        protected void GetSerializedProperties(ref List<SerializedProperty> outSerializedProperties)
        {
            outSerializedProperties.Clear();
            using (var iterator = serializedObject.GetIterator())
            {
                if (iterator.NextVisible(true))
                {
                    do
                    {
                        outSerializedProperties.Add(serializedObject.FindProperty(iterator.name));
                    }
                    while (iterator.NextVisible(false));
                }
            }
        }


        //弃用
        //public bool CheckDrawSubProperty(SerializedProperty property)
        //{
            //if (property.propertyType == SerializedPropertyType.ObjectReference)
            //{
            //    if (property.objectReferenceValue && PropertyUtility.GetAttribute<SerializeField>(property) != null)
            //    {
            //        var so = GetSO(property.objectReferenceValue);

            //        property.objectReferenceValue = EditorGUILayout.ObjectField(PropertyUtility.GetLabel(property), property.objectReferenceValue, typeof(Object), true);


            //        if (property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, property.displayName))
            //        {
            //            NaughtyEditorGUI.DoDrawDefaultInspector(so);
            //        }
            //        return true;
            //    }
            //}
            //return false;
        //}


        protected  void DrawSerializedProperties()
        {
            serializedObject.Update();
            int i = 1;
            // Draw non-grouped serialized properties
            foreach (var property in GetNonGroupedProperties(_serializedProperties))
            {
                if (property.name.Equals("m_Script", System.StringComparison.Ordinal))
                {
                    using (new EditorGUI.DisabledScope(disabled: true))
                    {
                        if (property.objectReferenceValue == null)
                        {
                            //if (serializedObject.targetObject is ScriptableObject so)
                            //{
                            //    MonoScript ms = MonoScript.FromScriptableObject(so);
                            //    EditorGUILayout.ObjectField("", ms, typeof(Object), true);
                            //}              
                            var namePro = serializedObject.FindProperty("m_Name");
                            EditorGUILayout.ObjectField(namePro.stringValue, targets[0], typeof(Object), true);
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(property);
                        }
                    }
                }
                else
                {
                    NaughtyEditorGUI.PropertyField_Layout(property, true);

                    //按钮
                    if (methodDic.ContainsKey(-i))
                    {
                        NaughtyEditorGUI.ButtonList(serializedObject.targetObject, methodDic[-i]);
                    }
                    i++;
                }
            }

            // Draw grouped serialized properties
            foreach (var group in GetGroupedProperties(_serializedProperties))
            {
                IEnumerable<SerializedProperty> visibleProperties = group.Where(p => PropertyUtility.IsVisible(p));
                if (!visibleProperties.Any())
                {
                    continue;
                }

                NaughtyEditorGUI.BeginBoxGroup_Layout(group.Key);
                foreach (var property in visibleProperties)
                {
                    NaughtyEditorGUI.PropertyField_Layout(property, true);
                }

                NaughtyEditorGUI.EndBoxGroup_Layout();
            }

            // Draw foldout serialized properties
            foreach (var group in GetFoldoutProperties(_serializedProperties))
            {
                IEnumerable<SerializedProperty> visibleProperties = group.Where(p => PropertyUtility.IsVisible(p));
                if (!visibleProperties.Any())
                {
                    continue;
                }

                if (!_foldouts.ContainsKey(group.Key))
                {
                    _foldouts[group.Key] = new SavedBool($"{target.GetInstanceID()}.{group.Key}", false);
                }

                _foldouts[group.Key].Value = EditorGUILayout.Foldout(_foldouts[group.Key].Value, group.Key);
                if (_foldouts[group.Key].Value)
                {
                    foreach (var property in visibleProperties)
                    {
                        NaughtyEditorGUI.PropertyField_Layout(property, true);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawNonSerializedFields(bool drawHeader = false)
        {
            if (_nonSerializedFields.Any())
            {
                if (drawHeader)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Non-Serialized Fields", GetHeaderGUIStyle());
                    NaughtyEditorGUI.HorizontalLine(
                        EditorGUILayout.GetControlRect(false), HorizontalLineAttribute.DefaultHeight, HorizontalLineAttribute.DefaultColor.GetColor());
                }

                foreach (var field in _nonSerializedFields)
                {
                    NaughtyEditorGUI.NonSerializedField_Layout(serializedObject.targetObject, field);
                }
            }
        }

        protected void DrawNativeProperties(bool drawHeader = false)
        {
            if (_nativeProperties.Any())
            {
                if (drawHeader)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Native Properties", GetHeaderGUIStyle());
                    NaughtyEditorGUI.HorizontalLine(
                        EditorGUILayout.GetControlRect(false), HorizontalLineAttribute.DefaultHeight, HorizontalLineAttribute.DefaultColor.GetColor());
                }

                foreach (var property in _nativeProperties)
                {
                    NaughtyEditorGUI.NativeProperty_Layout(serializedObject.targetObject, property);
                }
            }
        }

        private static IEnumerable<SerializedProperty> GetNonGroupedProperties(IEnumerable<SerializedProperty> properties)
        {
            return properties.Where(p => PropertyUtility.GetAttribute<IGroupAttribute>(p) == null);
        }

        private static IEnumerable<IGrouping<string, SerializedProperty>> GetGroupedProperties(IEnumerable<SerializedProperty> properties)
        {
            return properties
                .Where(p => PropertyUtility.GetAttribute<BoxGroupAttribute>(p) != null)
                .GroupBy(p => PropertyUtility.GetAttribute<BoxGroupAttribute>(p).Name);
        }

        private static IEnumerable<IGrouping<string, SerializedProperty>> GetFoldoutProperties(IEnumerable<SerializedProperty> properties)
        {
            return properties
                .Where(p => PropertyUtility.GetAttribute<FoldoutAttribute>(p) != null)
                .GroupBy(p => PropertyUtility.GetAttribute<FoldoutAttribute>(p).Name);
        }

        private static GUIStyle GetHeaderGUIStyle()
        {
            GUIStyle style = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.UpperCenter;

            return style;
        }
    }
}
