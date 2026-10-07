using System.Text.Json.Serialization;
using Printpress.Domain;

namespace Printpress.Application;

public class ConsumptionOutRowDto
{
    public Guid Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public int Cartons { get; set; }
    public int Units { get; set; }
    public string WorkerName { get; set; }
    public string Notes { get; set; }
    public string ReferenceLabel { get; set; }
    public string ReferenceRoute { get; set; }
}

public class ConsumptionExecuteRowDto
{
    public Guid Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public string OrderName { get; set; }
    public string WorkerName { get; set; }
    public int Quantity { get; set; }
    public int NumberOfPages { get; set; }
    public int NumberOfPrintingFaces { get; set; }
    public bool IsCover { get; set; }
    public decimal PaperUnits { get; set; }
    public string Notes { get; set; }
    public string ReferenceLabel { get; set; }
    public string ReferenceRoute { get; set; }
}

public class ConsumptionSellingRowDto
{
    public Guid Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public string OrderName { get; set; }
    public int Cartons { get; set; }
    public int Units { get; set; }
    public string Notes { get; set; }
    public string ReferenceLabel { get; set; }
    public string ReferenceRoute { get; set; }
}

public class ConsumptionConversionRowDto
{
    public Guid Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public int Quantity { get; set; }
    public string Notes { get; set; }
    public string ReferenceLabel { get; set; }
    public string ReferenceRoute { get; set; }
}

public class ConsumptionSettlementRowDto
{
    public Guid Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public int Quantity { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public InventoryUsageSettlementType SettlementType { get; set; }

    public string Notes { get; set; }
    public string ReferenceLabel { get; set; }
    public string ReferenceRoute { get; set; }
}

public class InventoryOutMovementProjection
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Cartons { get; set; }
    public string WorkerName { get; set; }
    public string Notes { get; set; }
}

public class ConsumptionExecuteProjection
{
    public Guid Id { get; set; }
    public DateTime ExecutionDate { get; set; }
    public Guid OrderId { get; set; }
    public string OrderName { get; set; }
    public Guid OrderGroupId { get; set; }
    public Guid OrderItemId { get; set; }
    public string WorkerName { get; set; }
    public int Quantity { get; set; }
    public int NumberOfPages { get; set; }
    public int NumberOfPrintingFaces { get; set; }
    public bool IsCover { get; set; }
    public string Notes { get; set; }
}

public class DeliveredSellingProjection
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string OrderName { get; set; }
    public DateTime DeliveryDate { get; set; }
    public int Cartons { get; set; }
    public string Name { get; set; }
}

public class ConversionSourceProjection
{
    public Guid Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public int Quantity { get; set; }
    public string Notes { get; set; }
}

public class SettlementSourceProjection
{
    public Guid Id { get; set; }
    public DateTime OccurredAt { get; set; }
    public int Quantity { get; set; }
    public InventoryUsageSettlementType SettlementType { get; set; }
    public string Notes { get; set; }
}
