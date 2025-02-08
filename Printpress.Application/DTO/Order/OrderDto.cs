﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Printpress.Application;

public class OrderDto : IObjectState
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public int ClientId { get; set; }
    public decimal? TotalPrice { get; set; }
    public decimal? TotalPaid { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ObjectState ObjectState { get; set; }
    public List<OrderGroupDTO> OrderGroups { get; set; }
    public List<OrderServiceDTO> OrderServices { get; set; }
}
