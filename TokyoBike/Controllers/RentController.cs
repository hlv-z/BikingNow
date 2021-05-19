using LiqPay.SDK;
using LiqPay.SDK.Dto;
using LiqPay.SDK.Dto.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using TokyoBike.Helpers;
using TokyoBike.Models;
using TokyoBike.Models.DbModels;

namespace TokyoBike.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    [Authorize]
    public class RentController : Controller
    {
        ApplicationContext appCtx;

        public RentController(ApplicationContext ctx)
        {
            appCtx = ctx;
        }

        [HttpGet("getStations")]
        public IActionResult GetStations()
        {
            List<IPointable> pointable = appCtx.Sightseens.Cast<IPointable>().ToList();
            pointable.AddRange(appCtx.Stations.ToList());
            return Json(pointable);
        }

        //"api/rent/chooseRoute"
        [HttpPost("chooseRoute")]
        public IActionResult ChooseRoute([FromBody] RouteRequest rrq)
        {
            switch (rrq.Type)
            {
                case "shortest":
                    StartRent(new RentModel { BikeId = rrq.BikeId });
                    return Json(ShortRoute(rrq));
                case "tourist":
                    StartRent(new RentModel { BikeId = rrq.BikeId });
                    return Json(TouristRoute(rrq));
                case "free":
                    StartRent(new RentModel { BikeId = rrq.BikeId });
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


        [HttpGet("checkExpireRent")]
        public IActionResult CheckExpireRent()
        {
            IEnumerable<Rent> rents = appCtx.Rents.Include(r => r.Bike).Include(r => r.User).Where(r => r.StartTime.CompareTo(DateTime.Now.AddDays(-1)) < 0).ToList();
            foreach(var r in rents)
            {
                User u = appCtx.Users.FirstOrDefault(u => u.Id == r.UserId);
                // отправитель - устанавливаем адрес и отображаемое в письме имя
                MailAddress from = new MailAddress(EmailOptions.Email, "BikingNow");
                // кому отправляем
                MailAddress to = new MailAddress(u.Email);
                // создаем объект сообщения
                MailMessage m = new MailMessage(from, to);
                // тема письма
                m.Subject = "Просроченная аренда";
                // текст письма
                m.Body = "<h2>Уважаемый клиент, время вашей аренды истекло. Будьте добры вернуть велик на место, плиз</h2>";
                // письмо представляет код html
                m.IsBodyHtml = true;
                // адрес smtp-сервера и порт, с которого будем отправлять письмо

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    // логин и пароль
                    smtp.Credentials = new NetworkCredential(EmailOptions.Email, EmailOptions.Password);
                    smtp.EnableSsl = true;
                    smtp.Send(m);
                }
            }
            return Ok();

        }

        [HttpPost("endRent")]
        public async Task<IActionResult> EndRent([FromBody] RentModel rm)
        {
            User user = (User)HttpContext.Items["User"];
            Rent rent = appCtx.Rents.Include(r => r.Bike).Include(r => r.User).Where(r => r.BikeId == rm.BikeId && r.EndTime == null).FirstOrDefault();
            long dateticks = DateTime.Now.Ticks;
            Random random = new Random();
            Day day = new Day()
            {
                Date = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds,
                Km = random.Next(5, 15),
                StatisticsId = (int)user.StatisticsId                
            };
            appCtx.Days.Add(day);
            if (rent != null)
            {
                rent.EndTime = DateTime.Now;
                appCtx.Entry(rent).State = EntityState.Modified;
                appCtx.SaveChanges();
                await LiqPay(user, rent);
                return Ok();
            }
            return BadRequest();
        }

        public async Task LiqPay(User u, Rent r)
        {
            double price = (r.EndTime - r.StartTime).Value.Hours * 30;
            price = price == 0 ? 30 : price; 
            var invoiceRequest = new LiqPayRequest
            {
                Email = u.Email,
                Amount = price,
                Currency = "UAH",
                OrderId = r.RentId.ToString(),
                Action = LiqPayRequestAction.InvoiceSend,
                Language = LiqPayRequestLanguage.RU
            };

            var liqPayClient = new LiqPayClient("sandbox_i33754480935", "sandbox_H2hSOXg3sjkkhpV42jcSVTWvNTiwtjbKvfFYr2n6");
            //liqPayClient.IsCnbSandbox = true;
            var response = await liqPayClient.RequestAsync("request", invoiceRequest);
        }        
    }
   
}
