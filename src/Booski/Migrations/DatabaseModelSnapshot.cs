﻿// <auto-generated />
using System;
using Booski;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Booski.Migrations
{
    [DbContext(typeof(Database))]
    partial class DatabaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("Booski.Common.FileCache", b =>
                {
                    b.Property<string>("Uri")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Available")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("DownloadedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Filename")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Uri");

                    b.ToTable("FileCaches");
                });

            modelBuilder.Entity("Booski.Common.PostLog", b =>
                {
                    b.Property<string>("RecordKey")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Deleted")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Ignored")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Mastodon_InstanceDomain")
                        .HasColumnType("TEXT");

                    b.Property<string>("Mastodon_StatusId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Repository")
                        .HasColumnType("TEXT");

                    b.Property<long?>("Telegram_ChatId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Telegram_MessageCount")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Telegram_MessageId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("Version")
                        .HasColumnType("INTEGER");

                    b.Property<string>("X_PostId")
                        .HasColumnType("TEXT");

                    b.HasKey("RecordKey");

                    b.ToTable("PostLogs");
                });

            modelBuilder.Entity("Booski.Common.UsernameMap", b =>
                {
                    b.Property<string>("Bluesky_Did")
                        .HasColumnType("TEXT");

                    b.Property<string>("Mastodon_Handle")
                        .HasColumnType("TEXT");

                    b.Property<string>("Telegram_Handle")
                        .HasColumnType("TEXT");

                    b.Property<string>("Threads_Handle")
                        .HasColumnType("TEXT");

                    b.Property<string>("X_Handle")
                        .HasColumnType("TEXT");

                    b.HasKey("Bluesky_Did");

                    b.ToTable("UsernameMaps");
                });
#pragma warning restore 612, 618
        }
    }
}
