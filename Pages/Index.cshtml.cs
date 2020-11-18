using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;

namespace myrazorapp.Pages
{
    [AuthorizeForScopes(Scopes = new[] { "User.Read", "Mail.Read" })]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly ITokenAcquisition _tokenGetter;
        private readonly HttpClient _httpClient;

        public string MeData;

        public string MailData;

        public IndexModel(ILogger<IndexModel> logger, ITokenAcquisition tokenGetter, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _tokenGetter = tokenGetter;
            _httpClient = clientFactory.CreateClient();
        }

        public async Task OnGet()
        {
            var accessToken = await _tokenGetter.GetAccessTokenForUserAsync(new[] { "User.Read", "Mail.Read" });

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var result = await _httpClient.GetAsync("https://graph.microsoft.com/v1.0/me");

            if (result.IsSuccessStatusCode)
            {
                MeData = await result.Content.ReadAsStringAsync();
            }
            else
            {
                MeData = $"{result.StatusCode}";
            }

            result = await _httpClient.GetAsync("https://graph.microsoft.com/beta/me/messages?$select=createdDateTime,subject,from");

            if (result.IsSuccessStatusCode)
            {
                MailData = await result.Content.ReadAsStringAsync();
            }
            else
            {
                MailData = $"{result.StatusCode}";
            }
        }
    }
}
