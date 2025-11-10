namespace Wissance.WebApiToolkit.TestApp.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Login { get; set; }
        public int OrganizationId { get; set; }
        public int[] Roles { get; set; }
    }
}