using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokyoBike.Models.DbModels
{
    public class Rent
    {
        public int RentId { get; set; }
        public int BikeId { get; set; }
        public Bike Bike { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}
