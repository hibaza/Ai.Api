using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ai.Domain
{
    public class Cursors
    {
        public string Before { get; set; }
        public string After { get; set; }
    }
    public class Paging
    {
        public string Next { get; set; }
        public string Previous { get; set; }
        public Cursors Cursors { get; set; }
        private int limit = 100;
        public int Limit
        {
            get
            {
                return limit;
            }
            set { limit = value; }
        }
    }
}
