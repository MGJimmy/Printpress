namespace Printpress.Application;

public record ClientUpsertDto
{
    public string Name { get; init; }
    public string Mobile { get; init; }
    public string Address { get; init; }

}
