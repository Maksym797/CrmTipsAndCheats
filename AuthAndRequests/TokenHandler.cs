using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AuthAndRequests
{
    public class TokenHandler
    {
        public static string GetToken(string clientId, string clientSecret, string authority, string resourceUrl)
        {
            ClientCredential credentials = new ClientCredential(clientId, clientSecret);
            var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(authority);
            var result = authContext.AcquireTokenAsync(resourceUrl, credentials).Result;
            return result.AccessToken;
        }
    }
}
