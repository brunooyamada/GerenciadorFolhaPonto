namespace GerenciadorFolhaPonto.Models
{
    public class PontoModel
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string ValorHora { get; set; }
        public DateTime Data { get; set; }
        public DateTime Entrada { get; set; }
        public DateTime Saida { get; set; }
        public string Almoco { get; set; }
    }
}
