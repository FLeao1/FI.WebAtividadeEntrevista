using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (!ModelState.IsValid || !ValidadorCPF.ValidarCPF(model.CPF))
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                if (!ValidadorCPF.ValidarCPF(model.CPF))
                {
                    erros.Add("CPF inválido.");
                }

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }

            if (bo.VerificarExistencia(model.CPF))
            {
                Response.StatusCode = 400;
                return Json("CPF já cadastrado.");
            }

            model.Id = bo.Incluir(new Cliente()
            {
                CEP = model.CEP,
                CPF = model.CPF,
                Cidade = model.Cidade,
                Email = model.Email,
                Estado = model.Estado,
                Logradouro = model.Logradouro,
                Nacionalidade = model.Nacionalidade,
                Nome = model.Nome,
                Sobrenome = model.Sobrenome,
                Telefone = model.Telefone
            });

            return Json("Cadastro efetuado com sucesso");
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model, List<BeneficiarioModel> beneficiarios)
        {
            BoCliente bo = new BoCliente();

            // Validação do estado do modelo e do CPF
            if (!ModelState.IsValid || !ValidadorCPF.ValidarCPF(model.CPF))
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                if (!ValidadorCPF.ValidarCPF(model.CPF))
                {
                    erros.Add("CPF inválido.");
                }

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }

            // Inserção do cliente e obtenção do ID gerado
            model.Id = bo.Incluir(new Cliente()
            {
                CEP = model.CEP,
                CPF = model.CPF,
                Cidade = model.Cidade,
                Email = model.Email,
                Estado = model.Estado,
                Logradouro = model.Logradouro,
                Nacionalidade = model.Nacionalidade,
                Nome = model.Nome,
                Sobrenome = model.Sobrenome,
                Telefone = model.Telefone
            });

            // Inserção de cada beneficiário
            foreach (var beneficiario in beneficiarios)
            {
                if (ValidadorCPF.ValidarCPF(beneficiario.CPF))
                {
                    // Converter BeneficiarioModel para FI.AtividadeEntrevista.DML.Beneficiario
                    var novoBeneficiario = new FI.AtividadeEntrevista.DML.Beneficiario
                    {
                        CPF = beneficiario.CPF,
                        Nome = beneficiario.Nome,
                        IdCliente = model.Id
                    };

                    // Chamar IncluirBeneficiario com o tipo correto
                    bo.IncluirBeneficiario(novoBeneficiario);
                }
            }

            return Json("Cadastro efetuado com sucesso");
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
       
            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    CPF = model.CPF,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone
                });
                               
                return Json("Cadastro alterado com sucesso");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    CPF = cliente.CPF,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone
                };


            }

            return View(model);
        }

        public static class ValidadorCPF
        {
            public static bool ValidarCPF(string cpf)
            {
                cpf = cpf.Replace(".", "").Replace("-", "");

                if (cpf.Length != 11 || cpf.All(c => c == cpf[0]))
                    return false;

                int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
                int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

                string tempCpf = cpf.Substring(0, 9);
                int soma = tempCpf.Select((t, i) => int.Parse(t.ToString()) * multiplicador1[i]).Sum();
                int resto = soma % 11;
                string digito = (resto < 2 ? 0 : 11 - resto).ToString();

                tempCpf += digito;
                soma = tempCpf.Select((t, i) => int.Parse(t.ToString()) * multiplicador2[i]).Sum();
                resto = soma % 11;
                digito += (resto < 2 ? 0 : 11 - resto).ToString();

                return cpf.EndsWith(digito);
            }
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}