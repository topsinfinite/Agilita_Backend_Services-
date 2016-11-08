using empservice.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OracleClient;
using System.DirectoryServices;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace empservice.DAL
{
    public class Utility
    {
        private static string lDAPConnectionString = ConfigurationManager.AppSettings["LDAP_URL"].ToString();
        public static string ConnectionStr()
        {
            //string connection = ConfigurationManager.ConnectionStrings["finLiveConnectionString"].ConnectionString;
            string connection = ConfigurationManager.ConnectionStrings["OraConnection"].ConnectionString;
            return connection;

        }
        public static AppUser GetUser(string staffID)
        {
            try
            {
                string connection = ConnectionStr();
                using (OracleConnection con = new OracleConnection(connection))
                {
                    con.Open();
                    string sql = "select staff_no,person_id,unit Department, sex, First_name, Last_name,job_title, email_address, supervisor_id, TO_CHAR( date_of_birth, 'MM/DD/YYYY') as  dob " +
                                 "from amcon_master_all where staff_no='" + staffID.Trim() + "'";
                    OracleCommand cmd = new OracleCommand(sql);
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();
                    //List<User> UserList = new List<User>();
                    AppUser urs = new AppUser();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            urs.EmployeeID = reader["staff_no"].ToString();
                            urs.PersonID = reader["person_id"].ToString();
                            urs.FullName = reader["last_name"].ToString() + " " + reader["first_name"].ToString();
                            urs.Email = reader["email_address"].ToString();
                            urs.Department = reader["Department"].ToString();
                            urs.JobTitle = reader["job_title"].ToString();
                            urs.Gender = reader["sex"].ToString();
                            urs.DOB = reader["dob"].ToString();
                            //UserList.Add(urs);
                        }
                    }
                    else
                    {
                        urs = null;
                    }
                    return urs;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<Employee> SearchEmployee (string empname)
        {
            try
            {
                List<Employee> EmpList = new List<Employee>();
                if (string.IsNullOrEmpty(empname))
                {
                    return EmpList;
                }
                string connection = ConnectionStr();
                using (OracleConnection con = new OracleConnection(connection))
                {
                    con.Open();
                    string sql = "select staff_no,person_id,unit Department, sex, First_name, Last_name,job_title, email_address, supervisor_id, TO_CHAR( date_of_birth, 'MM/DD/YYYY') as  dob " +
                                 "from amcon_master_all where full_name like '%" + empname.Trim().ToUpper() + "%'";
                    OracleCommand cmd = new OracleCommand(sql);
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();
                   
                   
                    if (reader.HasRows)
                    {
                         while (reader.Read())
                        {
                            Employee urs = new Employee();
                            urs.EmployeeID = reader["staff_no"].ToString();
                            urs.PersonID = reader["person_id"].ToString();
                            urs.FullName = reader["last_name"].ToString() + " " + reader["first_name"].ToString();
                            urs.Email = reader["email_address"].ToString();
                            urs.Department = reader["Department"].ToString();
                            urs.JobTitle = reader["job_title"].ToString();
                            urs.Gender = reader["sex"].ToString();
                            urs.DOB = reader["dob"].ToString();
                            EmpList.Add(urs);
                        }
                    }
                     
                    return EmpList;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Employee GetEmployeeDetails(string EmpID)
        {
            try
            {
               // SearchResult userDirectoryEntryDetails = GetUserDirectoryEntryDetails(EmpID);
                UserDetail detail = new UserDetail();
                detail = null;
               // detail = PopulateUserData(userDirectoryEntryDetails);
                string connection = ConnectionStr();
                using (OracleConnection con = new OracleConnection(connection))
                {
                    con.Open();
                    string sql = "select a.staff_no,a.person_id,unit Department,a.Full_name, a.sex, a.First_name,a.Last_name,a.job_title, " +
                "email_address,a.supervisor_id, TO_CHAR( date_of_birth, 'MM/DD/YYYY') as  dob, " +
                " nvl((select Full_name from amcon_master_all b where b.person_id  = a.supervisor_id), '' ) managerName, " +
                " nvl ((select staff_no from amcon_master_all c where c.person_id  = a.supervisor_id), '' )  managerID, " +
                " nvl ((select count(staff_no) from amcon_master_all d where d.supervisor_id  = a.person_id), '' ) Reports " +
                "from amcon_master_all a  where staff_no='" + EmpID + "'";
                    OracleCommand cmd = new OracleCommand(sql);
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();

                    Employee urs = new Employee();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            urs.EmployeeID = reader["staff_no"].ToString();
                            urs.PersonID = reader["person_id"].ToString();
                            urs.FullName = reader["last_name"].ToString() + " " + reader["first_name"].ToString();
                            urs.Email = reader["email_address"].ToString();
                            urs.Department = reader["Department"].ToString();
                            urs.JobTitle = reader["job_title"].ToString();
                            urs.Gender = reader["sex"].ToString();
                            urs.DOB = reader["dob"].ToString();
                            urs.ManagerID = reader["managerID"].ToString();
                            urs.ManagerName = reader["managerName"].ToString();
                            urs.Reports = int.Parse(reader["Reports"].ToString());
                            if (detail != null)
                            {
                                urs.MobileNo = detail.personMobileNumber;
                                urs.OfficeExt = detail.personTelephone;
                                urs.Email = detail.personEmail;
                                urs.Extension = detail.PhoneExt;
                            }else
                                {
                                    urs.MobileNo = "+234802123456789" ;
                                    urs.OfficeExt = "012703210";
                                    urs.Extension = "3110";
                                   // urs.Email = detail.personEmail;
                                }
                            // EmpList.Add(urs);
                        }
                    }
                    else
                    {
                        urs = null;
                    }

                    return urs;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SearchResult GetUserDirectoryEntryDetails(string loginUserName)
        {
            string str = loginUserName;
            try
            {
                DirectoryEntry entry = new DirectoryEntry(lDAPConnectionString, "userupdate", "amcon123456@")
                {
                    AuthenticationType = AuthenticationTypes.Secure
                };
                SearchResult result = new DirectorySearcher { SearchRoot = entry, Filter = "(&(objectClass=user)(SAMAccountName=" + loginUserName + "))", SearchScope = SearchScope.Subtree }.FindOne();
                if (result != null)
                {
                    return result;
                }
                return null;
            }
            catch (Exception exception)
            {
               // WriteError("Error: " + exception.Message);
                throw exception;
                return null;
            }
        }

        public static List<Employee> FindByManagerId(string mgrId)
        {
            try
            {
                List<Employee> EmpList = new List<Employee>();
                if (string.IsNullOrEmpty(mgrId))
                {
                    return EmpList;
                }
                string connection = ConnectionStr();
                using (OracleConnection con = new OracleConnection(connection))
                {
                    con.Open();
                    string sql = "select staff_no,person_id,unit Department, sex, First_name, Last_name,job_title, email_address, supervisor_id, TO_CHAR( date_of_birth, 'MM/DD/YYYY') as  dob " +
                                 "from amcon_master_all where  Supervisor_ID = '" + mgrId.Trim() + "'";
                    OracleCommand cmd = new OracleCommand(sql);
                    cmd.Connection = con;
                    OracleDataReader reader = cmd.ExecuteReader();


                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Employee urs = new Employee();
                            urs.EmployeeID = reader["staff_no"].ToString();
                            urs.PersonID = reader["person_id"].ToString();
                            urs.FullName = reader["last_name"].ToString() + " " + reader["first_name"].ToString();
                            urs.Email = reader["email_address"].ToString();
                            urs.Department = reader["Department"].ToString();
                            urs.JobTitle = reader["job_title"].ToString();
                            urs.Gender = reader["sex"].ToString();
                            urs.DOB = reader["dob"].ToString();
                            EmpList.Add(urs);
                        }
                    }

                    return EmpList;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static UserDetail PopulateUserData(SearchResult theSearchResult)
        {
            UserDetail detail = new UserDetail();
            if (theSearchResult != null)
            {
                ResultPropertyCollection properties = theSearchResult.Properties;
                if (properties.Contains("givenName"))
                {
                    detail.personFirstName = theSearchResult.Properties["givenName"][0].ToString();
                }
                if (properties.Contains("sn"))
                {
                    detail.personSurname = theSearchResult.Properties["sn"][0].ToString();
                }
                if (properties.Contains("mail"))
                {
                    detail.personEmail = theSearchResult.Properties["mail"][0].ToString();
                }
                if (properties.Contains("title"))
                {
                    detail.personJobTitle = theSearchResult.Properties["title"][0].ToString();
                }
                if (properties.Contains("info"))
                {
                    detail.personDepartment = theSearchResult.Properties["info"][0].ToString();
                }
                if (properties.Contains("st"))
                {
                    detail.personCounty = theSearchResult.Properties["st"][0].ToString();
                }
                if (properties.Contains("mobile"))
                {
                    detail.personMobileNumber = theSearchResult.Properties["mobile"][0].ToString();
                }
                if (properties.Contains("telephoneNumber"))
                {
                    detail.personTelephone = theSearchResult.Properties["telephoneNumber"][0].ToString();
                }
                if( properties.Contains("ipPhone"))
                {
                    detail.PhoneExt = theSearchResult.Properties["ipPhone"][0].ToString();
                }
                if (properties.Contains("postalCode"))
                {
                    detail.personPostCode = theSearchResult.Properties["postalCode"][0].ToString();
                }
                if (properties.Contains("department"))
                {
                    detail.personDepartment = theSearchResult.Properties["department"][0].ToString();
                }
                if (properties.Contains("streetAddress"))
                {
                    detail.personAddress1 = theSearchResult.Properties["streetAddress"][0].ToString();
                }
                if (properties.Contains("l"))
                {
                    detail.personTown = theSearchResult.Properties["l"][0].ToString();
                }
                if (properties.Contains("physicalDeliveryOfficeName"))
                {
                    detail.personLocation = theSearchResult.Properties["physicalDeliveryOfficeName"][0].ToString();
                }
                return detail;
            }
            return null;
        }

        public static void WriteError(string errorMessage)
        {
            try
            {
                string path = "~/ErrorLog/" + DateTime.Today.ToString("dd-MMM-yy") + ".txt";
                if (!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path)))
                {
                    File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();
                }
                using (StreamWriter w = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
                {
                    w.WriteLine("\r\nLog Entry : ");
                    w.WriteLine("{0}", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    string err = "Error in: " + System.Web.HttpContext.Current.Request.Url.ToString() +
                                  ". Error Message:" + errorMessage;
                    w.WriteLine(err);
                    w.WriteLine("__________________________");
                    w.Flush();
                    w.Close();
                }
            }
            catch (Exception ex)
            {
                WriteError(ex.Message);
            }

        }
    }

}