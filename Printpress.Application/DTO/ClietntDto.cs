namespace Printpress.Application;

public record ClietntDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Mobile { get; init; }
    public string Address { get; init; }
}
