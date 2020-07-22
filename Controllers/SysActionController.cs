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
    public class SysActionController : EntityController<SysAction, SysActionController, Guid>
    {
        public SysActionController(EntityContext context, ILogger<SysActionController> logger,IDistributedCache cache)
            : base(context, logger,cache)
        {
        }
    }
}