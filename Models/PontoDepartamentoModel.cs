using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GerenciadorFolhaPonto.Models
{
    public class PontoDepartamentoModel
    {
        public string Departamento { get; set; }
        public string MesVigencia { get; set; }
        public int AnoVigencia { get; set; }
        public float TotalPagar { get; set; }
        public float TotalDescontos { get; set; }
        public float TotalExtras { get; set; }
        public List<FuncionarioModel> Funcionarios { get; set; }
    }
}
