using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPI.Models;
using Microsoft.Extensions.Caching.Distributed;
using WebAPI.DBContexts;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SysRoleTableController : EntityController<SysRoleTable, SysRoleTableController, Guid>
    {
        public SysRoleTableController(EntityContext context, ILogger<SysRoleTableController> logger,IDistributedCache cache)
            : base(context, logger,cache)
        {
        }
    }
}