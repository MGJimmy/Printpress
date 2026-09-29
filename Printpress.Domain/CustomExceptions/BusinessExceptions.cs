
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain
{
    public class BusinessExceptions : Exception
    {
        public string ErrorLocalizationKey { get; set; }
        public object[] Args { get; }

        public BusinessExceptions(string errorLocalizationKey, params object[] args) : base(errorLocalizationKey)
        {
            ErrorLocalizationKey = errorLocalizationKey;
            Args = args ?? [];
        }
    }
}
