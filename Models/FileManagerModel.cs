using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoFood.Models
{
    public class FileManagerModel
    {
        // dá acesso para tratar dos ficheiros
        public FileInfo[] Files { get; set; }
        // interface que permite enviar os ficheiro , copiar e detalhe
        public IFormFile IFormFile { get; set; }
        // lista dos ficheiros a enviar (em cima é um ficheiro, neste caso é uma lista)
        public List<IFormFile> IFormFiles { get; set; }
        // armazenar o nome da pasta no servidor
        // para passar a info do controller para a view
        public string PathImagesProduto { get; set; }
    }
}
