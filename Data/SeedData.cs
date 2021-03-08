using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoFood.Data
{
    public static class SeedData
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider, IConfiguration Configuration)
        {
            //incluir perfis customizados
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            //define os perfis em um array de strings
            string[] roleNames = { "Admin", "Member" };
            IdentityResult roleResult;

            //percorre o array de strings 
            //verifica se o perfil já existe
            foreach (var roleName in roleNames)
            {

                var roleExist = await RoleManager.RoleExistsAsync(roleName);

                // se nao existir, cria perfis e inclui na BD
                if (!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }


            // cria um super utilizador
            var poweruser = new IdentityUser
            {
                //obtem o nome e o email do appsettings
                UserName = Configuration.GetSection("UserSettings")["UserName"],
                Email = Configuration.GetSection("UserSettings")["UserEmail"]
            };

            //obtem a password do appsettings
            string userPassword = Configuration.GetSection("UserSettings")["UserPassword"];

            //verifica se existe um utilizador com o email indicado
            var user = await UserManager.FindByEmailAsync(Configuration.GetSection("UserSettings")["UserEmail"]);

            // se nao existir o user, cria-se
            if (user == null)
            {
                //cria o super utilizador com os dados indicados
                var createPowerUser = await UserManager.CreateAsync(poweruser, userPassword);
                // se o user for criado com sucesso, então é atribuido o pefil de admin
                if (createPowerUser.Succeeded)
                {
                    // atribui o perfil Admin ao utilizador
                    await UserManager.AddToRoleAsync(poweruser, "Admin");
                }
            }


        }
    }
}
