using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WorkForceManagement.WEB.Controller
{
    [Route("api/[controller]")]
    public class BearerTokenController : ControllerBase
    {
        private static HttpClient client = new HttpClient();

        public BearerTokenController() : base()
        {
        }

        [HttpPost("{username}&{password}")]
        public async Task<string> GetToken(string username, string password)
        {
            var body = new Dictionary<string, string>
            {
                {"username", username },
                {"password", password},
                {"grant_type", "password" },
                {"client_id", "WorkForceManagement"},
                {"client_secret", "secret" },
                {"scope", "users offline_access WorkForceManagement" }
            };

            var content = new FormUrlEncodedContent(body);

            HttpResponseMessage response = await client.PostAsync("https://localhost:5001/connect/token", content);

            return await response.Content.ReadAsStringAsync();
        }
    }
}