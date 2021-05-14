using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TokyoBike.Models.DbModels
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Email { get; set; } 
        public int StatisticsId { get; set; }
        public Statistics Statistics { get; set; }
    }
}