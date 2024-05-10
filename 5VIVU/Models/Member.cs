using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _5VIVU.Models
{
   
    public class Member
    {
        public int ID { get; set; }
        public string CMND { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public DateTime? Birthday { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public decimal? Wallet { get; set; }
        public int Role { get; set; }
    }
}
