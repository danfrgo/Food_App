using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjetoFood.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoFood.Models
{
    public class CarrinhoCompra
    {
        private readonly AppDbContext _context;

        public CarrinhoCompra(AppDbContext contexto)
        {
            _context = contexto;
        }
        public string CarrinhoCompraId { get; set; }
        public List<CarrinhoCompraItem> CarrinhoCompraItens { get; set; }


        public static CarrinhoCompra GetCarrinho(IServiceProvider services)
        {
            // define uma sessão acessando o contexto atual (tem que se registar em ISevicesCo
            // "?" -> retorna uma session se o IHttpContextAccessor não for nulo
            ISession session =
                services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;

            // obtem um serviço do tipo contexto
            var context = services.GetService<AppDbContext>();

            // obtem ou gera o ID do carrinho na sessao
            // retorna um carrinho -> "??" senao o Guid gera um novo identificador unico
            // gera um numero aleatorio de 128 bits -> "Guid"
            // o CarrinhoId vai ter um numero unico em cada sessão
            string carrinhoId = session.GetString("CarrinhoId") ?? Guid.NewGuid().ToString();

            // atribui o id do carrinho na sessao
            session.SetString("CarrinhoId", carrinhoId);

            // retorna o carrinho com o contexto atual e o Id atribuido ou obtido
            return new CarrinhoCompra(context)
            {
                CarrinhoCompraId = carrinhoId
            };
        }

        public void AdicionarAoCarrinho(Lanche lanche, int quantidade)
        {
            var carrinhoCompraItem =
                _context.CarrinhoCompraItens.SingleOrDefault(s => s.Lanche.LancheId == lanche.LancheId &&
                                                               s.CarrinhoCompraId == CarrinhoCompraId);

            // verifica se o carrinho existe e senao existir cria um novo
            if (carrinhoCompraItem == null)
            {
                carrinhoCompraItem = new CarrinhoCompraItem
                {
                    CarrinhoCompraId = CarrinhoCompraId,
                    Lanche = lanche,
                    Quantidade = 1
                };
                _context.CarrinhoCompraItens.Add(carrinhoCompraItem);
            }
            else // se existir o carrinho com o item então incrementa a quantidade
            {
                carrinhoCompraItem.Quantidade++;
            }
            _context.SaveChanges();
        }

        public int RemoverDoCarrinho(Lanche lanche)
        {
            var carrinhoCompraItem =
               _context.CarrinhoCompraItens.SingleOrDefault(s => s.Lanche.LancheId == lanche.LancheId &&
                                                              s.CarrinhoCompraId == CarrinhoCompraId);

            var quantidadeLocal = 0;

            // verificar se o carrinho é nulo
            if (carrinhoCompraItem != null)
            {
                // se a quantidade for < 1 , decrementa-se e atribui-se o valor à quantidadeLocal
                if (carrinhoCompraItem.Quantidade > 1)
                {
                    carrinhoCompraItem.Quantidade--;
                    quantidadeLocal = carrinhoCompraItem.Quantidade;
                }
                else // se nao for nulo remove o item do carrinho
                {
                    _context.CarrinhoCompraItens.Remove(carrinhoCompraItem);
                }
            }
            _context.SaveChanges();

            return quantidadeLocal;
        }

        // retorna os itens de um carrinho
        public List<CarrinhoCompraItem> GetCarrinhoCompraItens()
        {
            return CarrinhoCompraItens ?? (CarrinhoCompraItens = _context.CarrinhoCompraItens
                         .Where(c => c.CarrinhoCompraId == CarrinhoCompraId).Include(s => s.Lanche).ToList());
        }

        // limpar carrinho
        public void LimparCarrinho()
        {
            var carrinhoItens = _context.CarrinhoCompraItens
                .Where(carrinho => carrinho.CarrinhoCompraId == CarrinhoCompraId);

            _context.CarrinhoCompraItens.RemoveRange(carrinhoItens);

            _context.SaveChanges();
        }

        public decimal GetCarrinhoCompraTotal()
        {
            var total = _context.CarrinhoCompraItens.Where(c => c.CarrinhoCompraId == CarrinhoCompraId)
                                                        .Select(c => c.Lanche.Preco * c.Quantidade).Sum();

            return total;
        }

    }
}
