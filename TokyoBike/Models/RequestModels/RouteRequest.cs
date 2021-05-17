using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokyoBike.Models
{
    public class RouteRequest
    {
        public string Type { get; set; }
        public int BikeId { get; set; }
        public int? EndStationId { get; set; }
    }
}
