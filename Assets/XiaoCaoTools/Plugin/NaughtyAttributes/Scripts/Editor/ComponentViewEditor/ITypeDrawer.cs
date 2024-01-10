using System;

namespace ET
{
    public interface ITypeDrawer
    {
        Type HandlesType();

        object DrawAndGetNewValue(Type memberType, string memberName, object value, object target);
    }

    public interface IOtherTypeDrawer
    {
        bool HandlesType(Type type);

        object DrawAndGetNewValue(Type memberType, string memberName, object value, object target);
    }

    public class TypeDrawerAttribute : Attribute
    {
    }
}