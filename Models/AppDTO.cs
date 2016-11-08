using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace empservice.Models
{
    public class AppDTO
    {
    }
    public class AppUser
    {
        public string EmployeeID { get; set; }
        public string PersonID { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string Email { get; set; }
    }
    public class TokenDTO
    {
        public string Token { get; set; }
        public string StatusCode { get; set; }
    }
    public class Employee : AppUser
    {
        public string ManagerID { get; set; }
        public string ManagerName{ get; set; }
        public string MobileNo { get; set; }
        public string OfficeExt { get; set; }
        public string Extension { get; set; }
        public int Reports { get; set; }
    }
    public class UserDetail
    {
        // Properties
        public string personAddress1 { get; set; }

        public string personCounty { get; set; }

        public string personDepartment { get; set; }

        public string personEmail { get; set; }

        public string personFirstName { get; set; }

        public string personJobDescription { get; set; }

        public string personJobTitle { get; set; }

        public string personLocation { get; set; }

        public string personMobileNumber { get; set; }

        public string personPostCode { get; set; }

        public string personSurname { get; set; }

        public string personTelephone { get; set; }

        public string personTown { get; set; }

        public string PhoneExt { get; set; }
    }

}