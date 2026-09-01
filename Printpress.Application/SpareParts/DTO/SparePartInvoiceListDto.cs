namespace Printpress.Application;

public class SparePartInvoiceLineDto
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; }
    public int? PacksPerCarton { get; set; }
    public int? UnitsPerPack { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class SparePartPurchaseInvoiceListItemDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string SupplierName { get; set; }
    public decimal TotalAmount { get; set; }
    public string AttachmentFilePath { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsVoided { get; set; }
    public string VoidReason { get; set; }
    public DateTime? VoidedAt { get; set; }
    public string VoidedBy { get; set; }
    public string VoidedByName { get; set; }
    public List<SparePartInvoiceLineDto> Lines { get; set; } = [];
}

public class SparePartPurchaseInvoiceListDto
{
    public List<SparePartPurchaseInvoiceListItemDto> Invoices { get; set; } = [];
    public int InvoiceCount { get; set; }
    public int LineCount { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
}

public class SparePartSellingInvoiceListItemDto
{
    public Guid Id { get; set; }
    public int InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string ClientName { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsVoided { get; set; }
    public string VoidReason { get; set; }
    public DateTime? VoidedAt { get; set; }
    public string VoidedBy { get; set; }
    public string VoidedByName { get; set; }
    public List<SparePartInvoiceLineDto> Lines { get; set; } = [];
}

public class SparePartSellingInvoiceListDto
{
    public List<SparePartSellingInvoiceListItemDto> Invoices { get; set; } = [];
    public int InvoiceCount { get; set; }
    public int LineCount { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
}
