namespace Printpress.Domain;

public class Lender : Entity
{
    public string Name { get; private set; }
    public string Phone { get; private set; }
    public string Notes { get; private set; }

    private Lender()
    {
    }

    public Lender(string name, string phone, string notes)
    {
        Apply(name, phone, notes);
    }

    public void Update(string name, string phone, string notes)
    {
        Apply(name, phone, notes);
    }

    private void Apply(string name, string phone, string notes)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessExceptions(LocalizationKeys.Lenders.NameRequired);

        var trimmed = name.Trim();
        if (trimmed.Length > 200)
            throw new BusinessExceptions(LocalizationKeys.Lenders.NameMaxLength);

        Name = trimmed;

        var phoneTrimmed = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        if (phoneTrimmed is { Length: > 50 })
            throw new BusinessExceptions(LocalizationKeys.Lenders.PhoneMaxLength);

        var notesTrimmed = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        if (notesTrimmed is { Length: > 500 })
            throw new BusinessExceptions(LocalizationKeys.Lenders.NotesMaxLength);

        Phone = phoneTrimmed;
        Notes = notesTrimmed;
    }
}
