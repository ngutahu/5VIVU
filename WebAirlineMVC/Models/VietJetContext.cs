using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebAirlineMVC.Models;

public partial class VietJetContext : DbContext
{
    public VietJetContext()
    {
    }

    public VietJetContext(DbContextOptions<VietJetContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aircraft> Aircraft { get; set; }

    public virtual DbSet<Airport> Airports { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<SoldSeat> SoldSeats { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-U2TNCE6;Initial Catalog=VietJet;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aircraft>(entity =>
        {
            entity.HasKey(e => e.AircraftId).HasName("PK__Aircraft__F75CBC0B3147852E");

            entity.Property(e => e.AircraftId).HasColumnName("AircraftID");
            entity.Property(e => e.AircraftName).HasMaxLength(50);
        });

        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.AirportId).HasName("PK__Airport__E3DBE08AAAE96481");

            entity.ToTable("Airport");

            entity.Property(e => e.AirportId).HasColumnName("AirportID");
            entity.Property(e => e.AirportName).HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(100);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__73951ACD228F5297");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BookingTime).HasColumnType("datetime");
            entity.Property(e => e.TicketId).HasColumnName("TicketID");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("FK__Booking__TicketI__49C3F6B7");
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.FlightId).HasName("PK__Flight__8A9E148E4B513895");

            entity.ToTable("Flight");

            entity.Property(e => e.FlightId).HasColumnName("FlightID");
            entity.Property(e => e.AircraftId).HasColumnName("AircraftID");
            entity.Property(e => e.ArrivalAirportId).HasColumnName("ArrivalAirportID");
            entity.Property(e => e.ArrivalTime).HasColumnType("datetime");
            entity.Property(e => e.DepartureAirportId).HasColumnName("DepartureAirportID");
            entity.Property(e => e.DepartureTime).HasColumnType("datetime");

            entity.HasOne(d => d.Aircraft).WithMany(p => p.Flights)
                .HasForeignKey(d => d.AircraftId)
                .HasConstraintName("FK__Flight__Aircraft__3E52440B");

            entity.HasOne(d => d.ArrivalAirport).WithMany(p => p.FlightArrivalAirports)
                .HasForeignKey(d => d.ArrivalAirportId)
                .HasConstraintName("FK__Flight__ArrivalA__403A8C7D");

            entity.HasOne(d => d.DepartureAirport).WithMany(p => p.FlightDepartureAirports)
                .HasForeignKey(d => d.DepartureAirportId)
                .HasConstraintName("FK__Flight__Departur__3F466844");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("PK__Seat__311713D30BD360C5");

            entity.ToTable("Seat");

            entity.Property(e => e.SeatId).HasColumnName("SeatID");
            entity.Property(e => e.AircraftId).HasColumnName("AircraftID");

            entity.HasOne(d => d.Aircraft).WithMany(p => p.Seats)
                .HasForeignKey(d => d.AircraftId)
                .HasConstraintName("FK__Seat__AircraftID__3B75D760");
        });

        modelBuilder.Entity<SoldSeat>(entity =>
        {
            entity.HasKey(e => e.SoldSeatId).HasName("PK__SoldSeat__68DD86593B10D948");

            entity.ToTable("SoldSeat");

            entity.Property(e => e.SoldSeatId).HasColumnName("SoldSeatID");
            entity.Property(e => e.FlightId).HasColumnName("FlightID");
            entity.Property(e => e.SeatId).HasColumnName("SeatID");

            entity.HasOne(d => d.Flight).WithMany(p => p.SoldSeats)
                .HasForeignKey(d => d.FlightId)
                .HasConstraintName("FK__SoldSeat__Flight__4316F928");

            entity.HasOne(d => d.Seat).WithMany(p => p.SoldSeats)
                .HasForeignKey(d => d.SeatId)
                .HasConstraintName("FK__SoldSeat__SeatID__440B1D61");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Tickets__712CC62778788B71");

            entity.Property(e => e.TicketId).HasColumnName("TicketID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IdentificationNumber).HasMaxLength(20);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SoldSeatId).HasColumnName("SoldSeatID");

            entity.HasOne(d => d.SoldSeat).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.SoldSeatId)
                .HasConstraintName("FK__Tickets__SoldSea__46E78A0C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
