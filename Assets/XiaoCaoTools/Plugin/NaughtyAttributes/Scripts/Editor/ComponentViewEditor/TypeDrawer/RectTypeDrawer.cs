using System;
using UnityEditor;
using UnityEngine;

namespace ET
{
    [TypeDrawer]
    public class RectTypeDrawer: ITypeDrawer
    {
        public Type HandlesType()
        {
            return typeof (Rect);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.RectField(memberName, (Rect) value);
        }
    }
}