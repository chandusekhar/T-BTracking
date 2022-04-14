using BugTracking.Categories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace BugTracking.Controllers
{
    [Route("api/category")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public class CategoryController : BugTrackingController
    {
        HttpContext res;
        private readonly ICategoryAppService _categoryService;
        public CategoryController(ICategoryAppService categoryService, IHttpContextAccessor _res)
        {
            _categoryService = categoryService;
            res = _res.HttpContext;
        }
        //[HttpGet("get-list")]
        //public async Task<ListResultDto<CategoryDto>> GetList()=> await _categoryService.GetListAsync();
    }
}
