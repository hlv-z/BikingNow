using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokyoBike.Models
{
    public class StationModel
    {
        public int Id { get; set; }
        public float? X { get; set; }
        public float? Y { get; set; }        
        public string District { get; set; }
    }
}
