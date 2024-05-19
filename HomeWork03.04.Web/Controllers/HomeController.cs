using HomeWork03._04.Data.wwwroot;
using HomeWork03._04.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HomeWork03._04.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=SimchaFund;Integrated Security=true;";

        public IActionResult Index()
        {
            if (TempData["Meassage"] != null)
            {
                ViewBag.Message = TempData["Meassage"];
            }
            Manager mgr = new(_connectionString);
            SimchasViewModel vm = new();
            vm.Simchas = mgr.GetSimchas();
            vm.Contributors = mgr.TotalContributorsInSystem();
            return View(vm);
        }
        

        public IActionResult Contributions(int id)
        {
            Manager mgr = new(_connectionString);
            SimchaContibutorViewModel vm = new();
            vm.Contributors = mgr.GetSimchaContributors(id);
            vm.Simcha = mgr.GetSimcha(id);
            return View(vm);
        }

        [HttpPost]
        public IActionResult AddSimcha(string simchaName, DateTime date)
        {
            Manager mgr = new(_connectionString);
            mgr.AddSimcha(simchaName, date);
            TempData["Message"] = $"New Simcha Created: {simchaName}";
            return Redirect("/home/index");
        }

        [HttpPost]
        public IActionResult AddDeposit(int id, decimal amount, DateTime date)
        {
            Manager mgr = new(_connectionString);
            mgr.AddDeposit(id, amount, date);
            return Redirect("/home/contributors");
        }

        [HttpPost]
        public IActionResult AddContributor(Contributor contributor, decimal initialDeposit, DateTime date)
        {
            Manager mgr = new(_connectionString);
            mgr.AddContributor(contributor, initialDeposit, date);
            return Redirect("/home/contributors");

        }

        [HttpPost]
        public IActionResult EditContributor(Contributor contributor, int id)
        {
            Manager mgr = new(_connectionString);
            mgr.UpdateContributor(contributor,id);
            return Redirect("/home/contributors");

        }

        [HttpPost]
        public IActionResult UpdateContributions(List<ContributionInclusion> contributors, int simchaId)
        {
            Manager mgr = new(_connectionString);
            mgr.UpdateSimchaContributions(contributors, simchaId);
            TempData["Message"] = "Simcha updated successfully";
            return RedirectToAction("Index");
        }

        public IActionResult Contributors()
        {
            Manager mgr = new(_connectionString);
            ContributorsViewModel vm = new();
            vm.TotalAvailable = mgr.GetTotalAvailable();
            vm.Contributors = mgr.GetContributors();
            return View(vm);
        }

        public IActionResult History(int id)
        {
            Manager mgr = new(_connectionString);
            HistoryViewModel vm = new();
            if(mgr.GetContributor(id) == null)
            {
                return Redirect("/home/contributors");
            }
            vm.Actions = mgr.GetActions(id);
            vm.Contributor = mgr.GetContributor(id);
            return View(vm);
        }
    }
}
