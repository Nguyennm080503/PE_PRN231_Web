using DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PE_Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient httpClient;
        private readonly string LoginApiUrl = "https://localhost:7158/login";

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            LoginDto loginDto = new LoginDto()
            {
                UserEmail = Email,
                UserPassword = Password,
            };

            var content = new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(LoginApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                string strData = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginTokenDto>(strData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (loginResponse != null)
                {
                    HttpContext.Session.SetInt32("AccountID", loginResponse.UserAccountId);
                    HttpContext.Session.SetInt32("RoleID", loginResponse.Role ?? 0);
                    HttpContext.Session.SetString("Token", loginResponse.Token);

                    switch (loginResponse.Role)
                    {
                        case 2:
                            return RedirectToPage("/Painting/Index");
                        case 3:
                            return RedirectToPage("/Painting/Index");
                        default:
                            return RedirectToPage("/Permission");
                    }
                }
                else
                {
                    ErrorMessage = "You are not allowed to access this function!";
                    return Page();
                }
            }
            else
            {
                ErrorMessage = "You are not allowed to access this function!";
                return Page();
            }
        }
    }
}
