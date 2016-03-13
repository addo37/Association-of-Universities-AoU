using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData
{
    public class Scholarship
    {
        public int application_id{ get; set;}
        public string date_submission{ get; set;}
        public string valid_until
        { 
            get; set;
        }
        public string username
        {
            get; set;
        }
        public string name
        {
            get; set;
        }
        public string university
        {
            get; set;
        }
        public string father_name
        {
            get; set;
        }
        public string father_worktype
        {
            get; set;
        }
        public string father_workplace
        {
            get; set;
        }
        public string father_workaddress
        {
            get; set;
        }
        public string father_startdate
        {
            get; set;
        }
        public string father_enddate
        {
            get; set;
        }
        public string mother_name
        {
            get; set;
        }
        public string mother_worktype
        {
            get; set;
        }
        public string mother_workplace
        {
            get; set;
        }
        public string mother_workaddress
        {
            get; set;
        }
        public string mother_startdate
        {
            get; set;
        }
        public string mother_enddate
        {
            get; set;
        }
        public string other_enddate
        {
            get; set;
        }
        public string siblings_number
        {
            get; set;
        }
        public string siblings_position
        {
            get; set;
        }
        public double father_income
        {
            get; set;
        }
        public double mother_income
        {
            get; set;
        }
        public double other_income
        {
            get; set;
        }
        public double expenses
        {
            get; set;
        }
        public string status
        {
            get; set;
        }
        public int percentage
        {
            get; set;
        }
    }
}
