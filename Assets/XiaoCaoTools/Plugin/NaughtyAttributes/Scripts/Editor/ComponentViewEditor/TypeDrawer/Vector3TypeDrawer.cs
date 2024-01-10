using System;
using UnityEditor;
using UnityEngine;

namespace ET
{
    [TypeDrawer]
    public class Vector3TypeDrawer: ITypeDrawer
    {
        public Type HandlesType()
        {
            return typeof (Vector3);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.Vector3Field(memberName, (Vector3) value);
        }
    }
}