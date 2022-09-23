﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using InsuranceWebAPI.ViewModels;

#nullable disable

namespace InsuranceWebAPI.models
{
    public partial class insuranceContext : DbContext
    {
        public insuranceContext()
        {
        }

        public insuranceContext(DbContextOptions<insuranceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Claim> Claims { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Plan> Plans { get; set; }
        public virtual DbSet<Policy> Policies { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }

        public virtual DbSet<CustomerVehiclePolicy> CustomerVehiclePolicies { get; set; }

        public virtual DbSet<ClaimPolicy> ClaimPolicies { get; set; }

        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=.\\sqlexpress;database=insurance;trusted_connection=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Claim>(entity =>
            {
                entity.ToTable("claim");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClaimDate)
                    .HasColumnType("date")
                    .HasColumnName("claim_date");

                entity.Property(e => e.Isapproved)
                    .HasColumnName("isapproved")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.PolicyId).HasColumnName("policy_id");

                entity.HasOne(d => d.Policy)
                    .WithMany(p => p.Claims)
                    .HasForeignKey(d => d.PolicyId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_policy");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customer");

                entity.HasIndex(e => e.ContactNumber, "UQ__customer__4F86E9D7E4D5C95B")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__customer__AB6E6164CED56022")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasColumnName("address");

                entity.Property(e => e.ContactNumber)
                    .IsRequired()
                    .HasMaxLength(13)
                    .IsUnicode(false)
                    .HasColumnName("contactNumber");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("dateOfBirth");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasColumnName("password");
            });

            modelBuilder.Entity<Plan>(entity =>
            {
                entity.ToTable("plans");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("amount");

                entity.Property(e => e.Term).HasColumnName("term");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("type");

                entity.Property(e => e.Typeofvehicle)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("typeofvehicle");
            });

            modelBuilder.Entity<Policy>(entity =>
            {
                entity.ToTable("policy");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.PlansId).HasColumnName("plans_id");

                entity.Property(e => e.PurchaseDate)
                    .HasColumnType("date")
                    .HasColumnName("purchase_date");

                entity.Property(e => e.RegistrationNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("registration_number");

                entity.Property(e => e.RenewAmount)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("renew_amount");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Plans)
                    .WithMany(p => p.Policies)
                    .HasForeignKey(d => d.PlansId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_plans");

                entity.HasOne(d => d.RegistrationNumberNavigation)
                    .WithMany(p => p.Policies)
                    .HasForeignKey(d => d.RegistrationNumber)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_regno");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Policies)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_user");
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(e => e.RegistrationNumber)
                    .HasName("PK__vehicle__125DB2A223C24C6A");

                entity.ToTable("vehicle");

                entity.HasIndex(e => e.ChassisNumber, "UQ__vehicle__54B69A012524A0AD")
                    .IsUnique();

                entity.Property(e => e.RegistrationNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("registration_number");

                entity.Property(e => e.ChassisNumber)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("chassis_number");

                entity.Property(e => e.EngineNumber)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("engine_number");

                entity.Property(e => e.License)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("license");

                entity.Property(e => e.ManufacturerName)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasColumnName("manufacturer_name");

                entity.Property(e => e.Model)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("model");

                entity.Property(e => e.PurchaseDate)
                    .HasColumnType("date")
                    .HasColumnName("purchase_date");

                entity.Property(e => e.TypeOfVehicle)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("typeOfVehicle");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
