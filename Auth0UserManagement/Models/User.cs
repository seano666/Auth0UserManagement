using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Auth0UserManagement.Models
{      
    public class User
    {
        public string email { get; set; }        
        public ExpandoObject user_metadata { get; set; } = new ExpandoObject();
        public ExpandoObject app_metadata { get; set; } = new ExpandoObject();
        public string name { get; set; }                
        public string user_id { get; set; } = String.Empty;
        public string connection { get; set; }
        public string password { get; set; }
        public bool verify_email { get; set; } = false;

        public void AddUserMetadata(string key, object value)
        {            
            var metadataDict = user_metadata as IDictionary<string, object>;
            metadataDict.Add(key, value);
        }

        public void AddAppMetadata(string key, object value)
        {            
            var metadataDict = app_metadata as IDictionary<string, object>;
            metadataDict.Add(key, value);
        }
    }
}
