﻿using Printpress.Domain.Enums;
using System.Text.Json.Serialization;

namespace Printpress.Application;

public class OrderDto : TrackedDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ClientName { get; set; }
    public int ClientId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatusEnum  Status { get; set; }
    public decimal? TotalPrice { get; set; }
    public decimal? TotalPaid { get; set; }

    public List<OrderGroupDTO> OrderGroups { get; set; }
    public List<OrderServiceDTO> OrderServices { get; set; }
}
