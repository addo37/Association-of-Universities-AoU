using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData
{
    public class Contribution
    {
        public int application_id { get; set; }
        public string date_submission { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string university { get; set; }
        public string attribute { get; set; }
        public int amount { get; set; }
    }
}
