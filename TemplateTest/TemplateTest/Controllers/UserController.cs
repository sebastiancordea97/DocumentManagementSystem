using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using TemplateTest.Helpers;
using TemplateTest.Models;

namespace TemplateTest.Controllers
{
    [Authorize(Roles = "2")]
    public class UserController : BaseController
    {
        private IWebHostEnvironment _webHostEnvironment;

        private readonly string HSMServerUrl = "http://localhost:5010/DocumentSign/sign";

        public UserController(LicentaTestContext context, IOptions<AppConfiguration> configuration, IWebHostEnvironment environment)
           : base(context, configuration)
        {
            _webHostEnvironment = environment;
        }


        public IActionResult Index([FromQuery]int id = 0)
        {
            if (id == 0) id = UserId;

            return View(createDocumentsPageViewModel(id));
        }

        public IActionResult MessageIndex([FromQuery]string message, [FromQuery]bool isError)
        {
            if (isError) CreateErrorMessage(message);
                    else CreateSuccessMessage(message);
            return Redirect("Index");
        }

        public IActionResult GetDocument(string filePath)
        {
           // if(UserId nu este autorizat sa preia fisierul)
            //return redirectToIndex();
            var getPath = Path.Combine(_webHostEnvironment.WebRootPath, Path.GetFullPath(filePath));
            var filename = Path.GetFileName(getPath);

            return PhysicalFile(getPath, "Application/pdf",filename);

        }

