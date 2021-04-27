using System;
using System.Collections.Generic;

namespace DataAccessLibrary.Models
{
    public partial class Documents
    {
        public Documents()
        {
            DocumentsSigners = new HashSet<DocumentsSigners>();
        }

        public int DocId { get; set; }
        public string DocumentPath { get; set; }
        public DateTime? EndDate { get; set; }
        public int? UserId { get; set; }
        public string Status { get; set; }
        public bool? SignInProgress { get; set; }

        public virtual Users User { get; set; }
        public virtual ICollection<DocumentsSigners> DocumentsSigners { get; set; }
    }
}
