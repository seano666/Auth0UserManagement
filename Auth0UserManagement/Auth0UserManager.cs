using Auth0UserManagement.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Dynamic;
using System.Linq;

namespace Auth0UserManagement
{
    public class Auth0UserManager
    {
        private Auth0UserManagementConfig _config { get; set; }

        public Auth0UserManager(Auth0UserManagementConfig config)
        {
            _config = config;
        }

        private string tokenEndpoint { get { return $"{ _config._baseURI }oauth/token"; } }
        private string userImportEndpoint { get { return $"{ _config._baseURI }api/v2/jobs/users-imports"; } }
        private string userEndpoint { get { return $"{ _config._baseURI }api/v2/users"; } }

        private string _authToken { get; set; }
        private string authToken
        {
            get
            {
                if (_authToken == null)
                {
                    TokenResponse token = getToken();
                    _authToken = $"{ token.token_type } { token.access_token }";
                }
                return _authToken;
            }
        }

        private TokenResponse getToken()
        {
            RestClient client = new RestClient(tokenEndpoint);
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", $"{{\"client_id\":\"{ _config._clientID }\",\"client_secret\":\"{ _config._clientSecret }\",\"audience\":\"{ _config._baseDomain }/api/v2/\",\"grant_type\":\"client_credentials\"}}", ParameterType.RequestBody);            
            var response = client.Execute<TokenResponse>(request);
            var token = response.Data;
            return token;           
        }

        /// <summary>
        /// Search for user using the Auth0 User Search Query Syntax V2
        /// https://auth0.com/docs/users/user-search/v2/query-syntax
        /// </summary>
        /// <param name="SearchQuery">A string query of the users eg "user_name:'john smith'"</param>
        public List<User> SearchUsers(string searchQuery)
        {            
            var ep = $"{ userEndpoint }?q={ searchQuery }";
            RestClient client = new RestClient(ep);
            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("authorization", authToken);
            var response = client.Execute<List<User>>(request);
            return response.Data;            
        }

        public void GetUsers()
        {

        }

        public OperationResult InsertUsers(List<User> usersToInsert)
        {
            var result = new OperationResult();

            foreach (var user in usersToInsert)
            {
                //set random user password
                if (user.password == null)
                {
                    user.password = Utility.RandomPassword();
                }

                //set connection for new user based off of config
                if (user.connection == null)
                {
                    user.connection = _config._connection;
                }
               
                RestClient client = new RestClient(userEndpoint);
                RestRequest request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddJsonBody(user);
                request.AddHeader("authorization", authToken);
                IRestResponse<TokenResponse> response = client.Execute<TokenResponse>(request);
                if (!response.IsSuccessful)
                {
                    result.Success = false;
                    dynamic returnPayload = JsonConvert.DeserializeObject<object>(response.Content);
                    result.ErrorMessages.Add(returnPayload.message.ToString());
                }
            }
            
            return result;

        }

        public OperationResult UpdateUsers(List<User> usersToUpdate)
        {
            var result = new OperationResult();
            var userProps = usersToUpdate.First().GetType().GetProperties();

            foreach (var user in usersToUpdate)
            {
                RestClient client = new RestClient($"{ userEndpoint }/{ user.user_id }");
                RestRequest request = new RestRequest(Method.PATCH);
                request.AddHeader("authorization", authToken);

                //some of the user properties are not allowed to be updated
                //in order to remove these, we need to break apart the user object
                //and build a new one without these disallowed properties.                
                var userDict = new Dictionary<string, object>();
                var eo = new ExpandoObject();
                var eoColl = (ICollection<KeyValuePair<string, object>>)eo;

                //add user props to dictionary as needed
                foreach (var prop in userProps)
                {
                    if (prop.GetValue(user) != null && prop.Name != "user_id")
                    {
                        eoColl.Add(new KeyValuePair<string, object>(prop.Name, prop.GetValue(user)));
                    }                    
                }
                
                dynamic eoDynamic = eo;

                request.AddJsonBody(eoDynamic);
                IRestResponse response = client.Execute(request);
                if (!response.IsSuccessful)
                {
                    result.Success = false;
                    if (response.ErrorMessage == null)
                    {
                        result.ErrorMessages.Add(response.ErrorMessage);
                    }
                    else
                    {
                        dynamic returnPayload = JsonConvert.DeserializeObject<object>(response.Content);
                        result.ErrorMessages.Add(returnPayload.message.ToString());
                    }

                }
            }

            return result;
        }
        
        public bool BulkUpsertUsers(List<User> usersToImport)
        {
            var client = new RestClient($"{ _config._baseURI }api/v2/jobs/users-imports");            

            //transform list into encoded json bytes
            var usersArray = usersToImport.ToArray();            
            var serializedData = JsonConvert.SerializeObject(usersArray);            
            byte[] users = Encoding.UTF8.GetBytes(serializedData);

            var request = new RestRequest(Method.POST);

            request.AddHeader("authorization", authToken);

            request.AddParameter("connection_id", _config._connectionID);
            request.AddParameter("upsert", "true");
            request.AddFileBytes("users", users, "users.json");

            request.AddHeader("Content-Type", "multipart/form-data");

            IRestResponse response = client.Execute(request);

            bool success = response.IsSuccessful;

            return success;
        }

        public OperationResult DeleteUsers(List<string> usersToDelete)
        {
            var result = new OperationResult();

            foreach (var id in usersToDelete)
            {                
                RestClient client = new RestClient($"{ userEndpoint }/{ id }");
                RestRequest request = new RestRequest(Method.DELETE);                
                request.AddHeader("authorization", authToken);
                IRestResponse response = client.Execute(request);
                if (!response.IsSuccessful)
                {
                    result.Success = false;     
                    if (response.ErrorMessage == null)
                    {
                        result.ErrorMessages.Add(response.ErrorMessage);
                    }
                    else
                    {
                        dynamic returnPayload = JsonConvert.DeserializeObject<object>(response.Content);
                        result.ErrorMessages.Add(returnPayload.message.ToString());                        
                    }
                    
                }
            }

            return result;
        }
    }
    
}
