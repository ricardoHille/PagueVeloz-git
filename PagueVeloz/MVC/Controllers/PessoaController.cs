using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class PessoaController : Controller
    {

        private const string _formCadastro = "formCadastro";

        // GET: Pessoa
        public ActionResult Index(string nome="", DateTime? dataNascimentoInicio = null, DateTime? dataNascimentoFim = null, DateTime? dataCadastroInicio = null, DateTime? dataCadastroFim = null)
        {
            LimparMensagensSessao();
            IEnumerable<MvcPessoa> listPessoa;

            HttpResponseMessage response = VariaveisGlobal.WebApiClient.GetAsync("Pessoas").Result;

            listPessoa = response.Content.ReadAsAsync<IEnumerable<MvcPessoa>>().Result;

            if (!String.IsNullOrEmpty(nome))
            {
                listPessoa = listPessoa.Where<MvcPessoa>(x => x.Nome.Contains(nome)).ToList();
            }

            if(dataNascimentoInicio != null && dataNascimentoFim != null)
            {
                listPessoa = listPessoa.Where< MvcPessoa >(x => x.DataNascimento >= dataNascimentoInicio.Value && x.DataNascimento <= dataNascimentoFim.Value);
            }
            else
            {
                if (dataNascimentoInicio != null)
                    listPessoa = listPessoa.Where<MvcPessoa>(x => x.DataNascimento.CompareTo(dataNascimentoInicio.Value) >= 0);
            
                if (dataNascimentoFim != null)
                    listPessoa = listPessoa.Where<MvcPessoa>(x => x.DataNascimento.CompareTo(dataNascimentoFim.Value) <= 0);
            }
            
            if (dataCadastroInicio != null && dataCadastroFim != null)
            {
                listPessoa = listPessoa.Where<MvcPessoa>(x => x.DataCadastro >= dataCadastroInicio.Value && x.DataCadastro <= dataCadastroFim.Value);
            }
            else
            {
                if (dataCadastroInicio != null)
                    listPessoa = listPessoa.Where<MvcPessoa>(x => x.DataCadastro.CompareTo(dataCadastroInicio.Value) >= 0);
            
                if (dataCadastroFim != null)
                    listPessoa = listPessoa.Where<MvcPessoa>(x => x.DataCadastro.CompareTo(dataCadastroFim.Value) <= 0);
            }

            return View(listPessoa);
        }

        public ActionResult Create()
        {
            if (!EstadoPreenchido())
            {
                return RedirectToAction("Index");
            }
            var pessoaModel = new MvcPessoa();
            return View(_formCadastro, pessoaModel);
        }

        [HttpPost]
        public ActionResult Create(MvcPessoa pessoa)
        {
            if (!PodeRegistrarPessoa(pessoa))
            {
                return View(_formCadastro, pessoa);
            }

            pessoa.DataCadastro = DateTime.Now;
            HttpResponseMessage response = VariaveisGlobal.WebApiClient.PostAsJsonAsync("Pessoas", pessoa).Result;
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            
            if (!EstadoPreenchido())
            {
                return RedirectToAction("Index");
            }

            HttpResponseMessage response = VariaveisGlobal.WebApiClient.GetAsync("Pessoas/" + id.ToString()).Result;

            var pessoaModel = response.Content.ReadAsAsync<MvcPessoa>().Result;

            return View(_formCadastro, pessoaModel);
        }

        [HttpPost]
        public ActionResult Edit(MvcPessoa pessoa)
        {
            if (!PodeRegistrarPessoa(pessoa))
            {
                return View(_formCadastro, pessoa);
            }

            HttpResponseMessage response = VariaveisGlobal.WebApiClient.GetAsync("Pessoas/" + pessoa.Id.ToString()).Result;
            var pessoaModel = response.Content.ReadAsAsync<MvcPessoa>().Result;
            pessoa.DataCadastro = pessoaModel.DataCadastro;

            response = VariaveisGlobal.WebApiClient.PutAsJsonAsync("Pessoas/" + pessoa.Id.ToString(), pessoa).Result;
            return RedirectToAction("Index");
        }


        public ActionResult Delete(int id)
        {
            HttpResponseMessage response = VariaveisGlobal.WebApiClient.DeleteAsync("Pessoas/" + id.ToString()).Result;
            return RedirectToAction("Index");
        }


        private bool PodeRegistrarPessoa(MvcPessoa pessoa)
        {
            LimparMensagensSessao();
            string estado = (string) Session["estado"];

            if (estado.Equals("SC"))
            {
                if (String.IsNullOrEmpty(pessoa.RG))
                {
                    Session["MsgRGObrigatorio"] = "O campo RG é obrigátorio";
                    return false;
                }
            }
            else if (estado.Equals("PR"))
            {
                if(!EhMaiorDeIdade(pessoa.DataNascimento))
                {
                    Session["MsgMaiorDeIdade"] = "Pessoa precisa ser maior de idade para se cadastrar";
                    return false;
                }
            }

            if (pessoa.CPF.Length < 14)
            {
                Session["MsgCPFInvalido"] = "CPF invalido, informe o cpf da seguinte forma: xxx.xxx.xxx-xx";
                return false;
            }

            if (pessoa.Telefones.Length < 14)
            {
                Session["MsgTelefoneInvalido"] = "Telefone invalido, informe o telefone da seguinte forma: (xx)xxxxx-xxxx";
                return false;
            }

            return true;
        }

        private bool EhMaiorDeIdade(DateTime dataNascimento)
        {
            int AnoBase = DateTime.Today.Year - 18;
            if (dataNascimento.Year < AnoBase)
            {
                return true;
            }

            if (AnoBase == dataNascimento.Year)
            {
                if (dataNascimento.Month < DateTime.Now.Month)
                {
                    return true;
                }

                if (dataNascimento.Month == DateTime.Now.Month)
                {
                    if (dataNascimento.Day <= DateTime.Now.Day)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool EstadoPreenchido()
        {
            if (String.IsNullOrEmpty((string)Session["estado"]))
            {
                Session["MsgEstadoNaoPreenchido"] = "Estado precisa ser selecionado";
                return false;
            }

            return true;
        }

        [HttpGet]
        public ActionResult SetEstado(string estado)
        {
            Session["estado"] = estado;
            Session["MsgEstadoNaoPreenchido"] = "";
            return RedirectToAction("Index");
        }

        private void LimparMensagensSessao()
        {
            Session["MsgRGObrigatorio"] = "";
            Session["MsgMaiorDeIdade"] = "";
            Session["MsgTelefoneInvalido"] = "";
            Session["MsgCPFInvalido"] = "";
        }
    }
}