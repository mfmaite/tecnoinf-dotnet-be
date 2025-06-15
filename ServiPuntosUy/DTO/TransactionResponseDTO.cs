namespace ServiPuntosUy.DTO;

public class TransactionResponseDTO : TransactionDTO
{
    public TransactionItemDTO[] Items { get; set; }
}
