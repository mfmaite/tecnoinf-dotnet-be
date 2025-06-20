﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ServiPuntosUy.DAO.Data.Central;

#nullable disable

namespace ServiPuntosUy.DAO.Migrations.Central
{
    [DbContext(typeof(CentralDbContext))]
    [Migration("20250616195120_addPromotionPrice")]
    partial class addPromotionPrice
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.Branch", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeOnly>("ClosingTime")
                        .HasColumnType("time");

                    b.Property<string>("Latitud")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Longitud")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeOnly>("OpenTime")
                        .HasColumnType("time");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TenantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("Branches");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.FuelPrices", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BranchId")
                        .HasColumnType("int");

                    b.Property<int>("FuelType")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("TenantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("TenantId");

                    b.ToTable("FuelPrices");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.GeneralParameter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("GeneralParameters");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            Description = "Moneda por defecto para la aplicación",
                            Key = "Currency",
                            Value = "USD"
                        });
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.LoyaltyConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("AccumulationRule")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ExpiricyPolicyDays")
                        .HasColumnType("int");

                    b.Property<string>("PointsName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PointsValue")
                        .HasColumnType("int");

                    b.Property<int>("TenantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("LoyaltyConfigs");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            AccumulationRule = 100m,
                            ExpiricyPolicyDays = 180,
                            PointsName = "Puntos",
                            PointsValue = 1,
                            TenantId = -1
                        });
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("AgeRestricted")
                        .HasColumnType("bit");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("TenantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.ProductStock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BranchId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductStocks");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.Promotion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TenantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("Promotions");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.PromotionBranch", b =>
                {
                    b.Property<int>("PromotionId")
                        .HasColumnType("int");

                    b.Property<int>("BranchId")
                        .HasColumnType("int");

                    b.Property<int>("TenantId")
                        .HasColumnType("int");

                    b.HasKey("PromotionId", "BranchId", "TenantId");

                    b.HasIndex("BranchId");

                    b.HasIndex("TenantId");

                    b.ToTable("PromotionBranches");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.PromotionProduct", b =>
                {
                    b.Property<int>("PromotionId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("PromotionId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("PromotionProducts");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.Redemption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BranchId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("QRCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RedemptionDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TenantId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("TenantId");

                    b.HasIndex("UserId");

                    b.ToTable("Redemptions");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.SatisfactionSurvey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("BranchId")
                        .HasColumnType("int");

                    b.Property<string>("Comments")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<DateTime>("SubmissionDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TenantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("TenantId");

                    b.ToTable("SatisfactionSurveys");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.Service", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("AgeRestricted")
                        .HasColumnType("bit");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("TenantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.ServiceAvailability", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BranchId")
                        .HasColumnType("int");

                    b.Property<TimeOnly>("EndTime")
                        .HasColumnType("time");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.Property<TimeOnly>("StartTime")
                        .HasColumnType("time");

                    b.Property<int>("TenantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("ServiceId");

                    b.HasIndex("TenantId");

                    b.ToTable("ServiceAvailabilities");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.Tenant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Tenants");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            Name = "ancap"
                        });
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.TenantUI", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("LogoUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PrimaryColor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecondaryColor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TenantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("TenantUIs");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            LogoUrl = "https://example.com/logo-ancap.png",
                            PrimaryColor = "#0000FF",
                            SecondaryColor = "#FFFF00",
                            TenantId = -1
                        });
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("BranchId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("PointsEarned")
                        .HasColumnType("int");

                    b.Property<int>("PointsSpent")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("UserId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.TransactionItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("TransactionId")
                        .HasColumnType("int");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("TransactionId");

                    b.ToTable("TransactionItems");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("BranchId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastLoginDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("NotificationsEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PointBalance")
                        .HasColumnType("int");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<int?>("TenantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BranchId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("TenantId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            Email = "admin@servipuntos.uy",
                            IsVerified = false,
                            LastLoginDate = new DateTime(2025, 6, 16, 19, 51, 19, 648, DateTimeKind.Utc).AddTicks(4270),
                            Name = "Admin Central",
                            NotificationsEnabled = true,
                            Password = "HkpItQSRf7sgLGXpgTvQUfcGLhCWgJnxl6+l7S6vBWE=",
                            PasswordSalt = "/YqS4KL1CJi31s09QJFWvA==",
                            PointBalance = 0,
                            Role = 1
                        },
                        new
                        {
                            Id = -2,
                            Email = "admintenant@servipuntos.uy",
                            IsVerified = false,
                            LastLoginDate = new DateTime(2025, 6, 16, 19, 51, 19, 648, DateTimeKind.Utc).AddTicks(4300),
                            Name = "Admin Tenant",
                            NotificationsEnabled = true,
                            Password = "HkpItQSRf7sgLGXpgTvQUfcGLhCWgJnxl6+l7S6vBWE=",
                            PasswordSalt = "/YqS4KL1CJi31s09QJFWvA==",
                            PointBalance = 0,
                            Role = 2,
                            TenantId = -1
                        });
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.Branch", b =>
                {
                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.FuelPrices", b =>
                {
                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Branch");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.LoyaltyConfig", b =>
                {
                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.Product", b =>
                {
                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.ProductStock", b =>
                {
                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Branch");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.Promotion", b =>
                {
                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.PromotionBranch", b =>
                {
                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Branch", "Branch")
                        .WithMany("PromotionBranch")
                        .HasForeignKey("BranchId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Promotion", "Promotion")
                        .WithMany("PromotionBranch")
                        .HasForeignKey("PromotionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Tenant", "Tenant")
                        .WithMany("PromotionBranch")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Branch");

                    b.Navigation("Promotion");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.PromotionProduct", b =>
                {
                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Product", "Product")
                        .WithMany("PromotionProduct")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Promotion", "Promotion")
                        .WithMany("PromotionProduct")
                        .HasForeignKey("PromotionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Promotion");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.Redemption", b =>
                {
                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ServiPuntosUy.DAO.Models.Central.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Branch");

                    b.Navigation("Tenant");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.SatisfactionSurvey", b =>
                {
                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId");

                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Branch");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.Service", b =>
                {
                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.ServiceAvailability", b =>
                {
                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Branch");

                    b.Navigation("Service");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.TenantUI", b =>
                {
                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.Transaction", b =>
                {
                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ServiPuntosUy.DAO.Models.Central.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Branch");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.TransactionItem", b =>
                {
                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Transaction", "Transaction")
                        .WithMany()
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.User", b =>
                {
                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Branch", "Branch")
                        .WithMany()
                        .HasForeignKey("BranchId");

                    b.HasOne("ServiPuntosUy.DAO.Models.Central.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Branch");

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.Branch", b =>
                {
                    b.Navigation("PromotionBranch");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.Product", b =>
                {
                    b.Navigation("PromotionProduct");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.Promotion", b =>
                {
                    b.Navigation("PromotionBranch");

                    b.Navigation("PromotionProduct");
                });

            modelBuilder.Entity("ServiPuntosUy.DAO.Models.Central.Tenant", b =>
                {
                    b.Navigation("PromotionBranch");
                });
#pragma warning restore 612, 618
        }
    }
}
