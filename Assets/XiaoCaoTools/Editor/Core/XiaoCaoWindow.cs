using NaughtyAttributes.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XiaoCao
{

    /// <summary>
    /// 小草做的编辑器窗口  特性只用于编辑器
    /// </summary>
    public class XiaoCaoWindow : EditorWindow
    {
        private Vector2 m_ScrollPosition;

        private Editor editor = null;

        public virtual Object DrawTarget { get => this; }

        public virtual void OnEnable()
        {
            editor = Editor.CreateEditor(DrawTarget);
        }

        public static T OpenWindow<T>(string title = "") where T : XiaoCaoWindow
        {
            if (string.IsNullOrEmpty(title))
            {
                title = typeof(T).Name;
            }
            T win = GetWindow<T>(title);
            win.Show();
            return win;
        }


        ///<see cref="NaughtyInspector.OnInspectorGUI"/>
        private void OnGUI()
        {
            if (EditorApplication.isCompiling)
                return;

            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar);

            GUIUtility.GetControlID(645789, FocusType.Passive);

            DrawHead();

            editor.OnInspectorGUI();

            EditorGUILayout.EndScrollView();

            GUILayout.Space(15);
        }

        public virtual void DrawHead()
        {

        }

    }

}
