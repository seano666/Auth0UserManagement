using System;
namespace Auth0UserManagement
{
    public class Auth0UserManagementConfig
    {
        public string _baseDomain { get; set; }
        public string _baseURI { get; set; }
        public string _connection { get; set; }
        public string _connectionID { get; set; }
        public string _clientID { get; set; }
        public string _clientSecret { get; set; }
        /// <summary>
        /// A config object to be passed in to the main class constructor.
        /// </summary>
        /// <param name="BaseDomain">Your Auth0 domain e.g. "myapp.Auth0.com"</param>
        /// <param name="Connection">The name of the Auth0 connection you are targeting</param>
        /// <param name="ConnectionID">The ID of the Auth0 connection you are targeting</param>
        /// <param name="ClientID">The Client ID of the application used for retreiving the access token https://auth0.com/docs/tokens/management-api-access-tokens </param>
        /// <param name="ClientSecret">The Client Secret of the application used for retreiving the access token </param>
        public Auth0UserManagementConfig(string BaseDomain, string Connection, string ConnectionID, string ClientID, string ClientSecret)
        {
            _baseDomain = BaseDomain;
            _baseURI = $"{ _baseDomain }/";
            _connection = Connection;
            _connectionID = ConnectionID;
            _clientID = ClientID;
            _clientSecret = ClientSecret;
        }
    }
}