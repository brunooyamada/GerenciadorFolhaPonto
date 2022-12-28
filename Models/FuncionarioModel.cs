namespace GerenciadorFolhaPonto.Models
{
    public class FuncionarioModel
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public float TotalReceber { get; set; }
        public float HorasExtras { get; set; }
        public float HorasDebito { get; set; }
        public int DiasFalta { get; set; }
        public int DiasExtras { get; set; }
        public int DiasTrabalhados { get; set; }
    }
}
