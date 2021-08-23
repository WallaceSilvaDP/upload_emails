using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using upload_emails.Models;
using upload_emails.Util;
namespace upload_emails.Controllers
{
    public class UploadController : Controller
    {
        UploadModel oModelArquivos = new UploadModel();
        //Define uma instância de IHostingEnvironment
        IWebHostEnvironment _appEnvironment;
        UploadUtil upload;
        public UploadController(IWebHostEnvironment env)
        {
            _appEnvironment = env;
            upload = new UploadUtil(env);
            
        }
        public IActionResult Index()
        {
            return View();
        }

        //método para enviar os arquivos usando a interface IFormFile
        public async Task<IActionResult> UploadArquivo(IFormFile arquivo)
        {
            
            string nomeArquivo = Guid.NewGuid().ToString() + "_" + arquivo.FileName;
            string caminhoDestinoArquivo = _appEnvironment.WebRootPath + @"\Arquivos\";
            string caminhoDestinoArquivoOriginal = caminhoDestinoArquivo + @"\Recebidos\" + nomeArquivo;

            //copia o arquivo para o local de destino original
            using (var stream = new FileStream(caminhoDestinoArquivoOriginal, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }
            
            upload.separarArquivo(nomeArquivo);
            return View(upload.GetArquivos());
        }

        public async Task<FileResult> Download([FromQuery] string file)
        {
            var uploads = Path.Combine(_appEnvironment.WebRootPath, "Arquivos\\Novos");
            var filePath = Path.Combine(uploads, file);
            if (!System.IO.File.Exists(filePath))
                return null;

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, upload.GetContentType(filePath), file);
        }

    }
}

