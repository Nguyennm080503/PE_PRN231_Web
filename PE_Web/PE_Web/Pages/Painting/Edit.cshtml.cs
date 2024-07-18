using DTOS;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;

namespace PE_Web.Pages.Painting
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _paintingApiUrl;
        private readonly string TypeApiUrl;

        public WatercolorPaintingViewDto Painting { get; set; }

        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _paintingApiUrl = "https://localhost:7158/odata/WatercolorPainting";
            TypeApiUrl = "https://localhost:7158/odata/Style";
        }

        public class StyleResponse
        {
            public List<StyleDto> Value { get; set; }
        }

        public IList<StyleDto> Style { get; set; } = new List<StyleDto>();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (HttpContext.Session.GetInt32("RoleID") == 2 || HttpContext.Session.GetInt32("RoleID") == 3)
            {
                var token = HttpContext.Session.GetString("Token");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.GetAsync($"{_paintingApiUrl}?$filter=paintingId eq {id}");
                if (response.IsSuccessStatusCode)
                {
                    HttpResponseMessage responseType = await _httpClient.GetAsync(TypeApiUrl);
                    if (responseType.IsSuccessStatusCode)
                    {
                        string styleData = await responseType.Content.ReadAsStringAsync();
                        var style = JsonConvert.DeserializeObject<StyleResponse>(styleData);
                        Style = style.Value;
                    }
                    string strData = await response.Content.ReadAsStringAsync();
                    Painting = JsonConvert.DeserializeObject<WatercolorPaintingViewDto>(strData);
                    return Page();
                }
                else
                {
                    return RedirectToPage("Error");
                }
            }
            else
            {
                return RedirectToPage("/Permission");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonConvert.SerializeObject(Painting), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync($"{_paintingApiUrl}/{Painting.PaintingId}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["MessageSuccess"] = "Painting updated successfully.";
                return RedirectToPage("/Admin/Painting/Index");
            }
            else
            {
                TempData["MessageError"] = "Failed to update painting.";
                return RedirectToPage("/Error");
            }
        }
    }
}
