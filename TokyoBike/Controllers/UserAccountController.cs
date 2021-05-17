using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokyoBike.Helpers;
using TokyoBike.Models;
using TokyoBike.Models.DbModels;
using TokyoBike.Models.ResponseModels;

namespace TokyoBike.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class UserAccountController : Controller
    {
        ApplicationContext appCtx;
        public UserAccountController(ApplicationContext ctx)
        {
            appCtx = ctx;
        }
        [HttpGet]
        public IActionResult Get()
        {            
            User user = (User)HttpContext.Items["User"];
            DateTime dt = new DateTime(1970, 01, 01);
            var daysFromBd = appCtx.Days.Where(d => d.StatisticsId == (int)user.StatisticsId).ToList();
            List<Day> days = new List<Day>();
            foreach(var d in daysFromBd)
            {
                var f = dt.AddMilliseconds(d.Date).AddHours(3).Date;
                if (f.CompareTo(DateTime.Now.Date) == 0)
                {
                    days.Add(d);
                }
            }
            UserAccountResponse uar = new UserAccountResponse
            {
                Email = user.Email,
                Login = user.Login,
                Days = days
            };
            return Json(uar);
        }

        [HttpGet("{timePeriod}")]
        public IActionResult Get(string timePeriod)
        {
            User user = (User)HttpContext.Items["User"];
            DateTime dt = new DateTime(1970, 01, 01);
            var daysFromBd = appCtx.Days.Where(d => d.StatisticsId == (int)user.StatisticsId).ToList();
            List<Day> days = new List<Day>();
            switch (timePeriod) {
                case "day":
                    foreach (var d in daysFromBd)
                    {
                        var f = dt.AddMilliseconds(d.Date).AddHours(3).Date;
                        if (f.CompareTo(DateTime.Now.Date) == 0)
                        {
                            days.Add(d);
                        }
                    }
                    break;
                case "week":
                    foreach (var d in daysFromBd)
                    {
                        var f = dt.AddMilliseconds(d.Date).AddHours(3).Date;
                        if (f.CompareTo(DateTime.Now.Date) <= 0 && f.CompareTo(DateTime.Now.AddDays(-7))>= 0)
                        {
                            days.Add(d);
                        }
                    }                    
                    break;
                case "month":
                    foreach (var d in daysFromBd)
                    {
                        var f = dt.AddMilliseconds(d.Date).AddHours(3).Date;
                        if (f.CompareTo(DateTime.Now.Date) <= 0 && f.CompareTo(DateTime.Now.AddMonths(-1)) >= 0)
                        {
                            days.Add(d);
                        }
                    }
                    break;
                case "year":
                    foreach (var d in daysFromBd)
                    {
                        var f = dt.AddMilliseconds(d.Date).AddHours(3).Date;
                        if (f.CompareTo(DateTime.Now.Date) <= 0 && f.CompareTo(DateTime.Now.AddYears(-1)) >= 0)
                        {
                            days.Add(d);
                        }
                    }
                    break;
            }
             
            UserAccountResponse uar = new UserAccountResponse
            {
                Email = user.Email,
                Login = user.Login,
                Days = days
            };
            return Json(uar);
        }
    }
}
