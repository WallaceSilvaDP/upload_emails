using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace upload_emails.Models
{
    public class UploadModel
    {
        public int arquivoID { get; set; }
        public string arquivoNome { get; set; }
        public string arquivoCaminho
        {
            get; set;
        }
        public List<string> emails { get; set;}
        
    }
}
