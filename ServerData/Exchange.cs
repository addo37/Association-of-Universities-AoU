using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData
{
    public class Exchange
    {
        public int application_id { get; set; }
        public string date_submission { get; set; }
        public string valid_until { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string university { get; set; }
        public string subjects { get; set; }
        public string term { get; set; }
        public string class_hours{ get; set; }
        public string other_info { get; set; }
        public string status { get; set; }
    }
}
