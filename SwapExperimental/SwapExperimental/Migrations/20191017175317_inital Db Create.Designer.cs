﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Swap.WebApi.Entities;

namespace Swap.WebApi.Migrations
{
    [DbContext(typeof(SwapContext))]
    [Migration("20191017175317_inital Db Create")]
    partial class initalDbCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Swap.Object.ChatObjects.Chat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("Swap.Object.ChatObjects.InstantMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Body");

                    b.Property<Guid>("ChatId");

                    b.Property<DateTime>("DateTime");

                    b.Property<int>("UserId");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("UserId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("Swap.Object.ChatObjects.UserToGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("Guid");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("Guid");

                    b.HasIndex("UserId");

                    b.ToTable("UserToGroup");
                });

            modelBuilder.Entity("Swap.Object.GeneralObjects.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ItemId");

                    b.Property<string>("Path");

                    b.Property<int?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("UserId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("Swap.Object.GeneralObjects.Trade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ItemId");

                    b.Property<string>("ItemsToTrade");

                    b.Property<int>("OfferedById");

                    b.Property<int>("OfferedToId");

                    b.Property<int?>("Status");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("OfferedById");

                    b.HasIndex("OfferedToId");

                    b.ToTable("Trades");
                });

            modelBuilder.Entity("Swap.Object.GeneralObjects.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CellPhone");

                    b.Property<string>("ChatConnectionId");

                    b.Property<string>("City");

                    b.Property<string>("Email");

                    b.Property<string>("FirebaseToken");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<string>("Password");

                    b.Property<DateTime>("SignUpDate");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Swap.Object.Items.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Condition");

                    b.Property<string>("Description")
                        .HasMaxLength(300);

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Genre");

                    b.Property<int>("IdCustomer");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UploadDate");

                    b.Property<int>("Views");

                    b.HasKey("Id");

                    b.HasIndex("IdCustomer");

                    b.ToTable("Items");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Item");
                });

            modelBuilder.Entity("Swap.Object.Items.Book", b =>
                {
                    b.HasBaseType("Swap.Object.Items.Item");

                    b.Property<string>("Author");

                    b.Property<short>("Pages");

                    b.ToTable("Book");

                    b.HasDiscriminator().HasValue("Book");
                });

            modelBuilder.Entity("Swap.Object.Items.VideoGame", b =>
                {
                    b.HasBaseType("Swap.Object.Items.Item");

                    b.Property<int>("Platform");

                    b.ToTable("VideoGame");

                    b.HasDiscriminator().HasValue("VideoGame");
                });

            modelBuilder.Entity("Swap.Object.ChatObjects.InstantMessage", b =>
                {
                    b.HasOne("Swap.Object.ChatObjects.Chat", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Swap.Object.GeneralObjects.User", "User")
                        .WithMany("Messages")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Swap.Object.ChatObjects.UserToGroup", b =>
                {
                    b.HasOne("Swap.Object.ChatObjects.Chat", "Chat")
                        .WithMany("UsersToGroup")
                        .HasForeignKey("Guid")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Swap.Object.GeneralObjects.User", "User")
                        .WithMany("Groups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Swap.Object.GeneralObjects.Image", b =>
                {
                    b.HasOne("Swap.Object.Items.Item", "Item")
                        .WithMany("ImagesOfItem")
                        .HasForeignKey("ItemId");

                    b.HasOne("Swap.Object.GeneralObjects.User", "User")
                        .WithMany("Images")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Swap.Object.GeneralObjects.Trade", b =>
                {
                    b.HasOne("Swap.Object.Items.Item", "OfferedItem")
                        .WithMany("Trades")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Swap.Object.GeneralObjects.User", "OfferedBy")
                        .WithMany()
                        .HasForeignKey("OfferedById")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Swap.Object.GeneralObjects.User", "OfferedTo")
                        .WithMany("Trades")
                        .HasForeignKey("OfferedToId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Swap.Object.Items.Item", b =>
                {
                    b.HasOne("Swap.Object.GeneralObjects.User", "Owner")
                        .WithMany("ItemsOfUser")
                        .HasForeignKey("IdCustomer")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}