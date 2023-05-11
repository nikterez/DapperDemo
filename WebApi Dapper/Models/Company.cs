namespace WebAPI_Dapper.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();

    }
}
