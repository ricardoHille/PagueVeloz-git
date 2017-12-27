using MVC.Models;
using MVC.Models.Enum;
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
        public ActionResult Index()
        {
            IEnumerable<MvcPessoa> listPessoa;

            HttpResponseMessage response = VariaveisGlobal.WebApiClient.GetAsync("Pessoas").Result;

            listPessoa = response.Content.ReadAsAsync<IEnumerable<MvcPessoa>>().Result;


            ViewBag.Estado = new SelectList(new List<string> { "Santa Catarina", "Paraná" }, "Estado", "Nome");

            return View(listPessoa);
        }

        public ActionResult Create()
        {
            var pessoaModel = new MvcPessoa();
            return View(_formCadastro, pessoaModel);
        }

        [HttpPost]
        public ActionResult Create(MvcPessoa pessoa)
        {
            string msgErro = "";
            if (!CanCadastrar(pessoa, ref msgErro))
            {
                TempData["ErrorCreate"] = msgErro;
            }

            pessoa.DataCadastro = DateTime.Now;
            HttpResponseMessage response = VariaveisGlobal.WebApiClient.PostAsJsonAsync("Pessoas", pessoa).Result;
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            HttpResponseMessage response = VariaveisGlobal.WebApiClient.GetAsync("Pessoas/"+id.ToString()).Result;

            var pessoaModel = response.Content.ReadAsAsync<MvcPessoa>().Result;

            return View(_formCadastro, pessoaModel);
        }

        [HttpPost]
        public ActionResult Edit(MvcPessoa pessoa)
        {
            HttpResponseMessage response = VariaveisGlobal.WebApiClient.GetAsync("Pessoas/" + pessoa.Id.ToString()).Result;
            var pessoaModel = response.Content.ReadAsAsync<MvcPessoa>().Result;
            pessoa.DataCadastro = pessoaModel.DataCadastro;

            response = VariaveisGlobal.WebApiClient.PutAsJsonAsync("Pessoas/"+pessoa.Id.ToString(), pessoa).Result;
            return RedirectToAction("Index");
        }

        
        public ActionResult Delete(int id)
        {
            HttpResponseMessage response = VariaveisGlobal.WebApiClient.DeleteAsync("Pessoas/" + id.ToString()).Result;
            return RedirectToAction("Index");
        }


        private bool CanCadastrar(MvcPessoa pessoa, ref string mensagemErro)
        {
            if (VariaveisGlobal.idEstado == EnumEstado.SantaCatarina)
            {
                return String.IsNullOrEmpty(pessoa.RG);
            }
            else if (VariaveisGlobal.idEstado == EnumEstado.Parana)
            {
                mensagemErro = "É necessário ser maior de idade para se cadastrar";
                return EhMaiorDeIdade(pessoa.DataNascimento);
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

                if(dataNascimento.Month == DateTime.Now.Month)
                {
                    if(dataNascimento.Day <= DateTime.Now.Day)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}