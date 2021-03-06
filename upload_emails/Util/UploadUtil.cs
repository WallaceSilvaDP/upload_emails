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

namespace upload_emails.Util
{
    public class UploadUtil
    {
        IWebHostEnvironment _appEnvironment;
        public UploadUtil(IWebHostEnvironment env)
        {
            _appEnvironment = env;
        }


        private Boolean ValidateEmail(string email_recebido)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email_recebido);
            if (match.Success)
                return true;
            else
                return false;
        }
        public void separarArquivo(string nome_arquivo)
        {
            int counter = 0;
            string line;

            string caminhoDestinoArquivo = _appEnvironment.WebRootPath + @"\Arquivos";

            StreamReader file = new StreamReader(caminhoDestinoArquivo + @"\Recebidos\" + nome_arquivo);
            StreamWriter arquivoNovo = new StreamWriter(caminhoDestinoArquivo + @"\Novos\01.txt", false, Encoding.ASCII);

            ArrayList emailsNovo = new ArrayList();

            while ((line = file.ReadLine()) != null)
            {
                if (ValidateEmail(line) && !emailsNovo.Contains(line))
                {
                    emailsNovo.Add(line + Environment.NewLine);
                    arquivoNovo.Write(line + Environment.NewLine);
                    counter++;
                }

                if (counter % 5 == 0)
                {
                    arquivoNovo.Close();
                    arquivoNovo = new StreamWriter(caminhoDestinoArquivo + @"\Novos\0" + ((counter / 5) + 1).ToString() + ".txt", false, Encoding.ASCII);
                    emailsNovo = new ArrayList();
                }

            }
            arquivoNovo.Close();
            System.IO.File.Delete(caminhoDestinoArquivo + @"\Novos\0" + ((counter / 5) + 1).ToString() + ".txt");

            file.Close();
        }
        public List<UploadModel> GetArquivos()
        {

            List<UploadModel> lstArquivos = new List<UploadModel>();
            string caminhoDestinoArquivo = _appEnvironment.WebRootPath + @"\Arquivos\";
            string caminhoDestinoArquivoOriginal = caminhoDestinoArquivo + @"\Novos\";
            DirectoryInfo dirInfo = new DirectoryInfo(caminhoDestinoArquivoOriginal);

            int i = 0;
            foreach (var item in dirInfo.GetFiles())
            {

                StreamReader file = new StreamReader(dirInfo.FullName + item.Name);
                string line;
                List<string> emails = new List<string>();
                while ((line = file.ReadLine()) != null)
                {
                    emails.Add(line);
                }

                file.Close();
                lstArquivos.Add(new UploadModel()
                {
                    arquivoID = i + 1,
                    arquivoNome = item.Name,
                    arquivoCaminho = dirInfo.FullName + item.Name,
                    emails = emails
                });
                i = i + 1;
            }
            return lstArquivos;
        }

        
        public string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}

