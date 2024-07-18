using DTOS;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PE_Web.Pages.Painting
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _paintingApiUrl;
        private readonly string TypeApiUrl;

        [BindProperty]
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

        public class PaintingResponse
        {
            public  List<WatercolorPaintingViewDto> Value { get; set; }
        }

        public IList<StyleDto> Style { get; set; } = new List<StyleDto>();


        private async Task LoadDataStyle()
        {
            var token = HttpContext.Session.GetString("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage responseType = await _httpClient.GetAsync(TypeApiUrl);

            if (responseType.IsSuccessStatusCode)
            {
                string strData = await responseType.Content.ReadAsStringAsync();
                var style = JsonConvert.DeserializeObject<StyleResponse>(strData);
                Style = style.Value;
            }
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (HttpContext.Session.GetInt32("RoleID") == 2 || HttpContext.Session.GetInt32("RoleID") == 3)
            {
                var token = HttpContext.Session.GetString("Token");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.GetAsync($"{_paintingApiUrl}?$filter=PaintingId eq '{id}'");
                if (response.IsSuccessStatusCode)
                {
                    await LoadDataStyle();
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


        public async Task<IActionResult> OnPostAsync()
        {
            var paintingUpdateDto = new WatercolorPaintingUpdate
            {
                PaintingName = Painting.PaintingName,
                PaintingDescription = Painting.PaintingDescription,
                PaintingAuthor = Painting.PaintingAuthor,
                Price = Painting.Price,
                PublishYear = Painting.PublishYear,
                StyleId = Painting.StyleID
            };
            var token = HttpContext.Session.GetString("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonConvert.SerializeObject(paintingUpdateDto), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync($"{_paintingApiUrl}/{Painting.PaintingId}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["MessageSuccess"] = "Painting updated successfully.";
                await LoadDataStyle();
                return Page();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                string strData = await response.Content.ReadAsStringAsync();
                await LoadDataStyle();
                try
                {
                    var jsonDoc = JsonDocument.Parse(strData);
                    var root = jsonDoc.RootElement;
                    var error = root.GetProperty("error");
                    var message = error.GetProperty("message").GetString();
                    TempData["MessageError"] = message;
                }
                catch (Exception ex)
                {
                    TempData["MessageError"] = "An error occurred while parsing the error response.";
                }
                return Page();
            }
            else
            {
                TempData["MessageError"] = "Failed to update painting.";
                await LoadDataStyle();
                return Page();
            }
        }
    }
}
