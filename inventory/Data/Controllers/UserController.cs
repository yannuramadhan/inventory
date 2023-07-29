using inventory.Data.Models;
using inventory.Libs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Npgsql;
using System.Data;
using System.Reflection.Emit;

namespace inventory.Data.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private lDbConn dbconn = new lDbConn();
        private BaseController bc = new BaseController();
        private lUser lp = new lUser();
        private lMessage mc = new lMessage();
        private lConvert lc = new lConvert();


    }
}
