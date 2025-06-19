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
}

public class GoogleAuthRequest
{
    public string IdToken { get; set; }  // The ID token from Google
    public string Email { get; set; }
    public string Name { get; set; }
}
