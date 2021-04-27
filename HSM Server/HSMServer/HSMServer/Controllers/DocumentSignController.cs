using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using HSMServer.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Syncfusion.Pdf.Security;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;

namespace HSMServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentSignController : ControllerBase
    {       
        private readonly ILogger<DocumentSignController> _logger;

        public DocumentSignController(ILogger<DocumentSignController> logger)
        {
            _logger = logger;
        }

        [HttpPost("sign")]
        public IActionResult Sign([FromForm] UserFile userFile)
        {

            Byte[] fs = readFileFromRequest(userFile);

            //initialize the Windows store
            X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);


            //Find the certificate using subjectName print. 
            X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates.Find(X509FindType.FindBySubjectName, userFile.Email, false);

            if (collection.Count == 0) return new EmptyResult();

            X509Certificate2 digitalID = collection[0];



            //Se încarcă documentul
            PdfLoadedDocument document = new PdfLoadedDocument(fs);

            //Se stochează numărul de pagini ale documentului
            PdfLoadedPage page = document.Pages[0] as PdfLoadedPage;

            //Se stabilește câmpul inițial pentru semnare
            PdfSignatureField signatureField = new PdfSignatureField(page, "signature filed");
            signatureField.Bounds = new RectangleF(0, 0, 100, 100);

            //Se Instanțiază certificatul pentru semnare
            PdfCertificate certificate = new PdfCertificate(digitalID);

            //se generează semnătura
            signatureField.Signature = new PdfSignature(document, page, certificate, "Signature nr:"); ;

            //se specifică obțiunile 
            signatureField.Signature.DocumentPermissions = PdfCertificationFlags.AllowFormFill | PdfCertificationFlags.AllowComments 
                                                                                               | PdfCertificationFlags.ForbidChanges;
            
            signatureField.Signature.Settings.DigestAlgorithm = DigestAlgorithm.SHA256;






            MemoryStream ms = new MemoryStream();
            //save the pdf
            document.Save(ms);
            ms.Position = 0;
            document.Close(true);
            return File(ms.ToArray(), "application/pdf");

        }

            #region Private

            private byte[] readFileFromRequest(UserFile userFile)
        {

            using (var inputStream = userFile.File.OpenReadStream())
            {
                using (var memoryStream = new MemoryStream())
                {
                    inputStream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        #endregion
    }
}
