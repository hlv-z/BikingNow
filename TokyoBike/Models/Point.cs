using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TokyoBike.Models
{
    [Owned]
    public class Point
    {
        public float X { get; set;}
        public float Y { get; set; }
    }
}
