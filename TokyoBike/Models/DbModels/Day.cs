using System;

namespace TokyoBike.Models.DbModels
{
    public class Day
    {
        public int Id { get; set; }
        public int StatisticsId { get; set; }
        public long Date { get; set; }
        public double Km { get; set; }
    }
}