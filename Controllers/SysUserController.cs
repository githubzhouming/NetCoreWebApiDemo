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
    public class SysUserController : EntityController<SysUser, SysUserController, Guid>
    {
        public SysUserController(EntityContext context, ILogger<SysUserController> logger,IDistributedCache cache)
            : base(context, logger,cache)
        {
        }
    }
}