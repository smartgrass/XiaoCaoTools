using System;
using UnityEditor;

namespace ET
{
    [TypeDrawer]
    public class FloatTypeDrawer: ITypeDrawer
    {
        public Type HandlesType()
        {
            return typeof (float);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.FloatField(memberName, (float) value);
        }
    }
}