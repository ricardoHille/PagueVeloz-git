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
    }
}