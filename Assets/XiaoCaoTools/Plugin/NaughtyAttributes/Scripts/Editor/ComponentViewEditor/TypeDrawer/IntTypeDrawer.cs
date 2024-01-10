using System;
using UnityEditor;

namespace ET
{
    [TypeDrawer]
    public class IntTypeDrawer: ITypeDrawer
    {
        [TypeDrawer]
        public Type HandlesType()
        {
            return  typeof (int);
        }

        public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
        {
            return EditorGUILayout.IntField(memberName, (int) value);
        }
    }
}