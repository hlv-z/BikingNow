using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokyoBike.Models.DbModels;

namespace TokyoBike.Models.ResponseModels
{
    public class UserAccountResponse
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public IEnumerable<Day> Today { get; set; }
        public IEnumerable<Day> Week { get; set; }
        public IEnumerable<Day> Month { get; set; }
        public IEnumerable<Day> Year { get; set; }

    }
}
