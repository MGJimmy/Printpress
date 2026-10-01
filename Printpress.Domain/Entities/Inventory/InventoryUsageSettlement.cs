namespace Printpress.Domain;

public class InventoryUsageSettlement : Entity
{
    public Guid InventoryItemId { get; private set; }
    public int Quantity { get; private set; }
    public InventoryUsageSettlementType SettlementType { get; private set; }
    public string Notes { get; private set; }
    public DateTime OccurredAt { get; private set; }
    public bool IsVoided { get; private set; }
    public string VoidReason { get; private set; }
    public DateTime? VoidedAt { get; private set; }
    public string VoidedBy { get; private set; }

    public virtual InventoryItem InventoryItem { get; private set; }

    private InventoryUsageSettlement()
    {
    }

    public InventoryUsageSettlement(
        Guid inventoryItemId,
        int quantity,
        InventoryUsageSettlementType settlementType,
        string notes,
        DateTime occurredAt)
    {
        if (inventoryItemId == Guid.Empty)
            throw new BusinessExceptions(LocalizationKeys.Inventory.UsageSettlementItemRequired);

        if (quantity <= 0)
            throw new BusinessExceptions(LocalizationKeys.Inventory.UsageSettlementQuantityMustBePositive);

        if (!Enum.IsDefined(settlementType))
            throw new BusinessExceptions(LocalizationKeys.Inventory.UsageSettlementTypeInvalid);

        if (string.IsNullOrWhiteSpace(notes))
            throw new BusinessExceptions(LocalizationKeys.Inventory.UsageSettlementNotesRequired);

        var trimmedNotes = notes.Trim();
        if (trimmedNotes.Length > 500)
            throw new BusinessExceptions(LocalizationKeys.Inventory.UsageSettlementNotesMaxLength);

        if (occurredAt == default)
            throw new BusinessExceptions(LocalizationKeys.Inventory.UsageSettlementDateRequired);

        InventoryItemId = inventoryItemId;
        Quantity = quantity;
        SettlementType = settlementType;
        Notes = trimmedNotes;
        OccurredAt = occurredAt;
    }

    public void MarkAsVoided(string reason, string userId)
    {
        if (IsVoided)
            throw new BusinessExceptions(LocalizationKeys.Inventory.UsageSettlementAlreadyVoided);

        if (string.IsNullOrWhiteSpace(reason))
            throw new BusinessExceptions(LocalizationKeys.Invoices.ReasonRequired);

        var trimmed = reason.Trim();
        if (trimmed.Length > 500)
            throw new BusinessExceptions(LocalizationKeys.Inventory.UsageSettlementNotesMaxLength);

        IsVoided = true;
        VoidReason = trimmed;
        VoidedAt = DateTime.UtcNow;
        VoidedBy = userId;
    }
}
