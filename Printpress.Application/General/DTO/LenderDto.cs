namespace Printpress.Application;

public class LenderDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Notes { get; set; }
}

public class LenderUpsertDto
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Notes { get; set; }
}
