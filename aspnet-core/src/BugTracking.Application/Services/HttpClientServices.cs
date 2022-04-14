using BugTracking.HttpClients;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Http;

namespace BugTracking.Services
{
    [RemoteService(IsEnabled = false)]
    public class HttpClientService : BugTrackingAppService, IHttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string delConvesation = "api/v1/conversation/";

        public HttpClientService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<ResponseDto_Result> ResponseWithModel<Model>(object body, string path, string customBaseAddress = "")
        {
            var json = JsonConvert.SerializeObject(body);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = customBaseAddress != "" ? new Uri(customBaseAddress) : new Uri(_configuration.GetSection("App")["SelfUrl"]);
            var response = await client.PostAsync(path, httpContent);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = new ResponseDto_Result
            {
                Success = response.IsSuccessStatusCode
            };
            if (response.IsSuccessStatusCode)
            {
                result.Data = JsonConvert.DeserializeObject<Model>(jsonResponse);
            }
            else
            {

                result.Data = JsonConvert.DeserializeObject<RemoteServiceErrorResponse>(jsonResponse);
            }
            return result;
        }
        public async Task<HttpResponseMessage> DeleteConversation(string idProject)
        {
            var token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IkQ4QkY3NDJFNEUwQUVFRUVBRTM5OThEMkREOTIxRDY4IiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2MzQyOTU3ODIsImV4cCI6MTY2NTgzMTc4MiwiaXNzIjoiaHR0cHM6Ly9hY2NvdW50LnRwb3MuZGV2IiwiYXVkIjpbIkFjY291bnRBcHAiLCJURGVza0FwcCIsIlRTaG9wQXBwIl0sImNsaWVudF9pZCI6IlREZXNrQXBwIiwic3ViIjoiMTU2MDQxODI1MTgxMDkzMCIsImF1dGhfdGltZSI6MTYzNDI5NTc4MSwiaWRwIjoibG9jYWwiLCJwaG9uZV9udW1iZXIiOiIrODQ5Nzg3NDQ0NzUiLCJwaG9uZV9udW1iZXJfdmVyaWZpZWQiOiJUcnVlIiwiZW1haWwiOiJuZ3V5ZW50aGFpaG9hNDc5N0BnbWFpbC5jb20iLCJuYW1lIjoiKzg0OTc4NzQ0NDc1IiwiaWF0IjoxNjM0Mjk1NzgyLCJzY29wZSI6WyJBY2NvdW50QXBwIiwiZW1haWwiLCJvcGVuaWQiLCJwaG9uZSIsInByb2ZpbGUiLCJURGVza0FwcCIsIlRTaG9wQXBwIiwib2ZmbGluZV9hY2Nlc3MiXSwiYW1yIjpbInB3ZCJdfQ.NKbl_PFqXG4vNB7X6oJpbHUx7MW5vmfxIo9cYBqidy4tUrHb86aA3YxtRUCcUoNn28kOTFBK5lqd95hgxfKvtNEJKMg7_lM1AfAgONZ2mRJhi7nCpOnK3HwqrD-cLXf3nnb4wZa-CudSC2R653FyYkVGipL8ZGtb-NZgjXgRt7CYK-6AXHue7--vabJyz3Sy-qm1WWx1tyCSqfVB9zP_JYGtcDbcKaDwEaI3g4If5Ua3ZqZ1v-49sauLIvHIG7dIHJKmUDQiuQwVXlTyO2DCQ8viBHtOACBIfKznO69zwF9B8qOpgiw2Vuf_SgaWpDyrPxK6ykXJBzhFQYqD6wVtIQ";
            var client = _httpClientFactory.CreateClient();
            var url = _configuration["AuthServer:TDesk"];
            var uri = url + delConvesation + idProject;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync(uri);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response;
            }
            else throw new Exception(response.RequestMessage.ToString());
        }

    }
}