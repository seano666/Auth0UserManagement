using System;
using System.Collections.Generic;
using System.Text;

namespace Auth0UserManagement.Models
{
    public class OperationResult
    {
        public bool Success { get; set; } = true;
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public int TotalOperations { get; set; } = 0;
        public int CompletedOperations { get; set; } = 0;
    }
}
