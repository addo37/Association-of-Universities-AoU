using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData
{
    public class Fund
    {
        public int application_id { get; set; }
        public string date_submission { get; set; }
        public string valid_until { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string university { get; set; }
        public string reasons { get; set; }
        public double amount { get; set; }
        public string itemized_expenses { get; set; }
        public string planned_term { get; set; }
        public string other_info { get; set; }
        public string status { get; set; }
        public int amount_given { get; set; }
    }
}
