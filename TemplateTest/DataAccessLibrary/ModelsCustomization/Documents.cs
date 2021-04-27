using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Models
{
    public partial class Documents
    {
        public enum DocumentStatus
        {
            Uploaded,
            Pending,
            Revoked,
            Approved
        }
    }
}
