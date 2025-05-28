namespace ServiPuntosUy.Requests;

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class SignupRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public int TenantId { get; set; }
}
