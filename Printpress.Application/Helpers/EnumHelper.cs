using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Application
{
    public static class EnumHelper
    {
        public static bool IsValidEnumValue(Type enumType, string value)
        {
            if (!enumType.IsEnum)
            {
                throw new Exception($"Function {MethodBase.GetCurrentMethod()?.Name} parameter type is ({enumType.Name}) and it allows only parameter of Type Enum");
            }

            return Enum.GetNames(enumType).Any(x => x.ToLower() == value?.ToLower());
        }

        public static T MapStringToEnum<T>(string value)
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception($"Function {MethodBase.GetCurrentMethod()?.Name} passed type is ({typeof(T).Name}) and it works only with Type Enum");
            }

            return (T)Enum.Parse(typeof(T), value, ignoreCase: true);
        }
    }
}
