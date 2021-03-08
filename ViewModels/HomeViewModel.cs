using ProjetoFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoFood.ViewModels
{
    public class HomeViewModel
    {
        // exibir lanches preferidos
        public IEnumerable<Lanche> LanchesPreferidos { get; set; }
    }
}
