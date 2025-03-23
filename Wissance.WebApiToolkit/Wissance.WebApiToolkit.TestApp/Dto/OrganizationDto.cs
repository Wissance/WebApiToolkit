namespace Wissance.WebApiToolkit.TestApp.Dto
{
    public class OrganizationDto
    {
        public OrganizationDto()
        {
            Codes = new List<int>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string TaxNumber { get; set; }
        
        public IList<int> Codes { get; set; }
    }
}