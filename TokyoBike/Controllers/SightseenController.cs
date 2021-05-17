using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokyoBike.Helpers;
using TokyoBike.Models;
using TokyoBike.Models.DbModels;

namespace TokyoBike.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class SightseenController : Controller
    {
        ApplicationContext appCtx;
        public SightseenController(ApplicationContext ctx)
        {
            appCtx = ctx;
        }

        [HttpGet]
        public IActionResult Get()
        {           
            IEnumerable<Sightseen> ss = appCtx.Sightseens.ToList();
            return Json(ss);
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
           
            Sightseen sightseen = appCtx.Sightseens.FirstOrDefault(s => s.Id == id);
            if (sightseen != null)
            {
                return Json(sightseen);
            }
            return BadRequest(new { errorText = "Invalid username or password." });
        }
        [HttpPost("rate")]
        public void Rate(int Sightseenid, int rate)
        {
            Sightseen sightseen = appCtx.Sightseens.FirstOrDefault(s => s.Id == Sightseenid);
            User user = (User)HttpContext.Items["User"];         
            if (user.Statistics.Rates.Where(s => s.SightseenId == Sightseenid).FirstOrDefault() == null)
            {
                UserRate ur = new UserRate { StatisticsId = (int)user.StatisticsId, Rate = rate, SightseenId = sightseen.Id, Sightseen = sightseen };
                user.Statistics.Rates.Add(ur);
                sightseen.Count++;
                sightseen.BaseRate += rate;
            }
            else
            {
                sightseen.BaseRate -= (int)user.Statistics.Rates.Where(s => s.SightseenId == Sightseenid).FirstOrDefault().Rate;
                sightseen.BaseRate += rate;                
            }
            user.Statistics.Rates.Where(s => s.SightseenId == Sightseenid).FirstOrDefault().Rate = rate;
            appCtx.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            appCtx.Entry(sightseen).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            appCtx.SaveChanges();            
        }

    }
}
