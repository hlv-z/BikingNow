
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class AdminController : Controller
    {
        ApplicationContext appCtx;
        public AdminController(ApplicationContext ctx)
        {
            appCtx = ctx;
        }

        [HttpPost("addBike")]
        public IActionResult AddBike([FromBody] BikeModel bm)
        {
            //TODO: Добавить кол-во великов
            Station st = appCtx.Stations.FirstOrDefault(s => s.Id == bm.StationId);
            if (st != null)
            {
                Bike bk = new Bike { StationId = bm.StationId, Station = st };
                appCtx.Bikes.Add(bk);
                appCtx.SaveChanges();
                return Ok();
            }

            return BadRequest(new { errorText = "Invalid StationId" });
        }
        [HttpDelete("deleteBike/{id}")]
        public IActionResult DeleteBike(int id)
        {
            Bike b = appCtx.Bikes.FirstOrDefault(b => b.Id == id);
            if (b != null)
            {
                appCtx.Bikes.Remove(b);
                appCtx.SaveChanges();
                return Ok();
            }

            return BadRequest(new { errorText = "Invalid BikeId" });

        }

        [HttpPost("addSightseen")]
        public IActionResult AddSightseen([FromBody] SightseenModel ssm)
        {
            Sightseen ss = new Sightseen
            {
                BaseRate = 0,
                Count = 0,
                Discription = ssm.Discription,
                ImagePath = ssm.ImagePath,
                Name = ssm.Name,
                Point = new Point { X = (float)ssm.X, Y = (float)ssm.Y },
                Type = "sightseen"
            };
            appCtx.Sightseens.Add(ss);
            appCtx.SaveChanges();
            return Ok();
        }        

        [HttpPost("editSightseen")]
        public IActionResult EditSightseen(SightseenModel ssm)
        {
            Sightseen ss = appCtx.Sightseens.FirstOrDefault(s => s.Id == ssm.Id);
            if(ss != null)
            {
                ss.Name = ssm.Name == null ? ss.Name : ssm.Name;
                ss.Point = new Point { X = ssm.X == null? ss.Point.X : (float)ssm.X, Y = ssm.Y == null ? ss.Point.Y : (float)ssm.Y };
                ss.Discription = ssm.Discription == null ? ss.Discription : ssm.Discription;
                appCtx.Entry(ss).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                appCtx.SaveChanges();
                return Ok();
            }
            return BadRequest(new { errorText = "Invalid SightseenId" });

        }
        [HttpDelete("deleteSightseen/{id}")]
        public IActionResult DeleteSightseen(int id)
        {
            Sightseen ss = appCtx.Sightseens.FirstOrDefault(s => s.Id == id);
            if (ss != null)
            {
                appCtx.Sightseens.Remove(ss);
                appCtx.SaveChanges();
                return Ok();
            }

            return BadRequest(new { errorText = "Invalid SightseenId" });

        }
        [HttpPost("addStation")]
        public IActionResult AddStation([FromBody]StationModel stm)
        {
            Station st = new Station
            {
                District = stm.District,
                Type = "station",
                Point = new Point { X = (float)stm.X, Y = (float)stm.Y }
            };
            appCtx.Stations.Add(st);
            appCtx.SaveChanges();
            return Ok();
        }
        [HttpPost("editStation")]
        public IActionResult EditStation([FromBody]StationModel stm)
        {
            Station st = appCtx.Stations.FirstOrDefault(s => s.Id == stm.Id);
            if (st != null)
            {               
                st.Point = new Point { X = stm.X == null ? st.Point.X : (float)stm.X, Y = stm.Y == null ? st.Point.Y : (float)stm.Y };
                st.District = stm.District == null ? st.District : stm.District;
                appCtx.Entry(st).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                appCtx.SaveChanges();
                return Ok();
            }
            return BadRequest(new { errorText = "Invalid StationId" });
        }
        [HttpDelete("deleteStation/{id}")]
        public IActionResult DeleteStation(int id)
        {
            Station st = appCtx.Stations.FirstOrDefault(st => st.Id == id);
            if (st != null)
            {
                appCtx.Stations.Remove(st);
                appCtx.SaveChanges();
                return Ok();
            }

            return BadRequest(new { errorText = "Invalid StationId" });

        }

    }
}
