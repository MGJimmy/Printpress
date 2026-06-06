namespace Printpress.Application;

/// <summary>
/// DTO containing basic information (Id and Name) used for dropdowns and filters
/// Don't include any additional properties 
/// </summary>
public class EnumBasicInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}
