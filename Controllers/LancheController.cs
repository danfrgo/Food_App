using Microsoft.AspNetCore.Mvc;
using ProjetoFood.Models;
using ProjetoFood.Repositories;
using ProjetoFood.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoFood.Controllers
{
    public class LancheController : Controller
    {
        private readonly ILancheRepository _lancheRepository;
        private readonly ICategoriaRepository _categoriaRepository;

        public LancheController(ILancheRepository lancheRepository, ICategoriaRepository categoriaRepository)
        {
            _lancheRepository = lancheRepository;
            _categoriaRepository = categoriaRepository;
        }

        public IActionResult List(string categoria)
        {
            /*
            ViewBag.Lanche = "Lanches";
            ViewData["Categoias"] = "Categoria";

            
            var lanches = _lancheRepository.Lanches;
            return View(lanches);
            */

            // string _categoria = categoria;
            IEnumerable<Lanche> lanches;
            string categoriaAtual = string.Empty;

            // se for nula ou vazia recebe todos os menus
            if (string.IsNullOrEmpty(categoria))
            {
                lanches = _lancheRepository.Lanches.OrderBy(l => l.LancheId);
                categoria = "Todos os menus";
            }
            else
            {
                /*
                if (string.Equals("Normal", _categoria, StringComparison.OrdinalIgnoreCase))
                {
                    lanches = _lancheRepository.Lanches.Where(l => l.Categoria.CategoriaNome.Equals("Normal")).OrderBy(l => l.Nome);
                }
                else
                {
                    lanches = _lancheRepository.Lanches.Where(l => l.Categoria.CategoriaNome.Equals("Natural")).OrderBy(l => l.Nome);
                }
                categoriaAtual = _categoria; */



                // obter categorias de forma dinamica
                lanches = _lancheRepository.Lanches
                            .Where(p => p.Categoria.CategoriaNome.Equals(categoria))
                            .OrderBy(p => p.Nome);

                categoriaAtual = categoria;
            }



            var lancheslistViewModel = new LancheListViewModel()
            {
                Lanches = lanches,
                CategoriaAtual = categoriaAtual
        };

            return View(lancheslistViewModel);

        }

        public IActionResult Details(int lancheId)
        {
            var lanche = _lancheRepository.Lanches.FirstOrDefault(lanche => lanche.LancheId == lancheId);

            if(lanche == null)
            {
                return View("~/Views/Error/Error.cshtml");
            }
            else
            {
                return View(lanche);
            }

        }

        public ViewResult Search(string searchString)
        {
            string _searchString = searchString;

            IEnumerable<Lanche> lanches;

            string currentCategory = string.Empty;

            if (string.IsNullOrEmpty(_searchString))
            {
                // se a string estiver nula ou vazia, nao faz nenhuma pesquisa em concreto, retorna apenas todos os menus
                lanches = _lancheRepository.Lanches.OrderBy(p => p.LancheId);
            }
            else
            {
                lanches = _lancheRepository.Lanches.Where(p => p.Nome.ToLower().Contains(_searchString.ToLower()));
            }

            return View("~/Views/Lanche/List.cshtml", new LancheListViewModel { Lanches = lanches, CategoriaAtual = "Todos os lanches" });
        }



    }
}