        public IActionResult UploadDocument(DocumentViewModel document)
        {
            try
            {
               

                using (var inputStream = Request.Form.Files[0].OpenReadStream())
                {
                    string extention = Path.GetExtension(Request.Form.Files[0].FileName);

                    if(extention == ".pdf")
                    {
                        string documentPath = appendDocumentToFileSystem(Request.Form.Files[0].FileName, User.Identity.Name, inputStream);

                        appendDocumentToDatabase(documentPath, document);

                        CreateSuccessMessage("You have succesfully uploaed a new document!");

                    }else
                    {
                        CreateErrorMessage("You need to upload only pdfs, please try again!!!");
                    }
                       
                

                    
                }
            }
            catch (Exception exception)
            {
                //todo: return a better response
                return Redirect("Index");
            }

            return Redirect("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteDocument([FromQuery]int id = 0)
        {
            if (id > 0)
            {
                var documents = await _dbContext.Documents.FirstOrDefaultAsync(x => x.DocId == id);
                if (documents != null)
                {
                    _dbContext.Documents.Remove(documents);
                    await _dbContext.SaveChangesAsync();
                    TempData["Message"] = "You succesfully removed the Document " + Path.GetFileName(documents.DocumentPath) + "!";
                    TempData["Type"] = "success";
                }
                else
                {
                    TempData["Message"] = "The Document you requested could not be found!";
                    TempData["Type"] = "danger";
                }
            }
            else
            {
                TempData["Message"] = "Your request could not be done!";
                TempData["Type"] = "danger";
            }
            return Redirect("Index");
        }


        [HttpGet]
        public async Task<IActionResult> ApproveDocument([FromQuery]int id = 0)
        {

            if (id > 0)
            {
                var documents = await _dbContext.Documents.FirstOrDefaultAsync(x => x.DocId == id);
                if (documents != null)
                {
                    documents.Status = Documents.DocumentStatus.Approved.ToString();
                    _dbContext.Update(documents);
                    await _dbContext.SaveChangesAsync();
                    TempData["Message"] = "You succesfully approved the Document: " + Path.GetFileName(documents.DocumentPath) + "!";
                    TempData["Type"] = "success";
                }
                else
                {
                    TempData["Message"] = "The Document you requested could not be found!";
                    TempData["Type"] = "danger";
                }
            }
            else
            {
                TempData["Message"] = "Your request could not be done!";
                TempData["Type"] = "danger";
            }
            return Redirect("Index");

        }


        [HttpGet]
        public async Task<IActionResult> RevokeDocument([FromQuery]int id = 0)
        {

            if (id > 0)
            {
                var documents = await _dbContext.Documents.FirstOrDefaultAsync(x => x.DocId == id);
                if (documents != null)
                {
                    documents.Status = Documents.DocumentStatus.Revoked.ToString();
                    _dbContext.Update(documents);
                    await _dbContext.SaveChangesAsync();
                    TempData["Message"] = "You succesfully revoked the Document: " + Path.GetFileName(documents.DocumentPath) + "!";
                    TempData["Type"] = "success";
                }
                else
                {
                    TempData["Message"] = "The Document you requested could not be found!";
                    TempData["Type"] = "danger";
                }
            }
            else
            {
                TempData["Message"] = "Your request could not be done!";
                TempData["Type"] = "danger";
            }
            return Redirect("Index");

        }

        public async Task<IActionResult> SignDocument([FromQuery]int documentId = 0)
        {

            Documents document = _dbContext
                .Documents
                .Include(x => x.User)
                .Where(doc => doc.DocId == documentId).FirstOrDefault(); //get document by id

            if (document == null) return Redirect("Index");
            if (document.SignInProgress ?? false)
            {
                CreateErrorMessage("This document is already in signing process by you or another user. Please try again later.");
                return Redirect("Index");
            }

            setDocumentSigningStatus(document, false);

            string documentPath = Path.Combine(_webHostEnvironment.WebRootPath, Path.GetFullPath(document.DocumentPath));

            byte[] signedDocument = await signDocument(documentPath);

            if (signedDocument.Length == 0)
                CreateErrorMessage("Failed to sign the document. Contact the administrator.");
            else
            {
                using (var inputStream = new MemoryStream(signedDocument))
                {
                    appendDocumentToFileSystem(Path.GetFileName(document.DocumentPath), document.User.Lastname, inputStream);
                }
                CreateSuccessMessage("Document signed with success!");
            }

            setDocumentSigningStatus(document, true);

            return Redirect("Index");
        }


        #region Async calls actions

        [HttpPost]
        public async Task<IActionResult> StageDocument()
        {
            bool result = true;
            try
            {
                JObject body = JObject.Parse(await new StreamReader(Request.Body).ReadToEndAsync());

                result = appendDocumentSigners(body["signers"].ToObject<IEnumerable<string>>(),
                                       body["documentId"].ToObject<int>());
                if (result == true)
                {
                    CreateSuccessMessage("You have succesfully sent the document!");
                    updateStatus(body["documentId"].ToObject<int>());
                }
                else
                    return Json(new
                    {
                        message = "You need to provide an existing email address!",
                        isError = true
                    }); 
            }
            catch (Exception exception)
            {
                return Json(new
                {
                    message = "An exception was ocurred while processing your request",
                    isError = true
                });
            }

            return Json(
                 new
                 {
                     message = "Status changed with success",
                     isError = false
                 }
            );

        }

        #endregion

        #region Private section
        private async Task<byte[]> signDocument(string documentPath)
        {
            using (var client = new HttpClient())
            {
                var requestContent = new MultipartFormDataContent();

                using (var fileStream = System.IO.File.OpenRead(documentPath))
                {
                    HttpContent content = new StreamContent(fileStream);
                    content.Headers.ContentType =
                        MediaTypeHeaderValue.Parse("application/pdf");

                    requestContent.Add(content, "File", Path.GetFileName(documentPath));
                    requestContent.Add(new StringContent(
                         User.Claims.Where(claim => claim.Type == ClaimTypes.Email).FirstOrDefault().Value),
                         "Email");

                    using (var response = await client.PostAsync(HSMServerUrl, requestContent))
                    {
                        return await response.Content.ReadAsByteArrayAsync();
                    }
                }
            }

        }

        private string appendDocumentToFileSystem(string fileName, string username, Stream inputStream)
        {
            
            string folderPath = Path.Combine(_configuration.SignedDocumentsPath, username);
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string documentPath = Path.Combine(folderPath, fileName);


            if (System.IO.File.Exists(documentPath)) System.IO.File.Delete(documentPath);

            using (var outputStream = System.IO.File.Create(
                  documentPath
                ))
            {
                inputStream.CopyTo(outputStream);
            }


            return documentPath;
        }

        private void updateStatus(int docId)
        {
            var document = _dbContext.Documents.FirstOrDefault(x => x.DocId == docId);
            document.Status = Documents.DocumentStatus.Pending.ToString();
            _dbContext.Documents.Update(document);
            _dbContext.SaveChanges();

        }



        private void appendDocumentToDatabase(string documentPath, DocumentViewModel document)
        {

            var newDocument = new Documents()
            {
                DocumentPath = documentPath,
                EndDate = document.EndDate,
                UserId = Convert.ToInt32(User.Claims.Where(claim => claim.Type == ClaimTypes.NameIdentifier)
                                                    .FirstOrDefault()?.Value),
                Status = Documents.DocumentStatus.Uploaded.ToString(),
                SignInProgress = false,
            };

            _dbContext.Documents.Add(newDocument);
            _dbContext.SaveChanges();

        }

        private bool appendDocumentSigners(IEnumerable<string> signerEmails, int documentId)
        {
            var signers = _dbContext.Users.Where(user => signerEmails.Contains(user.Email));
            if (signers.Count() == 0)
            {
                CreateErrorMessage("The email Doesn't exist!!!");
                return false;
            }

            foreach (var signer in signers)
            {
                var documentSigner = new DocumentsSigners()
                {
                    DocumentId = documentId,
                    Signed = false,
                    Signer = signer.UserId,
                    Checked = false
                };

                _dbContext.DocumentsSigners.Add(documentSigner);
            }

            _dbContext.SaveChanges();
            return true;
        }

        private DocumentsPageViewModel createDocumentsPageViewModel(int userId)
        {
            List<Documents> documents = null;
            documents = _dbContext.Documents
                .Include(x => x.DocumentsSigners)
                .Where(x => x.UserId == userId)
                .ToList();

            List<Documents> docsToSign = null;
            docsToSign = _dbContext.Documents
                .Include(x => x.DocumentsSigners)
                .Include(x => x.User)
                .Where(x => x.DocumentsSigners.Where(docSigner => docSigner.Signer == userId).FirstOrDefault() != null)
                .ToList();

            return new DocumentsPageViewModel(documents, docsToSign);
        }

       

        private void setDocumentSigningStatus(Documents document, bool release)
        {
            document.SignInProgress = !release;

            _dbContext.Documents.Update(document);
            _dbContext.SaveChanges();
        }
        #endregion



    }





}