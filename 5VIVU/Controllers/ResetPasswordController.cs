using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SignUp.Models;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SignUp.Controllers;
public class ResetPasswordController : Controller
{
    private const string SqlConnectString = "Data Source=DESKTOP-U2TNCE6;Database=5vivu_banve;User Id=sa;Password=123;TrustServerCertificate=True";
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost("forgot-password")]
    public ActionResult ForgotPassword(string email)
    {
        if (!IsValidEmail(email))
        {
            TempData["ErrorMessage"] = "Please enter a valid email address.";
            return View("index");
        }

        User? user = FindUserByEmail(email);

        if (user == null)
        {
            // Display error message User Email not exist
            TempData["ErrorMessage"] = "User with this email does not exist.";
            return View("index");
        }

        string fromEmail = "tri488957@gmail.com";
        string fromPassword = "zpceoqduylpxoxqt";

        string optCode = (new Random()).Next(0, 1000000).ToString("D6");

        MailMessage message = new MailMessage();
        message.From = new MailAddress(fromEmail);
        message.Subject = "OTP for Resetting your password";
        message.To.Add(new MailAddress(email));
        message.Body = $"<html><body>Your OTP for resetting your password: {optCode}</body></html>";
        message.IsBodyHtml = true;

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(fromEmail, fromPassword),
            EnableSsl = true
        };

        smtpClient.Send(message);

        InsertNewOtp(email, optCode);

        return View("VerifyOtp", email);
    }
    [HttpPost("verify-otp")]
    public ActionResult VerifyOtp(string email, string inputOtp, string password)
    {
        // Lấy mã OTP từ cơ sở dữ liệu hoặc nơi lưu trữ khác
        string otp = GetOtpOfEmail(email);

        // Biểu thức chính quy để kiểm tra mã OTP
        string otpRegex = @"^\d{6}$";

        // Kiểm tra xem mã OTP có đúng định dạng không
        if (!Regex.IsMatch(inputOtp, otpRegex))
        {
            ViewBag.ErrorMessage = "OTP should have 6 digits.";
            return View("VerifyOtp", email);
        }

        // Kiểm tra xem mã OTP đã tồn tại và khớp với mã nhập vào không
        if (string.IsNullOrEmpty(otp) || otp != inputOtp)
        {
            ViewBag.ErrorMessage = "OTP does not exist or is incorrect.";
            return View("VerifyOtp", email);
        }

        // Cập nhật mật khẩu của người dùng
        if (UpdateUserPassword(email, password))
        {
            TempData["SuccessMessage"] = "Password updated successfully. You need to login again!";
            return RedirectToAction("Login", "Account");
        }

        return View("Index");
    }
    private User? FindUserByEmail(string email)
    {
        User? user = null;
        using (var connection = new SqlConnection(SqlConnectString))
        {
            var command = new SqlCommand($"SELECT TOP 1 * FROM [dbo].[AppUser] WHERE Email = @Email;", connection);
            SqlParameter parameter = new SqlParameter("@Email", email);
            command.Parameters.Add(parameter);

            connection.Open();

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    user = new User();
                    user.FullName = reader["FullName"].ToString() ?? "";
                    user.Email = reader["Email"].ToString() ?? "";
                    user.Password = reader["Password"].ToString() ?? "";
                }
            }
        }

        return user;
    }
    private bool UpdateUserPassword(string email, string password)
    {
        try
        {
            using (var connection = new SqlConnection(SqlConnectString))
            {
                var command = new SqlCommand("UPDATE [dbo].[AppUser] SET Password = @Password WHERE Email = @Email;", connection);
                SqlParameter emailParameter = new SqlParameter("@Email", email);
                SqlParameter otpParameter = new SqlParameter("@Password", password);
                command.Parameters.Add(emailParameter);
                command.Parameters.Add(otpParameter);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return false;
        }

    }
    private void InsertNewOtp(string email, string otp)
    {
        string query;
        if (GetOtpOfEmail(email) == null)
        {
            query = "INSERT INTO [dbo].[EmailOTP] (Email, OTP) VALUES (@Email,@OTP);";
        }
        else
        {
            query = "UPDATE [dbo].[EmailOTP] SET OTP = @OTP WHERE Email = @Email;";
        }
        using (var connection = new SqlConnection(SqlConnectString))
        {
            var command = new SqlCommand(query, connection);
            SqlParameter emailParameter = new SqlParameter("@Email", email);
            SqlParameter otpParameter = new SqlParameter("@OTP", otp);
            command.Parameters.Add(emailParameter);
            command.Parameters.Add(otpParameter);

            connection.Open();

            command.ExecuteNonQuery();
        }
    }
    private string? GetOtpOfEmail(string email)
    {
        string? otp = null;

        using (var connection = new SqlConnection(SqlConnectString))
        {
            var command = new SqlCommand("SELECT TOP 1 * FROM [dbo].[EmailOTP] WHERE Email = @Email;", connection);
            SqlParameter parameter = new SqlParameter("@Email", email);
            command.Parameters.Add(parameter);

            connection.Open();

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    otp = reader["OTP"].ToString();
                }
            }
        }

        return otp;
    }
    public bool IsValidEmail(string gmail) // t muoosn dung cai nay cho ben reset password 
    {

        bool exists = false;
        using (SqlConnection con = new SqlConnection(SqlConnectString))
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
