using System.Globalization;
using System.Security.Cryptography.Xml;

namespace GerenciadorFolhaPonto.Util
{
    public class MesEnum
    {
        
        public static string ExibirMesPorExtenso(int mes, int ano)
        {
            DateTime data = new DateTime(1, mes, ano);
            return data.ToString("MMMM", CultureInfo.CreateSpecificCulture("pt-br"));
        }

        public static int ExibirMes(string mes)
        {
            IDictionary<int, string> mesDicionario = new Dictionary<int, string>();
            mesDicionario.Add(1, "Janeiro");
            mesDicionario.Add(2, "Fevereiro");
            mesDicionario.Add(3, "Março");
            mesDicionario.Add(4, "Abril");
            mesDicionario.Add(5, "Maio");
            mesDicionario.Add(6, "Junho");
            mesDicionario.Add(7, "Julho");
            mesDicionario.Add(8, "Agosto");
            mesDicionario.Add(9, "Setembro");
            mesDicionario.Add(10, "Outubro");
            mesDicionario.Add(11, "Novembro");
            mesDicionario.Add(12, "Dezembro");

            return mesDicionario.Where(x => x.Value == mes).Select(item => item.Key).FirstOrDefault();
        }
    }
}
