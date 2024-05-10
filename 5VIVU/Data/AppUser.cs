using System;
using System.Collections.Generic;

namespace _5VIVU.Data;

public partial class AppUser
{
    public int Id { get; set; }

    public string? Cmnd { get; set; }

    public string? FullName { get; set; }

    public string? Address { get; set; }

    public DateTime? Birthday { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public decimal? Wallet { get; set; }

    public int Role { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
