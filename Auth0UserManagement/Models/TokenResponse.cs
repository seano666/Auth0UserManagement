using System;
using System.Collections.Generic;
using System.Text;

namespace Auth0UserManagement.Models
{
    class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
    }
}
