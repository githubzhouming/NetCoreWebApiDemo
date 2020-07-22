using System;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebAPI.Auth;
using WebAPI.Common;
using WebAPI.CustomModels;
using WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using WebAPI.DBContexts;
using ZM.Extensions.EntityFrameworkCoreEx;
using ZM.Extensions.DataEx;
using ZM.Extensions.JsonConvertEx;
using ZM.Extensions.HttpContextEx;

namespace WebAPI.Controllers
{
    [ApiController]
    // [Authorize]
    public abstract partial class EntityController<T, U, KeyType> : ControllerBase where T : MyEntityBase,new() where U : ControllerBase where KeyType : struct
    {
        private readonly DbContext _context;
        private readonly ILogger<U> _logger;
        private readonly IDistributedCache _cache;

        public EntityController(EntityContext context, ILogger<U> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public DbContext getContext()
        {
            return _context;
        }

        private System.Data.DataTable getDataByRight(SqlAdvancedSelect sqlAdvancedSelect, T Tobj, bool isRight)
        {
            var userId = this.HttpContext.getUserToken().userid;
            var result = DataServices.getDataByRight(_context, sqlAdvancedSelect, userId, Tobj, isRight);
            return result;
        }

        [HttpGet("unright/getAll")]
        public virtual ActionResult  getAllEntity()
        {
            return getAllEntityDefault(false);
        }
        [HttpGet("right/getAll")]
        public virtual ActionResult getAllEntityRight()
        {
            return getAllEntityDefault(true);
        }

        private ActionResult getAllEntityDefault(bool isRight)
        {
            CustomResult customResult = new CustomResult();
            try
            {
                _logger.LogInformation($"{(isRight ? "right" : "unright")}/getAll");
                var Tobj = Activator.CreateInstance<T>();
                SqlAdvancedSelect sqlAdvancedSelect = new SqlAdvancedSelect();
                sqlAdvancedSelect.startIndex = 0;
                sqlAdvancedSelect.endIndex = int.MaxValue;

                System.Data.DataTable result = getDataByRight(sqlAdvancedSelect, Tobj, isRight);

                customResult.resultCode = 0;
                customResult.resultBody = result.ToEnumerable<T>();
                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }
        }



        [HttpGet("unright/get/{id}")]
        public virtual ActionResult getEntityById(KeyType id)
        {
            return  getEntityByIdDefault(id, false);
        }

        [HttpGet("right/get/{id}")]
        public virtual ActionResult getEntityByIdRight(KeyType id)
        {
            return  getEntityByIdDefault(id, true);
        }

        private ActionResult getEntityByIdDefault(KeyType id, bool isRight)
        {
            CustomResult customResult = new CustomResult();
            try
            {
                _logger.LogInformation($"{(isRight ? "right" : "unright")}/get/{id}");
                var Tobj = Activator.CreateInstance<T>();
                SqlAdvancedSelect sqlAdvancedSelect = new SqlAdvancedSelect();
                sqlAdvancedSelect.startIndex = 0;
                sqlAdvancedSelect.endIndex = 1;
                sqlAdvancedSelect.whereStr = $"{Tobj.getIdName()}='{id}'";
                System.Data.DataTable result = getDataByRight(sqlAdvancedSelect, Tobj, isRight);
                customResult.resultCode = 0;
                customResult.resultBody = result.ToEnumerable<T>();//result.SerializeObject();
                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }
        }



        [HttpPost("unright/create")]
        public virtual async Task<IActionResult> createEntity(T item)
        {
            return await createEntityDefault(item, false);
        }
        [HttpPost("right/create")]
        public virtual async Task<IActionResult> createEntityRight(T item)
        {
            return await createEntityDefault(item, true);
        }

        private async Task<IActionResult> createEntityDefault(T item, bool isRight)
        {
            CustomResult customResult = new CustomResult();
            try
            {
                _logger.LogInformation($"{(isRight ? "right" : "unright")}/create/{item.SerializeObject()}");
                var userId = this.HttpContext.getUserToken().userid;
                if (isRight)
                {
                    var checkResult = RightHelper.checkCreateRight(userId, item.getTableName());
                    if (!checkResult)
                    {
                        customResult.resultCode = ResultCodeEnum.InvalidTableRight;
                        customResult.resultBody = $"userid:{userId},User does not have permission to create";
                        return BadRequest(customResult);
                    }
                }

                var entityEntry = _context.Entry(item);
                entityEntry.State = EntityState.Added;
                var result = await _context.saveChangesAsyncClientWin();
                customResult.resultCode = 0;
                customResult.resultBody = entityEntry.Entity;
                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }

        }

        [HttpPost("unright/createMulti")]
        public virtual async Task<IActionResult> createMultiEntity(List<T> items)
        {
            return await createMultiEntityDefault(items, false);
        }
        [HttpPost("right/createMulti")]
        public virtual async Task<IActionResult> createMultiEntityRight(List<T> items)
        {
            return await createMultiEntityDefault(items, true);
        }

        private async Task<IActionResult> createMultiEntityDefault(List<T> items, bool isRight)
        {
            CustomResult customResult = new CustomResult();
            List<T> entityLsit = new List<T>();
            try
            {
                _logger.LogInformation($"{(isRight ? "right" : "unright")}/createMulti/{items.SerializeObject()}");
                var userId = this.HttpContext.getUserToken().userid;
                if (isRight)
                {
                    var checkResult = RightHelper.checkCreateRight(userId, items[0].getTableName());
                    if (!checkResult)
                    {
                        customResult.resultCode = ResultCodeEnum.InvalidTableRight;
                        customResult.resultBody = $"userid:{userId},User does not have permission to create";
                        return BadRequest(customResult);
                    }
                }

                foreach (var item in items)
                {
                    if (item.getId() == null || string.IsNullOrEmpty(item.getId().ToString()))
                    {
                        customResult.resultCode = ResultCodeEnum.InvalidParameter;
                        customResult.resultBody = "Id is empty";
                        return BadRequest(customResult);
                    }
                    var entityEntry = _context.Entry(item);
                    entityEntry.State = EntityState.Added;
                    entityLsit.Add(entityEntry.Entity);
                }

                var result = await _context.saveChangesAsyncClientWin();
                customResult.resultCode = 0;
                customResult.resultBody = entityLsit;
                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }
        }



        [HttpPost("unright/update")]
        public virtual async Task<IActionResult> updateEntity(T item)
        {
            return await updateEntityDefault(item, false);
        }

        [HttpPost("right/update")]
        public virtual async Task<IActionResult> updateEntityRight(T item)
        {
            return await updateEntityDefault(item, true);
        }

        private async Task<IActionResult> updateEntityDefault(T item, bool isRight)
        {
            CustomResult customResult = new CustomResult();
            try
            {
                _logger.LogInformation($"{(isRight ? "right" : "unright")}/update/{item.SerializeObject()}");
                var userId = this.HttpContext.getUserToken().userid;
                if (isRight)
                {
                    List<T> entityList = new List<T>();
                    entityList.Add(item);
                    var checkResult = RightHelper.checkWriteTableRigth(userId, entityList);
                    if (!string.IsNullOrEmpty(checkResult))
                    {
                        customResult.resultCode = ResultCodeEnum.InvalidTableRight;
                        customResult.resultBody = $"userid:{userId},{checkResult}";
                        return BadRequest(customResult);
                    }
                }


                if (item.getId() == null || string.IsNullOrEmpty(item.getId().ToString()))
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "Id is empty";
                    return BadRequest(customResult);
                }

                var oldData = _context.Find<T>(item.getId());
                item.setObjectValue(ref oldData);

                var entityEntry = _context.Entry(oldData);

                entityEntry.State = EntityState.Modified;
                var result = await _context.saveChangesAsyncClientWin();
                customResult.resultCode = 0;
                customResult.resultBody = entityEntry.Entity;
                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }
        }

