namespace Printpress.Domain;

public class InventoryConversion : Entity
{
    public Guid InventoryItemId { get; private set; }
    public int Quantity { get; private set; }
    public string Notes { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public InventoryConversionStatus Status { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string CompletedBy { get; private set; }
    public bool IsVoided { get; private set; }
    public string VoidReason { get; private set; }
    public DateTime? VoidedAt { get; private set; }
    public string VoidedBy { get; private set; }

    public virtual InventoryItem InventoryItem { get; private set; }

    private InventoryConversion()
    {
    }

    public InventoryConversion(
        Guid inventoryItemId,
        int quantity,
        string notes,
        DateTime occurredAt)
    {
        if (inventoryItemId == Guid.Empty)
            throw new BusinessExceptions(LocalizationKeys.Inventory.ConversionItemRequired);

        if (quantity <= 0)
            throw new BusinessExceptions(LocalizationKeys.Inventory.ConversionQuantityMustBePositive);

        if (string.IsNullOrWhiteSpace(notes))
            throw new BusinessExceptions(LocalizationKeys.Inventory.ConversionNotesRequired);

        var trimmedNotes = notes.Trim();
        if (trimmedNotes.Length > 500)
            throw new BusinessExceptions(LocalizationKeys.Inventory.ConversionNotesMaxLength);

        if (occurredAt == default)
            throw new BusinessExceptions(LocalizationKeys.Inventory.ConversionDateRequired);

        InventoryItemId = inventoryItemId;
        Quantity = quantity;
        Notes = trimmedNotes;
        OccurredAt = occurredAt;
        Status = InventoryConversionStatus.Open;
    }

    public void Complete(string userId)
    {
        if (IsVoided)
            throw new BusinessExceptions(LocalizationKeys.Inventory.ConversionAlreadyVoided);

        if (Status == InventoryConversionStatus.Completed)
            throw new BusinessExceptions(LocalizationKeys.Inventory.ConversionAlreadyCompleted);

        Status = InventoryConversionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        CompletedBy = userId;
    }

    public void MarkAsVoided(string reason, string userId)
    {
        if (IsVoided)
            throw new BusinessExceptions(LocalizationKeys.Inventory.ConversionAlreadyVoided);

        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessExceptions(LocalizationKeys.Invoices.ReasonRequired);

        var trimmed = reason.Trim();
        if (trimmed.Length > 500)
            throw new BusinessExceptions(LocalizationKeys.Inventory.ConversionNotesMaxLength);

        IsVoided = true;
        VoidReason = trimmed;
        VoidedAt = DateTime.UtcNow;
        VoidedBy = userId;
    }
}
