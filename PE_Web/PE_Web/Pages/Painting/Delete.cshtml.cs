using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using DTOS;

namespace PE_Web.Pages.Painting
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient httpClient;
        private string PaintingApiUrl;

        public WatercolorPaintingViewDto Painting { get; set; }

        public class PaintingResponse
        {
            public List<WatercolorPaintingViewDto> Value { get; set; }
        }

        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            PaintingApiUrl = "https://localhost:7158/odata/WatercolorPainting";
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (HttpContext.Session.GetInt32("RoleID") == 2 || HttpContext.Session.GetInt32("RoleID") == 3)
            {
                var token = HttpContext.Session.GetString("Token");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.GetAsync($"{PaintingApiUrl}?$filter=PaintingId eq '{id}'");
                if (response.IsSuccessStatusCode)
                {
                    string strData = await response.Content.ReadAsStringAsync();
                    var painting = JsonConvert.DeserializeObject<PaintingResponse>(strData);
                    Painting = painting.Value[0];
                    return Page();
                }
                else
                {
                    return RedirectToPage("/Error");
                }
            }
            else
            {
                return RedirectToPage("/Permission");
            }
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var token = HttpContext.Session.GetString("Token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await httpClient.DeleteAsync($"{PaintingApiUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Painting/Index");
            }
            else
            {
                return RedirectToPage("/Error");
            }
        }
    }
}
