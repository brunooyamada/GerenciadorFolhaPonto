using GerenciadorFolhaPonto.Models;
using GerenciadorFolhaPonto.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography.Xml;

namespace GerenciadorFolhaPonto.Controllers
{
    public class ArquivosController : Controller
    {
        //private const string _caminhoArquivo = @"C:\Recebidos\";
        public readonly ArquivosService _arquivosService;

        public ArquivosController(ArquivosService arquivoService)
        {
            _arquivosService = arquivoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportarArquivos(List<IFormFile> arquivos)
        {
            List<PontoDepartamentoModel> departamentos = new List<PontoDepartamentoModel>();
            
            Parallel.ForEach(arquivos, async formFile =>
            {
                departamentos.Add(await new ArquivosService().ProcessaArquivo(formFile));
            });

            Task.WaitAll();
            Thread.Sleep(1000);            

            // Grava em arquivo JSON
            StreamWriter sw = new StreamWriter(@"C:\Recebidos\Processados.json");
            sw.WriteLine(JsonConvert.SerializeObject(departamentos));
            sw.Close();

            Console.WriteLine();
            Console.WriteLine("lista departamentos: " + JsonConvert.SerializeObject(departamentos));


            return View();
        }
    }
}
