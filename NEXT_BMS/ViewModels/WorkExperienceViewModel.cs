using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.ViewModels
{
    public class WorkExperienceViewModel
    {
        public string Title { set; get; }
        public string FirstName { set; get; }
        public string MiddleName { set; get; }
        public string LastName { set; get; }

        public string FullName { set; get; }
        public string AmharicFullName { set; get; }
        public string Sex { set; get; }
        public int? TotalYears { set; get; }
        public int? TotalMonths { set; get; }
        public int? TotalDays { set; get; }

        public class Experiences { 
            public int EmployeeId { set; get; }
            public string  Organization { set; get; }
            public string  JobTitle { set; get; }
            public string  Position { set; get; }
            public string  EmploymentType  { set; get; }
            public string FromDate { set; get; }
            public string ToDate { set; get; }
        }
    }
}