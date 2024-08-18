﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Demo.InternalApiService.DB;

#nullable disable

namespace Demo.InternalApiService.DB.EFMigrations
{
    [DbContext(typeof(ServiceDbContext))]
    partial class ServiceDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Demo.InternalApiService.DB.EFEntities.ProductEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Cost")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("CurrencyCountry")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Enabled")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
