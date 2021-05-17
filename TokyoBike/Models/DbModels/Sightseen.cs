using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TokyoBike.Models.DbModels
{
    public class Sightseen : IPointable
    {
        public int Id { get; set; }
        public Point Point { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
        public int Count { get; set; }        
        public int BaseRate { get; set; }

        [NotMapped]
        public double UserRate { get => Count != 0 ? (double)BaseRate / (double)Count : 0; }        

    }
}
