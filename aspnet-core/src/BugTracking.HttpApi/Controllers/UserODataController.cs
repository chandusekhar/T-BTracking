using BugTracking.UserOData;
using BugTracking.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Uow;

namespace BugTracking.Controllers
{
    [RemoteService]
    [IgnoreAntiforgeryToken]
    [ODataAttributeRouting]
    [UnitOfWork]

    public class UserODataController : ODataController
    {
        private readonly IUserODataAppService _userODataAppService;
        // GET /User(1)/Supplier
        public UserODataController(IUserODataAppService userODataAppService)
        {
            _userODataAppService = userODataAppService;
        }
       
        [EnableQuery]
        public Task<IQueryable<UserDto>> Get([FromODataUri] Guid key)
        {
            return _userODataAppService.GetListUserByProject(key);
        }
    }
}
