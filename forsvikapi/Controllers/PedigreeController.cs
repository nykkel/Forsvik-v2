using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Forsvik.Core.Model.External;
using forsvikapi.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace forsvikapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ErrorHandlingFilter]
    public class PedigreeController
    {
        [HttpPost]
        [Route("sendone")]
        public ResponseModel<bool> SendOne(PedigreeOne model)
        {
            var smtpClient = new SmtpClient("mailcluster.loopia.se")
            {
                Port = 587,
                Credentials = new NetworkCredential("noreply@forsvik-arkiv.se", "XJsDqLqW4q"),
                EnableSsl = true,
            };

            try
            {
                var json = JsonConvert.SerializeObject(model);
                var body = JValue.Parse(json).ToString(Formatting.Indented);

                smtpClient.Send("pedigree@forsvik-arkiv.se", "xxstig@gmail.com", "Enkät släktforskarföreningen", body);
                return new ResponseModel<bool> {Result = true};
            }
            catch (Exception e)
            {
                return new ResponseModel<bool> { Result = false, Error = e.Message };
            }
        }
    }
}
