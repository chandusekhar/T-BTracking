using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BugTracking.HttpClients
{
    public interface IHttpClientService
    {
        //Task<ResponseResult> ResponseWithModel<Model>(object body, string path);
        //Task<ResponseResult> ResponseWithoutModel(object body, string path);
        Task<ResponseDto_Result> ResponseWithModel<Model>(object body, string path, string customBaseAddress = "");
        Task<HttpResponseMessage> DeleteConversation(string idProject);
    }
}