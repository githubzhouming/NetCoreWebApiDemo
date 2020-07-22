﻿using System;
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
    public class ActionAccessInfoController : EntityController<ActionAccessInfo, ActionAccessInfoController, Guid>
    {
        public ActionAccessInfoController(EntityContext context, ILogger<ActionAccessInfoController> logger,IDistributedCache cache)
            : base(context, logger,cache)
        {
        }
    }
}