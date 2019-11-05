using System;

namespace Ambition.Utils
{
    public class TypeUtils
    {
        public static bool IsClassAssignableFrom(Type classType, Type type)
        {
            return classType.IsClass && !classType.IsAbstract && type.IsAssignableFrom(classType);
        }
    }
}