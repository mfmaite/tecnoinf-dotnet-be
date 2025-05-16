namespace ServiPuntosUy.DAO.Models.Central;

public class Tenant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DatabaseName { get; set; }
    public string ConnectionString { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
}
