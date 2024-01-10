using System;
using UnityEditor;

namespace ET
{
    [TypeDrawer]
    public class DoubleTypeDrawer: ITypeDrawer
    {
        public Type HandlesType()
        {
            return typeof(double);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.DoubleField(memberName, (double) value);
        }
    }
}