        [HttpPost("unright/updateMulti")]
        public virtual async Task<IActionResult> updateMultiEntity(List<T> items)
        {
            return await updateMultiEntityDefault(items, false);
        }
        [HttpPost("right/updateMulti")]
        public virtual async Task<IActionResult> updateMultiEntityRight(List<T> items)
        {
            return await updateMultiEntityDefault(items, true);
        }
        private async Task<IActionResult> updateMultiEntityDefault(List<T> items, bool isRight)
        {
            CustomResult customResult = new CustomResult();
            try
            {
                List<T> entityLsit = new List<T>();
                _logger.LogInformation($"{(isRight ? "right" : "unright")}/updateMulti/{items.SerializeObject()}");
                var userId = this.HttpContext.getUserToken().userid;
                if (isRight)
                {
                    var checkResult = RightHelper.checkWriteTableRigth(userId, items);
                    if (!string.IsNullOrEmpty(checkResult))
                    {
                        customResult.resultCode = ResultCodeEnum.InvalidTableRight;
                        customResult.resultBody = $"userid:{userId},{checkResult}";
                        return BadRequest(customResult);
                    }
                }

                foreach (var item in items)
                {
                    if (item.getId() == null || string.IsNullOrEmpty(item.getId().ToString()))
                    {
                        customResult.resultCode = ResultCodeEnum.InvalidParameter;
                        customResult.resultBody = "Id is empty";
                        return BadRequest(customResult);
                    }
                    var oldData = _context.Find<T>(item.getId());
                    item.setObjectValue(ref oldData);
                    var entityEntry = _context.Entry(oldData);
                    entityEntry.State = EntityState.Modified;
                    entityLsit.Add(entityEntry.Entity);
                }

                var result = await _context.saveChangesAsyncClientWin();
                customResult.resultCode = 0;
                customResult.resultBody = entityLsit;
                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }
        }

