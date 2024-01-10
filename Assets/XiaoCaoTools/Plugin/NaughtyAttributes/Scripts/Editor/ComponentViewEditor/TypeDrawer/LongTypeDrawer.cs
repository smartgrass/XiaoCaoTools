using System;
using UnityEditor;

namespace ET
{
    [TypeDrawer]
    public class LongTypeDrawer: ITypeDrawer
    {
        [TypeDrawer]
        public Type HandlesType()
        {
            return typeof (long);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.LongField(memberName, (long) value);
        }
    }
}