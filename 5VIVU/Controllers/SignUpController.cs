using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using SignUp.Models;

namespace SignUp.Controllers
{
    
    public class SignUpController : Controller
    {
        public string value = "";

        [HttpGet]
        public IActionResult Index()
        {
           
             if (TempData.ContainsKey("SuccessMessage"))
 {
     ViewBag.SuccessMessage1 = TempData["SuccessMessage"];
 }


 return View("Index");
        }

        [HttpPost]
        public ActionResult SignUp(string name, string email, string pass, string confirmpass, string birthday, string status)
        {
            bool isSuccess = false;
            ViewBag.Exists = isSuccess;
            if (GetEmailExist(email))
            {
                ViewBag.Exists = isSuccess;
                
                return View("Index");
            }
            else
            {
                ViewBag.Exists = true;
              
                User er = new User();
                using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-U2TNCE6;Database=5vivu_banve;User Id=sa;Password=123;TrustServerCertificate=True"))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_AppUserDetail", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FullName", name);
                        cmd.Parameters.AddWithValue("@Password", pass);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Birthday", birthday);
                        cmd.Parameters.AddWithValue("@status", "INSERT");
                        cmd.Parameters.AddWithValue("@Role", "0");
                        con.Open();
                        ViewData["result"] = cmd.ExecuteNonQuery();
                        con.Close();
                        isSuccess = true;
                    }
                }
                
                if (isSuccess)
                {
                    ViewBag.SuccessMessage = "Đăng ký thành công!";
                }
            }

            ViewBag.Exists = isSuccess;
            return View("Index");
        }

        public bool GetEmailExist(string gmail)
        {

            bool exists = false;
            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-U2TNCE6;Database=5vivu_banve;User Id=sa;Password=123;TrustServerCertificate=True"))
            {


                using (SqlCommand cmd = new SqlCommand("CheckEmailExistence", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@gmail", gmail);

                    con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    exists = (count > 0);

                    con.Close();
                }
            }
            return exists;
        }
    }


}
