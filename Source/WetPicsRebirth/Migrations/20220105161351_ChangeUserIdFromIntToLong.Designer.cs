﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WetPicsRebirth.Data;

#nullable disable

namespace WetPicsRebirth.Migrations
{
    [DbContext(typeof(WetPicsRebirthDbContext))]
    [Migration("20220105161351_ChangeUserIdFromIntToLong")]
    partial class ChangeUserIdFromIntToLong
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WetPicsRebirth.Data.Entities.Actress", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("AddedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<int>("ImageSource")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("ModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Options")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.ToTable("Actresses");
                });

            modelBuilder.Entity("WetPicsRebirth.Data.Entities.PostedMedia", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("AddedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<string>("FileId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ImageSource")
                        .HasColumnType("integer");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("ModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PostHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PostId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ChatId", "ImageSource", "PostId");

                    b.ToTable("PostedMedia");
                });

            modelBuilder.Entity("WetPicsRebirth.Data.Entities.Scene", b =>
                {
                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<DateTimeOffset>("AddedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LastPostedTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("MinutesInterval")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("ModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("ChatId");

                    b.ToTable("Scenes");
                });

            modelBuilder.Entity("WetPicsRebirth.Data.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTimeOffset>("AddedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("ModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WetPicsRebirth.Data.Entities.Vote", b =>
                {
                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<DateTimeOffset>("AddedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("ModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("ChatId", "MessageId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("Votes");
                });

            modelBuilder.Entity("WetPicsRebirth.Data.Entities.Actress", b =>
                {
                    b.HasOne("WetPicsRebirth.Data.Entities.Scene", "Scene")
                        .WithMany("Actresses")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Scene");
                });

            modelBuilder.Entity("WetPicsRebirth.Data.Entities.Vote", b =>
                {
                    b.HasOne("WetPicsRebirth.Data.Entities.User", "User")
                        .WithMany("Votes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WetPicsRebirth.Data.Entities.PostedMedia", "PostedMedia")
                        .WithMany("Votes")
                        .HasForeignKey("ChatId", "MessageId")
                        .HasPrincipalKey("ChatId", "MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PostedMedia");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WetPicsRebirth.Data.Entities.PostedMedia", b =>
                {
                    b.Navigation("Votes");
                });

            modelBuilder.Entity("WetPicsRebirth.Data.Entities.Scene", b =>
                {
                    b.Navigation("Actresses");
                });

            modelBuilder.Entity("WetPicsRebirth.Data.Entities.User", b =>
                {
                    b.Navigation("Votes");
                });
#pragma warning restore 612, 618
        }
    }
}
