using System;
using System.Collections.Generic;

namespace DataAccessLibrary.Models
{
    public partial class DocumentsSigners
    {
        public int SignerId { get; set; }
        public bool? Signed { get; set; }
        public bool Checked { get; set; }
        public int? Signer { get; set; }
        public int? DocumentId { get; set; }

        public virtual Documents Document { get; set; }
    }
}
