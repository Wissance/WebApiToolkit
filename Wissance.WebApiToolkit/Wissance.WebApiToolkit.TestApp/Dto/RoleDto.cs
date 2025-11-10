namespace Wissance.WebApiToolkit.TestApp.Dto
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<int> Users { get; set; }
    }
}