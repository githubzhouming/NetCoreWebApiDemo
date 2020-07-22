using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPI.DBContexts;
using WebAPI.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    // [Authorize]
    public class SysRoleController : EntityController<SysRole, SysRoleController, Guid>
    {
        public SysRoleController(EntityContext context, ILogger<SysRoleController> logger,IDistributedCache cache)
            : base(context, logger,cache)
        {
        }
    }
}