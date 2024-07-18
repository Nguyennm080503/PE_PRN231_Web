using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DTOS;
using Newtonsoft.Json;

namespace PE_Web.Pages.Painting 
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient httpClient;
        private readonly string PaintingCreateApiUrl;
        private readonly string TypeApiUrl;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            PaintingCreateApiUrl = "https://localhost:7158/odata/WatercolorPainting";
            TypeApiUrl = "https://localhost:7158/odata/Style";
        }

        public class StyleResponse
        {
            public List<StyleDto> Value { get; set; }
        }

        public IList<StyleDto> Style { get; set; } = new List<StyleDto>();

        private async Task LoadDataStyle()
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage responseType = await httpClient.GetAsync(TypeApiUrl);

                if (responseType.IsSuccessStatusCode)
                {
                    string strData = await responseType.Content.ReadAsStringAsync();
                    var style = JsonConvert.DeserializeObject<StyleResponse>(strData);
                    Style = style.Value;
                }
            }catch(Exception ex)
            {
                MessageError = ex.Message;
            }
        }   

        public async Task<IActionResult> OnGet()
        {
            if (HttpContext.Session.GetInt32("RoleID") == 3)
            {
                await LoadDataStyle();
                return Page();
            }
            else
            {
                return RedirectToPage("/Permission");
            }
        }

        [BindProperty]
        public WatercolorPaintingCreateDto WatercolorPainting { get; set; } = new WatercolorPaintingCreateDto();

        public string MessageSuccess { get; set; } = "";
        public string MessageError { get; set; } = "";

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDataStyle();
                return Page();
            }

            var paintingCreateDto = new WatercolorPaintingCreateDto
            {
                PaintingId = WatercolorPainting.PaintingId,
                PaintingName = WatercolorPainting.PaintingName,
                PaintingDescription = WatercolorPainting.PaintingDescription,
                PaintingAuthor = WatercolorPainting.PaintingAuthor,
                Price = WatercolorPainting.Price,
                PublishYear = WatercolorPainting.PublishYear,
                StyleId = WatercolorPainting.StyleId
            };

            var token = HttpContext.Session.GetString("Token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(paintingCreateDto), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(PaintingCreateApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                MessageSuccess = "Painting created successfully!";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                string strData = await response.Content.ReadAsStringAsync();
                try
                {
                    var jsonDoc = JsonDocument.Parse(strData);
                    var root = jsonDoc.RootElement;
                    var error = root.GetProperty("error");
                    var message = error.GetProperty("message").GetString();
                    MessageError = message;
                }
                catch (Exception ex)
                {
                    MessageError = "An error occurred while parsing the error response.";
                }
            }
            else
            {
                MessageError = "An error occurred while creating the painting.";
            }
            await LoadDataStyle();
            return Page();
        }
    }
}
