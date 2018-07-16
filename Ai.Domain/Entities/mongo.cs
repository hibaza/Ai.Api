using System;
using System.Collections.Generic;
using System.Text;

namespace Ai.Domain.Entities
{
    public class mongodb
    {
        public string config { get; set; }
        public string para { get; set; }
        public string filter { get; set; }
        public string projection { get; set; }
        public int limit { get; set; }
        public string order { get; set; }
    }
}
