using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using ProjetoFood.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ProjetoFood.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminImagensController : Controller
    {
        // para conseguir obter o nome da pasta onde vou guardar as imagens
        private readonly ConfigurationImagens _myConfig;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdminImagensController(IWebHostEnvironment hostingEnvironment, IOptions<ConfigurationImagens> myConfiguration)
        {
            _hostingEnvironment = hostingEnvironment;
            _myConfig = myConfiguration.Value;
        }

        public IActionResult Index()
        {
            // apresenta o formulario com as opçoes para enviar e consultar ficheiros
            return View();
        }

        // files vai ser recebido do form html name="files"
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            // verificar se files tem alguma imagem para enviar, senao houver nada a enviar retorna erro
            if (files == null || files.Count == 0)
            {
                ViewData["Erro"] = "Error: Ficheiro(s) não selecionado(s)";
                return View(ViewData);
            }

            // veifica se a quantidade de ficheiros é superior a 10, se for apresenta mensagem de limite de ficheiros (maximo é 10)
            if(files.Count > 10)
            {
                ViewData["Erro"] = "Error: Quantidade de ficheiros excedeu o limite";
                return View(ViewData);
            }

            // calcular total em bytes dos ficheiros selecionados em files
            long size = files.Sum(f => f.Length);

            // lista de strings para armazenar os nomes dos ficheiros enviados
            var filePathsName = new List<string>();

            // obter o caminho completo dos ficheiros armazenados
            // WebRootPath -> para obter o diretorio da pasta wwwroot
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, _myConfig.NomePastaImagensProdutos);

            // pecorrer cada ficheiro selecionado
            foreach (var formFile in files)
            {
                // verificar se sao ficheiros de imagem
                if(formFile.FileName.Contains(".jpg") || formFile.FileName.Contains(".gif") || formFile.FileName.Contains(".png"))
                {
                    // concatenar o nome do diretorio com o nome do ficheiro para obter o diretorio completo
                    var fileNameWithPath = string.Concat(filePath, "\\", formFile.FileName);

                    // armazenar fileNameWithPath na filePathsName
                    filePathsName.Add(fileNameWithPath);

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        await
                            formFile.CopyToAsync(stream);
                    }
                }
            }

            ViewData["Resultado"] = $"{files.Count} ficheiros foram enviados para o servidor," + $"com tamanho total de : {size} bytes";

            ViewBag.Ficheiros = filePathsName;

            return View(ViewData);
        }

        // obter imagens
        public IActionResult GetImagens()
        {
            // para obter as informaçoes das imagens a exibir
            FileManagerModel model = new FileManagerModel();

            // obter o diretorio completo das imagens no servidor
            var userImagesPath = Path.Combine(_hostingEnvironment.WebRootPath,
                 _myConfig.NomePastaImagensProdutos);


            DirectoryInfo dir = new DirectoryInfo(userImagesPath);
            FileInfo[] files = dir.GetFiles();
            model.PathImagesProduto = _myConfig.NomePastaImagensProdutos;

            if (files.Length == 0)
            {
                ViewData["Erro"] = $"Nenhum ficheiro encontrado na pasta {userImagesPath}";
            }

            model.Files = files;
            return View(model);
        }

        // apagar imagens
        public IActionResult Deletefile(string fname)
        {
            // fname -> nome do ficheiro recebido
            // WebRootPath, _myConfig.NomePastaImagensProdutos -> diretorio obtido
            string _imagemApagar = Path.Combine(_hostingEnvironment.WebRootPath,
                _myConfig.NomePastaImagensProdutos + "\\", fname);

            if ((System.IO.File.Exists(_imagemApagar)))
            {
                System.IO.File.Delete(_imagemApagar);
                ViewData["Apagado"] = $"Ficheiro(s) {_imagemApagar} apagado com sucesso";
            }
            return View("index");
        }


    }
}
