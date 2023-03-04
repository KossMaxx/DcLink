﻿// <auto-generated />
using System;
using LegacySql.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LegacySql.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20201027120424_Add_Index_By_ErpGuid_To_ProductMaps")]
    partial class Add_Index_By_ErpGuid_To_ProductMaps
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("LegacySql.Data.Models.ClientMapEF", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ErpGuid")
                        .HasColumnType("uuid");

                    b.Property<long>("LegacyId")
                        .HasColumnType("bigint");

                    b.Property<Guid>("MapGuid")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ErpGuid")
                        .IsUnique();

                    b.HasIndex("LegacyId")
                        .IsUnique();

                    b.ToTable("ClientMaps");
                });

            modelBuilder.Entity("LegacySql.Data.Models.ClientOrderMapEF", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ErpGuid")
                        .HasColumnType("uuid");

                    b.Property<long>("LegacyId")
                        .HasColumnType("bigint");

                    b.Property<Guid>("MapGuid")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ErpGuid")
                        .IsUnique();

                    b.HasIndex("LegacyId")
                        .IsUnique();

                    b.ToTable("ClientOrderMaps");
                });

            modelBuilder.Entity("LegacySql.Data.Models.ExecutingJobEF", b =>
                {
                    b.Property<int>("JobType")
                        .HasColumnType("integer");

                    b.HasKey("JobType");

                    b.HasIndex("JobType")
                        .IsUnique();

                    b.ToTable("ExecutingJobs");
                });

            modelBuilder.Entity("LegacySql.Data.Models.LastChangedDateEF", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("EntityType")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("LastChangedDates");
                });

            modelBuilder.Entity("LegacySql.Data.Models.NotFullMappedEF", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("InnerId")
                        .HasColumnType("bigint");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Type");

                    b.HasIndex("InnerId", "Type")
                        .IsUnique();

                    b.ToTable("NotFullMapped");
                });

            modelBuilder.Entity("LegacySql.Data.Models.ProductMapEF", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ErpGuid")
                        .HasColumnType("uuid");

                    b.Property<long>("LegacyId")
                        .HasColumnType("bigint");

                    b.Property<Guid>("MapGuid")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ErpGuid")
                        .IsUnique();

                    b.HasIndex("LegacyId")
                        .IsUnique();

                    b.ToTable("ProductMaps");
                });

            modelBuilder.Entity("LegacySql.Data.Models.ProductTypeCategoryMapEF", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ErpGuid")
                        .HasColumnType("uuid");

                    b.Property<long>("LegacyId")
                        .HasColumnType("bigint");

                    b.Property<Guid>("MapGuid")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ErpGuid")
                        .IsUnique();

                    b.HasIndex("LegacyId")
                        .IsUnique();

                    b.ToTable("ProductTypeCategoryMaps");
                });

            modelBuilder.Entity("LegacySql.Data.Models.ProductTypeCategoryParameterMapEF", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ErpGuid")
                        .HasColumnType("uuid");

                    b.Property<long>("LegacyId")
                        .HasColumnType("bigint");

                    b.Property<Guid>("MapGuid")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ErpGuid")
                        .IsUnique();

                    b.HasIndex("LegacyId")
                        .IsUnique();

                    b.ToTable("ProductTypeCategoryParameterMaps");
                });

            modelBuilder.Entity("LegacySql.Data.Models.ProductTypeMapEF", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ErpGuid")
                        .HasColumnType("uuid");

                    b.Property<long>("LegacyId")
                        .HasColumnType("bigint");

                    b.Property<Guid>("MapGuid")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ErpGuid")
                        .IsUnique();

                    b.HasIndex("LegacyId")
                        .IsUnique();

                    b.ToTable("ProductTypeMaps");
                });

            modelBuilder.Entity("LegacySql.Data.Models.SellingPriceMapEF", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ErpGuid")
                        .HasColumnType("uuid");

                    b.Property<long>("LegacyId")
                        .HasColumnType("bigint");

                    b.Property<Guid>("MapGuid")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("SellingPriceMaps");
                });

            modelBuilder.Entity("LegacySql.Data.Models.SupplierPriceMapEF", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ErpGuid")
                        .HasColumnType("uuid");

                    b.Property<long>("LegacyId")
                        .HasColumnType("bigint");

                    b.Property<Guid>("MapGuid")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ErpGuid")
                        .IsUnique();

                    b.HasIndex("LegacyId")
                        .IsUnique();

                    b.ToTable("SupplierPriceMaps");
                });
#pragma warning restore 612, 618
        }
    }
}
