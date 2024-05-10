using _5VIVU.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SignUp.Models;
using SignUp.Models.PageDto;
using System.Data.SqlClient;

namespace SignUp.Controllers;
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet("Admin/list-bookings")]
    public IActionResult ListBookings([FromQuery] int page = 1, [FromQuery] int take = 5,
        [FromQuery] SearchType searchType = SearchType.None, [FromQuery] string searchValue = "")
    {
        var pageDto = GetAllBookings(new PageQueryDto { Page = page, Take = take, SearchType = searchType, SearchValue = searchValue });
        return View("ListBookings", pageDto);
    }
    private PageDto<BookingModel> GetAllBookings(PageQueryDto pageQueryDto)
    {
        var data = new List<BookingModel>();
        var itemCount = 0;
        var query = "SELECT * FROM dbo.Ticket";
        if (pageQueryDto.SearchType != SearchType.None && !pageQueryDto.SearchValue.IsNullOrEmpty())
        {
            if (pageQueryDto.SearchType == SearchType.Email)
            {
                query = query + $" WHERE {pageQueryDto.SearchType.ToString()} LIKE '%{pageQueryDto.SearchValue}%'";
            }
            else
            {
                query = query + $" WHERE {pageQueryDto.SearchType.ToString()} = {pageQueryDto.SearchValue}";
            }
        }
        var selectQuery = query + $" ORDER BY TicketID OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";
        var countQuery = $"SELECT COUNT(*) AS ItemCount FROM ({query}) AS subquery;";

        using (var connection = new SqlConnection("Data Source=DESKTOP-U2TNCE6;Database=5vivu_banve;User Id=sa;Password=123;TrustServerCertificate=True"))
        {
            var command = new SqlCommand(selectQuery, connection);
            command.Parameters.Add(new SqlParameter("@Skip", pageQueryDto.Skip()));
            command.Parameters.Add(new SqlParameter("@Take", pageQueryDto.Take));

            connection.Open();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    data.Add(new BookingModel(reader));
                }
            }

            command = new SqlCommand(countQuery, connection);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    itemCount = reader.GetInt32(reader.GetOrdinal("ItemCount"));
                }
            }

            var pageMetaDto = new PageMetaDto(pageQueryDto, itemCount);
            return new PageDto<BookingModel>(data, pageMetaDto);
        }
    }
    private string connectionString;

    public AdminController(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("5vivu");
    }

    public IActionResult MemberList(string search)
    {
        List<Member> members = new List<Member>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = @"SELECT FullName, Email, Address, Phone, CMND FROM AppUser 
                      WHERE (@Search IS NULL OR FullName LIKE '%' + @Search + '%' OR CMND LIKE '%' + @Search + '%')";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Search", search ?? (object)DBNull.Value);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Member member = new Member
                        {
                            FullName = reader["FullName"].ToString(),
                            Email = reader["Email"].ToString(),
                            Address = reader["Address"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            CMND = reader["CMND"].ToString()
                        };
                        members.Add(member);
                    }
                }
            }
        }

        return View(members);
    }

    public IActionResult AddUser(Member member)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = "INSERT INTO AppUser (FullName, Email, Address, Phone, CMND, Password,Role) VALUES (@FullName, @Email, @Address, @Phone, @CMND, @Password,@Role)";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FullName", member.FullName);
                command.Parameters.AddWithValue("@Email", member.Email);
                command.Parameters.AddWithValue("@Address", member.Address);
                command.Parameters.AddWithValue("@Phone", member.Phone);
                command.Parameters.AddWithValue("@CMND", member.CMND);
                command.Parameters.AddWithValue("@Password", member.Password);
                command.Parameters.AddWithValue("@Role", "0");
                command.ExecuteNonQuery();
            }
        }

        return RedirectToAction("MemberList");
    }

    [HttpGet]
    public IActionResult AddUser()
    {
        return View();
    }
}
