using Microsoft.AspNetCore.Authorization;
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
    public class CarrinhoCompraController : Controller
    {
        private readonly ILancheRepository _lancheRepository;
        private readonly CarrinhoCompra _carrinhoCompra;

        public CarrinhoCompraController(ILancheRepository lancheRepository, CarrinhoCompra carrinhoCompra)
        {
            _lancheRepository = lancheRepository;
            _carrinhoCompra = carrinhoCompra;
        }

        public IActionResult Index()
        {
            var itens = _carrinhoCompra.GetCarrinhoCompraItens();
            _carrinhoCompra.CarrinhoCompraItens = itens;

            var carrinhoCompraViewModel = new CarrinhoCompraViewModel
            {
                CarrinhoCompra = _carrinhoCompra,
                CarrinhoCompraTotal = _carrinhoCompra.GetCarrinhoCompraTotal()
            };
            // vai exibir o carrinho de compras com base na ViewModel
            return View(carrinhoCompraViewModel);
        }

        // adicionar item no carrinho
        [Authorize]
        public RedirectToActionResult AdicionarItemNoCarrinhoCompra(int lancheId)
        {
            var lancheSelecionado = _lancheRepository.Lanches.FirstOrDefault(p => p.LancheId == lancheId);

            if(lancheSelecionado != null)
            {
                _carrinhoCompra.AdicionarAoCarrinho(lancheSelecionado, 1);   
            }
            return RedirectToAction("Index");     
        }

        // remover item do carrinho
        [Authorize]
        public IActionResult RemoverItemDoCarrinhoCompra(int lancheId)
        {
            var lancheSelecionado = _lancheRepository.Lanches.FirstOrDefault(p => p.LancheId == lancheId);

            if (lancheSelecionado != null)
            {
                _carrinhoCompra.RemoverDoCarrinho(lancheSelecionado);
            }
            return RedirectToAction("Index");
        }

    }
}
