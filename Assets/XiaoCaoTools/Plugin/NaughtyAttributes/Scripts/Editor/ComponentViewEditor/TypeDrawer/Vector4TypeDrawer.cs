using System;
using UnityEditor;
using UnityEngine;

namespace ET
{
    [TypeDrawer]
    public class Vector4TypeDrawer: ITypeDrawer
    {
        public Type HandlesType()
        {
            return typeof(Vector4);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.Vector4Field(memberName, (Vector4) value);
        }
    }
}