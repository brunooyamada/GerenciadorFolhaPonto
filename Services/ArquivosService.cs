using GerenciadorFolhaPonto.Models;
using GerenciadorFolhaPonto.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;

namespace GerenciadorFolhaPonto.Services
{
    public class ArquivosService
    {
        private const string _caminhoArquivo = @"C:\Recebidos\";

        public async Task<PontoDepartamentoModel> ProcessaArquivo(IFormFile formFile)
        {
            PontoDepartamentoModel departamento = new PontoDepartamentoModel();

            if (formFile.Length > 0)
            {
                // Cria o diretório se não existir
                if (!Directory.Exists(_caminhoArquivo))
                {
                    System.IO.Directory.CreateDirectory(_caminhoArquivo);
                }

                // Copia o arquivo
                var filePath = _caminhoArquivo + "\\" + formFile.FileName;
                using (var stream = System.IO.File.Create(filePath))
                {
                    await formFile.CopyToAsync(stream);
                }


                // Grava na lista
                List<PontoModel> listaPonto = new List<PontoModel>();

                using var file = new StreamReader(filePath);
                string? linha;

                while ((linha = file.ReadLine()) != null)
                {
                    if (!linha.Contains("Nome"))
                    {
                        PontoModel ponto = new PontoModel();

                        string[] linhaSeparada = linha.Split(';');

                        ponto.Codigo = int.Parse(linhaSeparada[0]);
                        ponto.Nome = linhaSeparada[1];
                        ponto.ValorHora = linhaSeparada[2];
                        ponto.Data = DateTime.Parse(linhaSeparada[3]);
                        ponto.Entrada = DateTime.Parse(linhaSeparada[3] + " " + linhaSeparada[4]);
                        ponto.Saida = DateTime.Parse(linhaSeparada[3] + " " + linhaSeparada[5]);
                        ponto.Almoco = linhaSeparada[6];

                        listaPonto.Add(ponto);
                    }
                }
                file.Close();


                // Processa a saída
                string[] nomesPlanilha = formFile.FileName.Replace(".csv", "").Split('-');

                departamento.Departamento = nomesPlanilha[0];
                departamento.MesVigencia = nomesPlanilha[1];
                departamento.AnoVigencia = int.Parse(nomesPlanilha[2]);
                departamento.TotalDescontos = 0;
                departamento.TotalExtras = 0;

                // Funcionario
                List<FuncionarioModel> funcionarios = new List<FuncionarioModel>();
                var listaAgrupada =
                    from item in listaPonto
                    group item by item.Codigo into grupo
                    orderby grupo.Key
                    select grupo;

                foreach (var funcionarioGroup in listaAgrupada)
                {
                    var funcionario = new FuncionarioModel();
                    funcionario.Codigo = funcionarioGroup.Key;
                    funcionario.Nome = funcionarioGroup.FirstOrDefault().Nome;
                    funcionario.DiasTrabalhados = funcionarioGroup.GroupBy(x => x.Data).Count();
                    funcionario.HorasExtras = 0;
                    funcionario.TotalReceber = 0;
                    funcionario.DiasExtras = 0;
                    funcionario.DiasFalta = 0;
                    funcionario.HorasDebito = 0;
                    foreach (var horas in funcionarioGroup)
                    {
                        float valorHora = ValorHora(horas.ValorHora);
                        funcionario.TotalReceber += valorHora * (horas.Saida.Subtract(horas.Entrada).Hours - 1);

                        if (horas.Data.DayOfWeek == DayOfWeek.Saturday || horas.Data.DayOfWeek == DayOfWeek.Sunday)
                        {
                            funcionario.HorasExtras += horas.Saida.Subtract(horas.Entrada).Hours - 1;
                            funcionario.DiasExtras++;

                            var horasExtrasDia = horas.Saida.Subtract(horas.Entrada).Hours - 1;
                            departamento.TotalExtras += valorHora * horasExtrasDia;
                        }
                        else
                        {
                            var horasExtrasDia = horas.Saida.Subtract(horas.Entrada).Hours - 9;

                            if (horasExtrasDia > 0)
                            {
                                funcionario.HorasExtras += horasExtrasDia;
                                departamento.TotalExtras += valorHora * horasExtrasDia;
                            }
                            else
                            {
                                funcionario.HorasDebito += Math.Abs(horasExtrasDia);
                                //funcionario.HorasExtras = 0;

                                departamento.TotalDescontos += valorHora * Math.Abs(horasExtrasDia);
                            }
                        }
                    }

                    // Calcula os dias de falta
                    int mesVigencia = MesEnum.ExibirMes(departamento.MesVigencia);
                    for (int dia=1; dia < DateTime.DaysInMonth(departamento.AnoVigencia, mesVigencia); dia++)
                    {
                        var diaDaSemana = new DateTime(departamento.AnoVigencia, mesVigencia, dia).DayOfWeek;
                        var diaVigente = new DateTime(departamento.AnoVigencia, mesVigencia, dia);
                        
                        if (diaDaSemana != DayOfWeek.Saturday || diaDaSemana != DayOfWeek.Sunday)
                        {
                            if (!funcionarioGroup.Any(x => x.Data == diaVigente))
                            {
                                funcionario.DiasFalta++;
                                funcionario.HorasDebito += 8;
                                departamento.TotalDescontos += ValorHora(funcionarioGroup.Max(x => x.ValorHora)) * 8;
                            }
                        }
                    }

                    funcionarios.Add(funcionario);
                }

                departamento.Funcionarios = funcionarios;
                departamento.TotalPagar = funcionarios.Sum(x => x.TotalReceber);
            }

            

            return departamento;
        }

        private float ValorHora(string valorHora)
        {
            return float.Parse(valorHora.Replace("R$", "").Replace(" ", ""));
        }

    }
}
