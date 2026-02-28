namespace Printpress.Domain;

public interface ISoftDelete
{
   bool IsDeleted { get; set; }
}
