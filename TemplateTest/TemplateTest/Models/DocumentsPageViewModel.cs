using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TemplateTest.Models
{
    /// <summary>
    /// This class represents the view model used for User/Index view
    /// </summary>
    public class DocumentsPageViewModel
    {
        public List<Documents> Documents { get; }

        public List<Documents> DocumentsToSign { get; }

        public DocumentsPageViewModel(List<Documents> documents, List<Documents> documentsToSign)
        {
            this.Documents = documents;
            this.DocumentsToSign = documentsToSign;
        }

    }
}
