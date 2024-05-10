using _5VIVU.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net;
using System.Net.Mail;

namespace _5VIVU.Controllers
{

        public class ForgotPasswordController : Controller
        {
            private readonly string _connectionString;

            public ForgotPasswordController(IConfiguration configuration)
            {
                _connectionString = configuration.GetConnectionString("5vivu");
            }

        public IActionResult ForgotPassword()
        {
            return View();
        }
            [HttpPost]
            public IActionResult ForgotPassword(string email)
            {
                bool emailExists = CheckEmailExists(email);

                if (emailExists)
                {
                    string otp = GenerateOTP();

                    SaveOTPToDatabase(email, otp);

                    SendOTPByEmail(email, otp);

                    TempData["Email"] = email;

                    return RedirectToAction("EnterOTP");
                }
                else
                {
                    return View("EmailNotFound");
                }
            }

            [HttpGet]
            public IActionResult EnterOTP()
        {
            ViewBag.Email = TempData["Email"];
            return View();
            }

            [HttpPost]
            public IActionResult VerifyOTP(string email, string otp)
            {
                bool otpMatches = CheckOTP(email, otp);
           
            if (otpMatches)
                {
                TempData["Email"] = email;
                return RedirectToAction("ResetPassword");
                }
                else
                {
                    return View("InvalidOTP");
                }
            }

            [HttpGet]
            public IActionResult ResetPassword()
            {
            ViewBag.Email = TempData["Email"];
            return View();
            }

            [HttpPost]
            public IActionResult UpdatePassword(string email, string newPassword)
            {
             /*email = TempData["Email"].ToString();*/
            UpdatePasswordInDatabase(email, newPassword);
                            return RedirectToAction("Login","Account");

           
      
           
        }

            private bool CheckEmailExists(string email)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT COUNT(*) FROM AppUser WHERE Email = @Email";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }

            private string GenerateOTP()
            {
                Random random = new Random();
                int otpNumber = random.Next(100000, 999999);
                return otpNumber.ToString();
            }

        /*            private void SaveOTPToDatabase(string email, string otp)
                    {
                        using (SqlConnection connection = new SqlConnection(_connectionString))
                        {
                            string query = "INSERT INTO EmailOTP (Email, OTP) VALUES (@Email, @OTP)";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@Email", email);
                            command.Parameters.AddWithValue("@OTP", otp);

                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }*/

        private void SaveOTPToDatabase(string email, string otp)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Kiểm tra xem email đã tồn tại
                    var checkQuery = "SELECT COUNT(*) FROM EmailOTP WHERE Email = @Email";
                    using (var checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Email", email ?? (object)DBNull.Value);

                        int existingCount = (int)checkCommand.ExecuteScalar();

                        if (existingCount > 0)
                        {
                            // Cập nhật OTP nếu email đã tồn tại
                            var updateQuery = "UPDATE EmailOTP SET OTP = @OTP WHERE Email = @Email";
                            using (var updateCommand = new SqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@Email", email);
                                updateCommand.Parameters.AddWithValue("@OTP", otp);
                                updateCommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Thêm OTP mới nếu email chưa tồn tại
                            var insertQuery = "INSERT INTO EmailOTP (Email, OTP) VALUES (@Email, @OTP)";
                            using (var insertCommand = new SqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@Email", email);
                                insertCommand.Parameters.AddWithValue("@OTP", otp);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc xử lý thêm
                Console.WriteLine(ex.Message);
            }
        }


        private void SendOTPByEmail(string email, string otp)
            {
                string senderEmail = "ngutahulol@gmail.com";
                string senderPassword = "xzbh nscf uilw ffbd";
                string smtpServer = "smtp.gmail.com";

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(senderEmail);
                mail.To.Add(email);
                mail.Subject = "5VIVU - OTP FOR PASSWORD CHANGING/OTP CHO ĐỔI MẬT KHẨU";
                mail.Body = $"OTP của bạn là: {otp}";

                SmtpClient smtpClient = new SmtpClient(smtpServer);
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                smtpClient.EnableSsl = true;
                smtpClient.Send(mail);
            }

            private bool CheckOTP(string email, string otp)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT COUNT(*) FROM EmailOTP WHERE  OTP = @OTP and Email = @email";
                    SqlCommand command = new SqlCommand(query, connection);
                   
                    command.Parameters.AddWithValue("@OTP", otp);
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }

            private void UpdatePasswordInDatabase(string email, string newPassword)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "UPDATE AppUser SET Password = @Password WHERE Email = @Email";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Password", newPassword);
                    command.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
