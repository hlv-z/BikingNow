using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokyoBike.Models.DbModels
{
    interface IPointable
    {
        public int Id { get; set; }
        public Point Point { get; set; }
        public string Type { get; set; }

    }
}
