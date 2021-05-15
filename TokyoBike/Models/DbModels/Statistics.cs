using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TokyoBike.Models.DbModels
{
    
    public class Statistics
    {
        [Key]
        public int StatisticId { get; set; }    
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public IEnumerable<Day> Days { get; set; }
        public List<UserRate> Rates { get; set; }
    }
}
