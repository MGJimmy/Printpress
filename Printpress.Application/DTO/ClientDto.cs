namespace Printpress.Application;

public record ClientDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Mobile { get; init; }
    public string Address { get; init; }

}
