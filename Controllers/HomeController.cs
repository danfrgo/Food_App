using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjetoFood.Models;
using ProjetoFood.Repositories;
using ProjetoFood.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoFood.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILancheRepository _lancheRepository;

        public HomeController(ILancheRepository lancheRepository)
        {
            _lancheRepository = lancheRepository;
        }

        public IActionResult Index()
        {
            var HomeViewModel = new HomeViewModel
            {
                LanchesPreferidos = _lancheRepository.LanchesPreferidos
            };

            return View(HomeViewModel);
        }

        public ViewResult AccessDenied()
        {
            return View();
        }



    }
}
