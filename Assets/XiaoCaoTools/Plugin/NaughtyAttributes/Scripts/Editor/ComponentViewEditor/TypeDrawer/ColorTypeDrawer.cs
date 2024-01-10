using System;
using UnityEditor;
using UnityEngine;

namespace ET
{
    [TypeDrawer]
    public class ColorTypeDrawer: ITypeDrawer
    {
        public Type HandlesType()
        {
            return typeof(Color);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.ColorField(memberName, (Color) value);
        }
    }
}