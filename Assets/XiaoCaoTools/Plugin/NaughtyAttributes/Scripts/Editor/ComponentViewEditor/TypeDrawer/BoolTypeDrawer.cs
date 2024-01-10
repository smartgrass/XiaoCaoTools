using System;
using UnityEditor;

namespace ET
{
    [TypeDrawer]
    public class BoolTypeDrawer: ITypeDrawer
    {
        public Type HandlesType()
        {
            return typeof(bool);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.Toggle(memberName, (bool) value);
        }
    }
}