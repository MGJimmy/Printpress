using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Application
{
    public static class ResponseMessage
    {
        public const string InvalidPayload = "Invalid payload";
        public const string InternalServerError = "Internal server error, please try again else contact admistration";
        public const string Success = "Request proceed successfully";
        public const string NoDataFound = "No data found";

        public const string ValidationFailure = "One or more validation errors occurred.";

        public static string CreateInvalidDataMessage(string parameterName)
        {
            return $"Invalid {parameterName} data";
        }

        public static string CreateIdNotExistMessage(int id)
        {
            return $"No data exist for id ({id})";
        }

        public static string CreateInvalidEnumValueMessage(Type enumType, string paramterValueName)
        {
            if (!enumType.IsEnum)
            {
                throw new Exception($"Function {MethodBase.GetCurrentMethod().Name} parameter type is ({enumType.Name}) and it allows only parameter of Type Enum");
            }
            return $"Invalid {paramterValueName} value must be one of the following options [{string.Join(" - ", Enum.GetNames(enumType))}]";
        }
    }
}
