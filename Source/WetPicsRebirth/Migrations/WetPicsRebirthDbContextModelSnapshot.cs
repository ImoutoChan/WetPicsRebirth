﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WetPicsRebirth.Data;

#nullable disable

namespace WetPicsRebirth.Migrations
{
    [DbContext(typeof(WetPicsRebirthDbContext))]
    partial class WetPicsRebirthDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<Instant>("AddedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<int>("ImageSource")
                        .HasColumnType("integer");

                    b.Property<Instant>("ModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Options")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.ToTable("Actresses");
                });

            modelBuilder.Entity("WetPicsRebirth.Data.Entities.ModeratedMedia", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Instant>("AddedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("boolean");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.Property<Instant>("ModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("PostId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.HasIndex("PostId", "Hash");

                    b.ToTable("ModeratedMedia");
                });

            modelBuilder.Entity("WetPicsRebirth.Data.Entities.PostedMedia", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Instant>("AddedDate")
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

                    b.Property<Instant>("ModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PostHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("PostId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ChatId", "MessageId");

                    b.HasIndex("ChatId", "ImageSource", "PostId");

                    b.ToTable("PostedMedia");
                });

            modelBuilder.Entity("WetPicsRebirth.Data.Entities.Scene", b =>
                {
                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<Instant>("AddedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean");

                    b.Property<Instant?>("LastPostedTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("MinutesInterval")
                        .HasColumnType("integer");

                    b.Property<Instant>("ModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("ChatId");

                    b.ToTable("Scenes");
                });

            modelBuilder.Entity("WetPicsRebirth.Data.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<Instant>("AddedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<Instant>("ModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WetPicsRebirth.Data.Entities.UserAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<Instant>("AddedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ApiKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Instant>("ModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Source")
                        .HasColumnType("integer");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserAccounts");
                });

            modelBuilder.Entity("WetPicsRebirth.Data.Entities.Vote", b =>
                {
                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<Instant>("AddedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Instant>("ModifiedDate")
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

            modelBuilder.Entity("WetPicsRebirth.Data.Entities.UserAccount", b =>
                {
                    b.HasOne("WetPicsRebirth.Data.Entities.User", "User")
                        .WithMany("UserAccounts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
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
                    b.Navigation("UserAccounts");

                    b.Navigation("Votes");
                });
#pragma warning restore 612, 618
        }
    }
}
