using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoFood.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Utilizador")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "PW")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
