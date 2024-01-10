using System;
using UnityEditor;
using UnityEngine;

namespace ET
{
    [TypeDrawer]
    public class AnimationCurveTypeDrawer: ITypeDrawer
    {
        public Type HandlesType()
        {
            return typeof(AnimationCurve);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.CurveField(memberName, (AnimationCurve) value);
        }
    }
}