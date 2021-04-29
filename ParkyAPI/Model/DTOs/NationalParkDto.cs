using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Model.DTOs
{
    public class NationalParkDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public byte[] Pictures { get; set; }
        public DateTime Created { get; set; }
        public DateTime Established { get; set; }
    }
}
