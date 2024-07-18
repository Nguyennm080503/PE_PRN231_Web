using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using DTOS;
using Newtonsoft.Json;

namespace PE_Web.Pages.Painting
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient httpClient;
        private string PaintingApiUrl;

        public class PaintingResponse
        {
            public List<WatercolorPaintingViewDto> Value { get; set; }
        }

        public IList<WatercolorPaintingViewDto> Paintings { get; set; } = new List<WatercolorPaintingViewDto>();

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            PaintingApiUrl = "https://localhost:7158/odata/WatercolorPainting";
        }

        public async Task<IActionResult> OnGetAsync(string searchString)
        {
            if (HttpContext.Session.GetInt32("RoleID") == 2 || HttpContext.Session.GetInt32("RoleID") == 3)
            {
                var token = HttpContext.Session.GetString("Token");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response;
                if (!string.IsNullOrEmpty(searchString))
                {
                    if (int.TryParse(searchString, out int year))
                    {
                        string filter = $"$filter=PublishYear eq {year}";
                        response = await httpClient.GetAsync($"{PaintingApiUrl}?$orderby=CreatedDate desc&{filter}");
                    }
                    else
                    {
                        string filter = $"$filter=contains(PaintingAuthor, '{searchString}')";
                        response = await httpClient.GetAsync($"{PaintingApiUrl}?$orderby=CreatedDate desc&{filter}");
                    }
                }
                else
                {
                    response = await httpClient.GetAsync($"{PaintingApiUrl}?$orderby=CreatedDate desc");
                }

                if (response.IsSuccessStatusCode)
                {
                    string strData = await response.Content.ReadAsStringAsync();
                    var paintingResponse = JsonConvert.DeserializeObject<PaintingResponse>(strData);
                    Paintings = paintingResponse.Value;
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

    }
}
