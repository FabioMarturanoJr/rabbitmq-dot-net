namespace Rabbit.Api.Domain;

public class Message2
{
    public Message2(string texto, DateTime dataEnvio, Origem origem = Origem.Service)
    {
        Texto = texto;
        DataEnvio = dataEnvio;
        Origem = origem;
    }

    public string Texto { get; set; }
    public DateTime DataEnvio { get; set; }
    public Origem Origem { get; set; }
}
