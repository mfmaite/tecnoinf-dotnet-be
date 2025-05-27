namespace ServiPuntosUy.DTO
{
  public class UserSessionDTO(string token)
  {
    public string token { get; set; } = token;
  }
}