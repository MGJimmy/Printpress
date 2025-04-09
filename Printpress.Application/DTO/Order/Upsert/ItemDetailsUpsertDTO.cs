using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Printpress.Domain.Enums;

namespace Printpress.Application
{
    public class ItemDetailsUpsertDTO : IObjectState
    {
        public int Id { get; set; }
        public int ItemId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ItemDetailsKeyEnum Key { get; set; }

        public string Value { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ObjectState ObjectState { get; set; }
    }
}
