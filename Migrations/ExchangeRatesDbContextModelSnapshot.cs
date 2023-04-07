﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication2.Data;

namespace WebApplication2.Migrations
{
    [DbContext(typeof(ExchangeRatesDbContext))]
    partial class ExchangeRatesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.16");

            modelBuilder.Entity("WebApplication2.Model.Currency", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Country")
                        .HasColumnType("longtext");

                    b.HasKey("Code");

                    b.ToTable("Currency");
                });

            modelBuilder.Entity("WebApplication2.Model.ExchangeRate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("CurrencyCode")
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<float>("Rate")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyCode");

                    b.ToTable("ExchangeRates");
                });

            modelBuilder.Entity("WebApplication2.Model.ExchangeRate", b =>
                {
                    b.HasOne("WebApplication2.Model.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyCode");

                    b.Navigation("Currency");
                });
#pragma warning restore 612, 618
        }
    }
}
