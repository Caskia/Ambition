using System;

namespace Ambition.Core.Utils
{
    public class TypeUtils
    {
        public static bool IsClassAssignableFrom(Type classType, Type type)
        {
            return classType.IsClass && !classType.IsAbstract && type.IsAssignableFrom(classType);
        }
    }
}