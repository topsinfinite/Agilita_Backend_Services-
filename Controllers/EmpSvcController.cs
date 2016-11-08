using empservice.DAL;
using empservice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace empservice.Controllers
{
    public class EmpSvcController : ApiController
    {
        // GET api/empsvc
        public HttpResponseMessage GetAuthToken(string username, string pwd)
        {
            try
            {
                string val = ""; TokenDTO tokenData=null;;
                var tokenGenerator = new Firebase.TokenGenerator("8CWDFfXY250C2EM6Gb3I0g4TrxftS9QqE9O29Y3e");
                ADAuth.Service ADSvr = new ADAuth.Service();
                //val = ADSvr.ADValidateUser(username, pwd);///uncomment on depolyment to production
                 val = "true";
                if (val.ToLower().Contains("true"))
                {
                    AppUser usr = new AppUser();
                    usr = Utility.GetUser(username);
                    if (usr != null)
                    {
                        var authPayload = new Dictionary<string, object>()
                        {
                          { "uid",usr.EmployeeID  },
                          { "personID", usr.PersonID },
                          { "fullName", usr.FullName },
                          {"jobTitle",usr.JobTitle},
                          {"department",usr.Department},
                          {"gender",usr.Gender},
                          {"email",usr.Email}
                        };
                        string token = tokenGenerator.CreateToken(authPayload);
                        tokenData = new TokenDTO() { Token = token, StatusCode = "000" };
                        //return token;
                    }
                    else
                    {
                        tokenData = new TokenDTO() { Token = "NOT FOUND", StatusCode = "001" };
                        //return "NOT FOUND";
                    }
                }
                else
                {
                    tokenData = new TokenDTO() { Token = "INVALID", StatusCode = "002" };
                    
                }
                return Request.CreateResponse<TokenDTO>(HttpStatusCode.OK, tokenData);

            }
            catch (Exception ex)
            {
                Utility.WriteError("Error msg: " + ex.Message + " StackTrace: " + ex.StackTrace);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "An error occurred!!");
            }
        }

        [HttpGet]
        [ActionName("SearchEmployee")]
        public HttpResponseMessage SearchEmployee(string fname)
        {
            try
            {
                List<Employee> EmpList = new List<Employee>();
                if(!string.IsNullOrEmpty(fname))
                EmpList = Utility.SearchEmployee(fname.ToUpper());
                return Request.CreateResponse<IEnumerable<Employee>>(HttpStatusCode.OK, EmpList);
            }
            catch (Exception ex)
            {
                Utility.WriteError("Error msg: " + ex.Message + " StackTrace: " + ex.StackTrace);
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, "Not Found"); 
            }
        }
        [HttpGet]
        [ActionName("GetEmployeeDetails")]
        public HttpResponseMessage GetEmployeeDetails(string fname)
        {
            try
            {
                Employee EmpDetail = new Employee();
                if (!string.IsNullOrEmpty(fname))
                    EmpDetail = Utility.GetEmployeeDetails(fname.Trim());
                return Request.CreateResponse<Employee>(HttpStatusCode.OK, EmpDetail);
            }
            catch (Exception ex)
            {
                Utility.WriteError("Error msg: " + ex.Message + " StackTrace: " + ex.StackTrace);
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, "Not Found");
            }
        }
        [HttpGet]
        [ActionName("GetUserDetails")]
        public HttpResponseMessage GetUserDetails(string fname)
        {
            try
            {
                AppUser EmpDetail = new AppUser();
                if (!string.IsNullOrEmpty(fname))
                    EmpDetail = Utility.GetUser(fname.Trim());
                return Request.CreateResponse<AppUser>(HttpStatusCode.OK, EmpDetail);
            }
            catch (Exception ex)
            {
                Utility.WriteError("Error msg: " + ex.Message + " StackTrace: " + ex.StackTrace);
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, "Not Found");
            }
        }

        [HttpGet]
        [ActionName("FindByManagerId")]
        public HttpResponseMessage FindByManagerId(string fname)
        {
            try
            {
                List<Employee> EmpList = new List<Employee>();
                if (!string.IsNullOrEmpty(fname))
                    EmpList = Utility.FindByManagerId(fname.ToUpper());
                return Request.CreateResponse<IEnumerable<Employee>>(HttpStatusCode.OK, EmpList);
            }
            catch (Exception ex)
            {
                Utility.WriteError("Error msg: " + ex.Message + " StackTrace: " + ex.StackTrace);
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, "Not Found");
            }
        }
        // GET api/empsvc/5
        
        // POST api/empsvc
        public void Post([FromBody]string value)
        {
        }

        // PUT api/empsvc/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/empsvc/5
        public void Delete(int id)
        {
        }
    }
}
