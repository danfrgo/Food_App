using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetoFood.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoFood.Controllers
{
    // [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signManager)
        {
            _userManager = userManager;
            _signInManager = signManager;
        }

        // metodo login onde o forumulario é apresentado
        // [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel()
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            // se nao for valido retorna para a view Login
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            // se os dados forem validos, defino um user
            var user = await _userManager.FindByNameAsync(loginVM.UserName);

            // se o user for localizado, significa que existe
            if (user != null)
            {
                // fazer login passando o user e a password
                var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);

                // se tudo existe
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(loginVM.ReturnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    return Redirect(loginVM.ReturnUrl);
                }
            }
            // se o utilizador for nulo, ocorre um erro
            ModelState.AddModelError("", "Utilizador/Password inválidos ou não localizados");
            return View(loginVM);
        }

        // registar um novo user na app
        // [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // requer validaçao/token para cada requisiçao post (cria um token que depois é validado entre a sessao e o formulario)
        public async Task<IActionResult> Register(LoginViewModel registoVM)
        {
            if (ModelState.IsValid)
            {
                // crio um user e atribuo o nome que foi informado no formulario
                var user = new IdentityUser() { UserName = registoVM.UserName };

                // criar um novo user e respetiva password
                var result = await _userManager.CreateAsync(user, registoVM.Password);

                // se a acçao result foi bem sucedida
                if (result.Succeeded)
                {
                    // no momento do registo adiciona o utilizador padrão ao perfil Member (utilizador comum)
                     await _userManager.AddToRoleAsync(user, "Member");
                     await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("LoggedIn", "Account");
                }
            }
            // se o model state for invalido retorna para a view de registo
            return View(registoVM);
        }

        [HttpPost]
        // [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
   


    }
}