        [HttpPost("unright/delete")]
        public virtual async Task<IActionResult> deleteEntity(T item)
        {
            return await deleteEntityDefault(item, false);
        }
        [HttpPost("right/delete")]
        public virtual async Task<IActionResult> deleteEntityRight(T item)
        {
            return await deleteEntityDefault(item, true);
        }
        private async Task<IActionResult> deleteEntityDefault(T item, bool isRight)
        {
            CustomResult customResult = new CustomResult();
            try
            {
                _logger.LogInformation($"{(isRight ? "right" : "unright")}/delete/{item.SerializeObject()}");
                var userId = this.HttpContext.getUserToken().userid;
                if (isRight)
                {
                    List<T> entityList = new List<T>();
                    entityList.Add(item);
                    var checkResult = RightHelper.checkDeleteTableRigth(userId, entityList);
                    if (!string.IsNullOrEmpty(checkResult))
                    {
                        customResult.resultCode = ResultCodeEnum.InvalidTableRight;
                        customResult.resultBody = $"userid:{userId},{checkResult}";
                        return BadRequest(customResult);
                    }
                }

                if (item.getId() == null || string.IsNullOrEmpty(item.getId().ToString()))
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = "Id is empty";
                    return BadRequest(customResult);
                }
                var oldData = _context.Find<T>(item.getId());
                if (oldData == null)
                {
                    customResult.resultCode = ResultCodeEnum.InvalidParameter;
                    customResult.resultBody = $"Data is not exists. Id:{item.getId() }";
                    return BadRequest(customResult);
                }
                _context.Entry(oldData).State = EntityState.Deleted;
                var result = await _context.saveChangesAsyncClientWin();
                customResult.resultCode = 0;
                customResult.resultBody = result;
                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }
        }


        [HttpPost("unright/deleteMulti")]
        public virtual async Task<IActionResult> deleteMultiEntity(List<T> items)
        {
            return await deleteMultiEntityDefault(items, false);
        }

        [HttpPost("right/deleteMulti")]
        public virtual async Task<IActionResult> deleteMultiEntityRight(List<T> items)
        {
            return await deleteMultiEntityDefault(items, true);
        }
        private async Task<IActionResult> deleteMultiEntityDefault(List<T> items, bool isRight)
        {
            CustomResult customResult = new CustomResult();
            try
            {
                _logger.LogInformation($"{(isRight ? "right" : "unright")}/deleteMulti/{items.SerializeObject()}");
                var userId = this.HttpContext.getUserToken().userid;
                if (isRight)
                {
                    var checkResult = RightHelper.checkDeleteTableRigth(userId, items);
                    if (!string.IsNullOrEmpty(checkResult))
                    {
                        customResult.resultCode = ResultCodeEnum.InvalidTableRight;
                        customResult.resultBody = $"userid:{userId},{checkResult}";
                        return BadRequest(customResult);
                    }
                }
                foreach (var item in items)
                {
                    if (item.getId() == null || string.IsNullOrEmpty(item.getId().ToString()))
                    {
                        customResult.resultCode = ResultCodeEnum.InvalidParameter;
                        customResult.resultBody = "Id is empty";
                        return BadRequest(customResult);
                    }
                    var oldData = _context.Find<T>(item.getId());
                    if (oldData == null)
                    {
                        customResult.resultCode = ResultCodeEnum.InvalidParameter;
                        customResult.resultBody = $"Data is not exists. Id:{item.getId() }";
                        return BadRequest(customResult);
                    }
                    _context.Entry(oldData).State = EntityState.Deleted;
                }

                var result = await _context.saveChangesAsyncClientWin();
                customResult.resultCode = 0;
                customResult.resultBody = result;
                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }
        }



        [HttpPost("unright/sqlAdvancedSelect")]
        public virtual ActionResult getAdvanced(SqlAdvancedSelect item)
        {
            return getAdvancedDefault(item, false);
        }

        [HttpPost("right/sqlAdvancedSelect")]
        public virtual ActionResult getAdvancedRight(SqlAdvancedSelect item)
        {
            return getAdvancedDefault(item, true);
        }

        private ActionResult getAdvancedDefault(SqlAdvancedSelect sqlAdvancedSelect, bool isRight)
        {
            CustomResult customResult = new CustomResult();
            try
            {
                _logger.LogInformation($"{(isRight ? "right" : "unright")}/getAdvanced");
                var Tobj = Activator.CreateInstance<T>();
                System.Data.DataTable result = getDataByRight(sqlAdvancedSelect, Tobj, isRight);
                customResult.resultCode = 0;
                customResult.resultBody=result.ToEnumerable<T>();
                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }
        }


        [HttpGet("admin/clearUserTableRight/{id}")]
        public virtual ActionResult clearUserTableRight(Guid id)
        {
            CustomResult customResult = new CustomResult();
            try
            {
                _logger.LogInformation($"admin/clearUserTableRight/{id}");
                RightHelper.clearUserTableRight(id);
                return Ok(customResult);
            }
            catch (Exception ex)
            {
                customResult.resultCode = ResultCodeEnum.Exception;
                customResult.resultBody = ex.ToString();
                return BadRequest(customResult);
            }

        }
    }
}