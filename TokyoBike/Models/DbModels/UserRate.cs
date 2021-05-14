using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokyoBike.Models.DbModels
{
    public class UserRate
    {
        public int Id { get; set; }
        public int StatisticsId { get; set; }        
        public int SightseenId { get; set; }
        public Sightseen Sightseen { get; set; }
        public int? Rate { get; set; }
    }
}
