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
    public class SysRolePrivilegeController : EntityController<SysRolePrivilege, SysRolePrivilegeController, Guid>
    {
        public SysRolePrivilegeController(EntityContext context, ILogger<SysRolePrivilegeController> logger,IDistributedCache cache)
            : base(context, logger,cache)
        {
        }
    }
}