
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
        public BusinessExceptions(string errorLocalizationKey) : base(errorLocalizationKey)
        {
            ErrorLocalizationKey = errorLocalizationKey;
        }
    }
}
