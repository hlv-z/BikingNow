using LiqPay.SDK;
using LiqPay.SDK.Dto;
using LiqPay.SDK.Dto.Enums;
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
    //[Authorize]
    public class RentController : Controller
    {
        ApplicationContext appCtx;

        public RentController(ApplicationContext ctx)
        {
            appCtx = ctx;
        }

        [HttpPost("chooseRoute")]
        public IActionResult ChooseRoute([FromBody] RouteRequest rrq)
        {
            switch (rrq.Type)
            {
                case "shortest":
                    return Json(ShortRoute(rrq));
                case "tourist":
                    return Json(TouristRoute(rrq));
                case "free":
                    return Json(FreeRoute(rrq));
                default:
                    return BadRequest(new { errorMessage = "Type of route is undefined" });
            }
        }

        private IEnumerable<Point> FreeRoute(RouteRequest rrq)
        {
            int StationId = appCtx.Bikes.Where(b => b.Id == rrq.BikeId).Select(b => b.StationId).SingleOrDefault();
            Station st = appCtx.Stations.FirstOrDefault(s => s.Id == StationId);
            if (st != null)
            {
                return new List<Point> { new Point { X = st.Point.X, Y = st.Point.Y } };
            }
            return null;
        }

        private IEnumerable<Point> TouristRoute(RouteRequest rrq)
        {
            IEnumerable<Sightseen> sightseens = appCtx.Sightseens.ToList();
            int StationId = appCtx.Bikes.Where(b => b.Id == rrq.BikeId).Select(b => b.StationId).SingleOrDefault();
            Station stBegin = appCtx.Stations.FirstOrDefault(s => s.Id == StationId);
            Station stEnd = appCtx.Stations.FirstOrDefault(s => s.Id == rrq.EndStationId);
            if (stBegin != null && stEnd != null)
            {
                Point c = new Point
                {
                    X = (stBegin.Point.X + stEnd.Point.X) / 2,
                    Y = (stBegin.Point.Y + stEnd.Point.Y) / 2
                };
                double r = Math.Sqrt(Math.Pow(c.X - stBegin.Point.X, 2) + Math.Pow(c.Y - stBegin.Point.Y, 2));
                List<Point> result = new List<Point> { new Point { X = stBegin.Point.X, Y = stBegin.Point.Y } };
                foreach (Sightseen s in sightseens)
                {
                    if (isInCircle(c, s.Point, r))
                    {
                        result.Add(s.Point);
                    }
                }
                result.Add(new Point { X = stEnd.Point.X, Y = stEnd.Point.Y });
                return result;
            }

            return null;

        }

        private bool isInCircle(Point c, Point p, double r)
        {
            return Math.Pow(p.Y - c.Y, 2) + Math.Pow(p.X - c.X, 2) <= r * r;
        }

        private IEnumerable<Point> ShortRoute(RouteRequest rrq)
        {
            int StationId = appCtx.Bikes.Where(b => b.Id == rrq.BikeId).Select(b => b.StationId).SingleOrDefault();
            Station stBegin = appCtx.Stations.FirstOrDefault(s => s.Id == StationId);
            Station stEnd = appCtx.Stations.FirstOrDefault(s => s.Id == rrq.EndStationId);

            if (stBegin != null && stEnd != null)
            {
                return new List<Point> { new Point { X = stBegin.Point.X, Y = stBegin.Point.Y }, new Point { X = stEnd.Point.X, Y = stEnd.Point.Y } };
            }
            return null;
        }

        [HttpPost("startRent")]
        public IActionResult StartRent([FromBody] RentModel rm)
        {
            Rent r = appCtx.Rents.FirstOrDefault(s => s.BikeId == rm.BikeId && s.EndTime == null);
            if (r == null)
            {
                Bike b = appCtx.Bikes.FirstOrDefault(b => b.Id == rm.BikeId);
                User user = (User)HttpContext.Items["User"];
                if (b != null)
                {
                    Rent rent = new Rent
                    {
                        BikeId = rm.BikeId,
                        Bike = b,
                        UserId = user.Id,
                        User = user,
                        StartTime = DateTime.Now
                    };
                    appCtx.Rents.Add(rent);
                    appCtx.SaveChanges();
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpPost("endRent")]
        public async Task<IActionResult> EndRent([FromBody] RentModel rm)
        {
            await LiqPay();
            Rent rent = appCtx.Rents.Include(r => r.Bike).Include(r => r.User).Where(r => r.BikeId == rm.BikeId && r.EndTime == null).FirstOrDefault();
            if (rent != null)
            {
                rent.EndTime = DateTime.Now;
                appCtx.Entry(rent).State = EntityState.Modified;
                appCtx.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("LiqPay")]
        public async Task LiqPay()
        {
            
            var invoiceRequest = new LiqPayRequest
            {
                Email = "ilyaker1806@gmail.com",
                Amount = 1,
                Currency = "UAH",
                OrderId = "15",
                Action = LiqPayRequestAction.InvoiceSend,
                Language = LiqPayRequestLanguage.RU
            };

            var liqPayClient = new LiqPayClient("sandbox_i33754480935", "sandbox_H2hSOXg3sjkkhpV42jcSVTWvNTiwtjbKvfFYr2n6");
            //liqPayClient.IsCnbSandbox = true;
            var response = await liqPayClient.RequestAsync("request", invoiceRequest);


        }
        [HttpPost("liqpay-response")]
        public void LiqPayResponse()
        {

        }
    }
   
}
