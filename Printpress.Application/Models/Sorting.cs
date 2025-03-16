namespace Printpress.Application
{
    public record Sorting
    {
        public Sorting(string field, SortingDirection dir)
        {
            Field = field;
            Dir = dir;

        }
        public string Field { get; set; }

        public SortingDirection Dir { get; set; }
    }
}
