using Microsoft.AspNetCore.Mvc;
using RepositoryUOW;
using RepositoryUOW.Data.Repositories;
using RepositoryUOWDomain.Entities.System;
using RepositoryUOWDomain.Shared.Enums;
using RepositoryUOWDomain.ValueObject;

namespace RepositoryUOWService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public ILogger<WeatherForecastController> _logger { get; }
        private AppDbContext dbContext { get; }

        public WeatherForecastController(ILogger<WeatherForecastController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            dbContext = appDbContext;
        }

        [HttpGet("GetUsers")]
        public ResponseResult<IList<User>> GetUsers()
        {
            RepositoryUow<User> uDate = new(dbContext);

            //var f = IpHelper.GetIpAddress();

            return uDate.AllItemsAsync();

            //return new(uDate.ErrorMessage, uDate.DbEntity().Take(100).ToList());
        }
         

        [HttpPost("SaveUser")]
        public async Task<ResponseResult<bool>> SaveUser(InsertUpdateEnum isModify, User user)
        {
            RepositoryUow<User> uDate = new(dbContext);
            
            return new()
            {
                ReturnValue = await uDate.SaveAsync(user, isModify) != null,
                ResponseStr = uDate.ErrorMessage
            };
        }
    }
} 