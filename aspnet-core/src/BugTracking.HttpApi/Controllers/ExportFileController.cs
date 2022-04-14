using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace BugTracking.Controllers
{
    [RemoteService]
    [Controller]
    [Area("project")]
    [Route("api/export")]
    public class ExportFileController : BugTrackingController
    {

    }
}
