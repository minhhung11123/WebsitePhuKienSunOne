﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WebsitePhuKienSunOne.Models
{
    public partial class dbSunOneContext : DbContext
    {
        public dbSunOneContext()
        {
        }

        public dbSunOneContext(DbContextOptions<dbSunOneContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<TransactStatus> TransactStatuses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=MINHHUNG\\MINHHUNG;Database=dbSunOne;Integrated Security=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(150);

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Salt)
                    .HasMaxLength(6)
                    .IsFixedLength(true);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Accounts__RoleID__440B1D61");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CatId)
                    .HasName("PK__Categori__6A1C8ADABA52F3A5");

                entity.Property(e => e.CatId).HasColumnName("CatID");

                entity.Property(e => e.Alias).HasMaxLength(250);

                entity.Property(e => e.CatName).HasMaxLength(250);

                entity.Property(e => e.Cover).HasMaxLength(255);

                entity.Property(e => e.MetaDesc).HasMaxLength(250);

                entity.Property(e => e.MetaKey).HasMaxLength(250);

                entity.Property(e => e.ParentId).HasColumnName("ParentID");

                entity.Property(e => e.Thumb).HasMaxLength(250);

                entity.Property(e => e.Title).HasMaxLength(250);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.Avatar).HasMaxLength(255);

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.CrateDate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(150)
                    .IsFixedLength(true);

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Salt)
                    .HasMaxLength(6)
                    .IsFixedLength(true);

                entity.HasOne(d => d.DistrictNavigation)
                    .WithMany(p => p.CustomerDistrictNavigations)
                    .HasForeignKey(d => d.District)
                    .HasConstraintName("FK__Customers__Distr__5441852A");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.CustomerLocations)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("FK__Customers__Locat__534D60F1");

                entity.HasOne(d => d.WardNavigation)
                    .WithMany(p => p.CustomerWardNavigations)
                    .HasForeignKey(d => d.Ward)
                    .HasConstraintName("FK__Customers__Ward__5535A963");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Slug).HasMaxLength(100);

                entity.Property(e => e.Type).HasMaxLength(20);
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.HasKey(e => e.PostId)
                    .HasName("PK__News__AA126038C383EF6C");

                entity.Property(e => e.PostId).HasColumnName("PostID");

                entity.Property(e => e.AccountId).HasColumnName("AccountID");

                entity.Property(e => e.Alias).HasMaxLength(255);

                entity.Property(e => e.Author).HasMaxLength(255);

                entity.Property(e => e.CatId).HasColumnName("CatID");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.IsHot).HasColumnName("isHot");

                entity.Property(e => e.IsNewfeed).HasColumnName("isNewfeed");

                entity.Property(e => e.MetaDesc).HasMaxLength(255);

                entity.Property(e => e.MetaKey).HasMaxLength(255);

                entity.Property(e => e.Scontents)
                    .HasMaxLength(255)
                    .HasColumnName("SContents");

                entity.Property(e => e.Thumb).HasMaxLength(255);

                entity.Property(e => e.Title).HasMaxLength(255);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentId).HasColumnName("PaymentID");

                entity.Property(e => e.ShipDate).HasColumnType("datetime");

                entity.Property(e => e.TransactStatusId).HasColumnName("TransactStatusID");

                entity.HasOne(d => d.CityNavigation)
                    .WithMany(p => p.OrderCityNavigations)
                    .HasForeignKey(d => d.City)
                    .HasConstraintName("FK__Orders__City__5629CD9C");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK__Orders__Customer__403A8C7D");

                entity.HasOne(d => d.DistrictNavigation)
                    .WithMany(p => p.OrderDistrictNavigations)
                    .HasForeignKey(d => d.District)
                    .HasConstraintName("FK__Orders__District__571DF1D5");

                entity.HasOne(d => d.TransactStatus)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.TransactStatusId)
                    .HasConstraintName("FK__Orders__Transact__3F466844");

                entity.HasOne(d => d.WardNavigation)
                    .WithMany(p => p.OrderWardNavigations)
                    .HasForeignKey(d => d.Ward)
                    .HasConstraintName("FK__Orders__Ward__5812160E");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__OrderDeta__Order__3E52440B");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__OrderDeta__Produ__59063A47");
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.Property(e => e.PageId).HasColumnName("PageID");

                entity.Property(e => e.Alias).HasMaxLength(250);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.MetaDesc).HasMaxLength(250);

                entity.Property(e => e.MetaKey).HasMaxLength(250);

                entity.Property(e => e.PageName).HasMaxLength(250);

                entity.Property(e => e.Thumb).HasMaxLength(250);

                entity.Property(e => e.Title).HasMaxLength(250);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.Alias).HasMaxLength(255);

                entity.Property(e => e.CatId).HasColumnName("CatID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.MetaDesc).HasMaxLength(255);

                entity.Property(e => e.MetaKey).HasMaxLength(255);

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ShortDesc).HasMaxLength(255);

                entity.Property(e => e.Thumb).HasMaxLength(255);

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.Property(e => e.UnitslnStock).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Cat)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CatId)
                    .HasConstraintName("FK__Products__CatID__412EB0B6");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId)
                    .ValueGeneratedNever()
                    .HasColumnName("RoleID");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            modelBuilder.Entity<TransactStatus>(entity =>
            {
                entity.ToTable("TransactStatus");

                entity.Property(e => e.TransactStatusId).HasColumnName("TransactStatusID");

                entity.Property(e => e.Class)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
