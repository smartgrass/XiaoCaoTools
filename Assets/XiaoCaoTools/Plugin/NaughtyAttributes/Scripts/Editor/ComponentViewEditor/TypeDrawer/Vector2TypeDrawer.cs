using System;
using UnityEditor;
using UnityEngine;

namespace ET
{
    [TypeDrawer]
    public class Vector2TypeDrawer: ITypeDrawer
    {
        public Type HandlesType()
        {
            return typeof(Vector2);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.Vector2Field(memberName, (Vector2) value);
        }
    }
}