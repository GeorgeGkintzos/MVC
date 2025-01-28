using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace mvc.Models;

public partial class Final_Project_DBcontext : DbContext
{
    public Final_Project_DBcontext()
    {
    }

    public Final_Project_DBcontext(DbContextOptions<Final_Project_DBcontext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<Call> Calls { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Phone> Phones { get; set; }

    public virtual DbSet<Program> Programs { get; set; }

    public virtual DbSet<Seller> Sellers { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public object Client { get; internal set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=RISK-SEVEN\\SQLEXPRESS;Database=Ergasia_mvc;Trusted_Connection=True;Trust Server Certificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__admin__4A311D2F90D2CFE3");

            entity.Property(e => e.AdminId).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithMany(p => p.Admins).HasConstraintName("FK__admin__User_id__5165187F");
        });

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.Bill_Id).HasName("PK__bills__CF6E7D43C82EEB8D");

            entity.Property(e => e.Bill_Id).ValueGeneratedNever();

            entity.HasOne(d => d.PhoneNumberNavigation).WithMany(p => p.Bills).HasConstraintName("FK__bills__PhoneNumb__59063A47");

            entity.HasMany(d => d.Calls).WithMany(p => p.Bills)
                .UsingEntity<Dictionary<string, object>>(
                    "BillsCall",
                    r => r.HasOne<Call>().WithMany()
                        .HasForeignKey("CallId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BillsCall__Call___5EBF139D"),
                    l => l.HasOne<Bill>().WithMany()
                        .HasForeignKey("BillId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BillsCall__Bill___5DCAEF64"),
                    j =>
                    {
                        j.HasKey("BillId", "CallId").HasName("PK__BillsCal__6EF0120D5CEEF7BD");
                        j.ToTable("BillsCalls");
                        j.IndexerProperty<int>("BillId").HasColumnName("Bill_ID");
                        j.IndexerProperty<int>("CallId").HasColumnName("Call_ID");
                    });
        });

        modelBuilder.Entity<Call>(entity =>
        {
            entity.HasKey(e => e.CallId).HasName("PK__calls__19E6F4EB4F32B64F");

            entity.Property(e => e.CallId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK__clients__75A5D7186CB5C0A3");

            modelBuilder.Entity<Client>()
                .HasOne(s => s.User)              // Κάθε Client ανήκει σε έναν User
                .WithMany(u => u.Clients)         // Ένας User μπορεί να έχει πολλούς Clients
                .HasForeignKey(s => s.UserId);    // Ο σύνδεσμος είναι το UserId στον πίνακα Clients

            entity.HasOne(d => d.User).WithMany(p => p.Clients).HasConstraintName("FK__clients__User_id__4E88ABD4");
        });

        modelBuilder.Entity<Phone>(entity =>
        {
            entity.HasKey(e => e.PhoneNumber).HasName("PK__phones__85FB4E398D24E418");

            entity.HasOne(d => d.ProgramNameNavigation).WithMany(p => p.Phones).HasConstraintName("FK__phones__Program___5629CD9C");
        });

        modelBuilder.Entity<Program>(entity =>
        {
            entity.HasKey(e => e.ProgramName).HasName("PK__programs__4F925711E4795A62");
        });

        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.SellerId).HasName("PK__sellers__016148B1D466BAFC");

            modelBuilder.Entity<Seller>()
                .HasOne(s => s.User)              // Κάθε Seller ανήκει σε έναν User
                .WithMany(u => u.Sellers)         // Ένας User μπορεί να έχει πολλούς Sellers
                .HasForeignKey(s => s.UserId);    // Ο σύνδεσμος είναι το UserId στον πίνακα Sellers

            entity.HasOne(d => d.User).WithMany(p => p.Sellers).HasConstraintName("FK__sellers__User_id__4BAC3F29");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__206A9DF8AD4CC61C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
