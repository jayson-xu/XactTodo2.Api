﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using XactTodo.Infrastructure;

namespace XactTodo.Infrastructure.Migrations
{
    [DbContext(typeof(TodoContext))]
    [Migration("20210113154650_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("XactTodo.Domain.AggregatesModel.IdentityAggregate.Identity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AccessToken")
                        .HasMaxLength(32)
                        .HasColumnType("varchar(32) CHARACTER SET utf8mb4");

                    b.Property<int>("ExpiresIn")
                        .HasColumnType("int");

                    b.Property<bool>("Invalid")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("IssueTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4");

                    b.Property<string>("RefreshToken")
                        .HasMaxLength(32)
                        .HasColumnType("varchar(32) CHARACTER SET utf8mb4");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Identity");
                });

            modelBuilder.Entity("XactTodo.Domain.AggregatesModel.MatterAggregate.Evolvement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500) CHARACTER SET utf8mb4");

                    b.Property<DateTime>("CreationTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasDefaultValueSql("sysdate()");

                    b.Property<int>("CreatorUserId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<int>("MatterId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatorUserId");

                    b.HasIndex("MatterId");

                    b.ToTable("Evolvement");
                });

            modelBuilder.Entity("XactTodo.Domain.AggregatesModel.MatterAggregate.Matter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("CameFrom")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("CreationTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasDefaultValueSql("sysdate()");

                    b.Property<int>("CreatorUserId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Deadline")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("DeleterUserId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletionTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("ExecutantId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("FinishTime")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("Finished")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Importance")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("LastModifierUserId")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.Property<bool>("Periodic")
                        .HasColumnType("tinyint(1)");

                    b.Property<int?>("RelatedMatterId")
                        .HasColumnType("int");

                    b.Property<string>("Remark")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500) CHARACTER SET utf8mb4");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4");

                    b.Property<int?>("TeamId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatorUserId");

                    b.HasIndex("Deadline");

                    b.HasIndex("Importance");

                    b.HasIndex("Subject");

                    b.HasIndex("TeamId");

                    b.ToTable("Matter");
                });

            modelBuilder.Entity("XactTodo.Domain.AggregatesModel.MatterTagAggregate.MatterTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("MatterId")
                        .HasColumnType("int");

                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("MatterId", "UserId", "Tag")
                        .IsUnique();

                    b.ToTable("MatterTag");
                });

            modelBuilder.Entity("XactTodo.Domain.AggregatesModel.TeamAggregate.Member", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasDefaultValueSql("sysdate()");

                    b.Property<int>("CreatorUserId")
                        .HasColumnType("int");

                    b.Property<int?>("DeleterUserId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletionTime")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsSupervisor")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("LastModifierUserId")
                        .HasColumnType("int");

                    b.Property<int>("TeamId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("TeamId", "UserId")
                        .IsUnique()
                        .HasFilter("IsDeleted=0");

                    b.ToTable("Member");
                });

            modelBuilder.Entity("XactTodo.Domain.AggregatesModel.TeamAggregate.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasDefaultValueSql("sysdate()");

                    b.Property<int>("CreatorUserId")
                        .HasColumnType("int");

                    b.Property<int?>("DeleterUserId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletionTime")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("LastModifierUserId")
                        .HasColumnType("int");

                    b.Property<int>("LeaderId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4");

                    b.Property<string>("ProposedTags")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("CreatorUserId");

                    b.HasIndex("LeaderId");

                    b.HasIndex("Name", "CreatorUserId")
                        .IsUnique()
                        .HasFilter("IsDeleted=0");

                    b.ToTable("Team");
                });

            modelBuilder.Entity("XactTodo.Domain.AggregatesModel.UserAggregate.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasDefaultValueSql("sysdate()");

                    b.Property<int?>("CreatorUserId")
                        .HasColumnType("int");

                    b.Property<int?>("DeleterUserId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletionTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("LastModifierUserId")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128) CHARACTER SET utf8mb4");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("CreatorUserId");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasFilter("IsDeleted=0");

                    b.HasIndex("UserName")
                        .IsUnique()
                        .HasFilter("IsDeleted=0");

                    b.ToTable("User");
                });

            modelBuilder.Entity("XactTodo.Domain.AggregatesModel.MatterAggregate.Evolvement", b =>
                {
                    b.HasOne("XactTodo.Domain.AggregatesModel.UserAggregate.User", null)
                        .WithMany()
                        .HasForeignKey("CreatorUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("XactTodo.Domain.AggregatesModel.MatterAggregate.Matter", null)
                        .WithMany("Evolvements")
                        .HasForeignKey("MatterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("XactTodo.Domain.AggregatesModel.MatterAggregate.Matter", b =>
                {
                    b.HasOne("XactTodo.Domain.AggregatesModel.UserAggregate.User", null)
                        .WithMany()
                        .HasForeignKey("CreatorUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("XactTodo.Domain.AggregatesModel.TeamAggregate.Team", null)
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.OwnsOne("XactTodo.Domain.AggregatesModel.MatterAggregate.PeriodOfTime", "EstimatedTimeRequired", b1 =>
                        {
                            b1.Property<int>("MatterId")
                                .HasColumnType("int");

                            b1.Property<decimal>("Num")
                                .HasColumnType("decimal(9,1)");

                            b1.Property<int>("Unit")
                                .HasColumnType("int");

                            b1.HasKey("MatterId");

                            b1.ToTable("Matter");

                            b1.WithOwner()
                                .HasForeignKey("MatterId");
                        });

                    b.OwnsOne("XactTodo.Domain.AggregatesModel.MatterAggregate.PeriodOfTime", "IntervalPeriod", b1 =>
                        {
                            b1.Property<int>("MatterId")
                                .HasColumnType("int");

                            b1.Property<decimal>("Num")
                                .HasColumnType("decimal(9,1)");

                            b1.Property<int>("Unit")
                                .HasColumnType("int");

                            b1.HasKey("MatterId");

                            b1.ToTable("Matter");

                            b1.WithOwner()
                                .HasForeignKey("MatterId");
                        });

                    b.Navigation("EstimatedTimeRequired");

                    b.Navigation("IntervalPeriod");
                });

            modelBuilder.Entity("XactTodo.Domain.AggregatesModel.MatterTagAggregate.MatterTag", b =>
                {
                    b.HasOne("XactTodo.Domain.AggregatesModel.MatterAggregate.Matter", null)
                        .WithMany()
                        .HasForeignKey("MatterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("XactTodo.Domain.AggregatesModel.UserAggregate.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("XactTodo.Domain.AggregatesModel.TeamAggregate.Member", b =>
                {
                    b.HasOne("XactTodo.Domain.AggregatesModel.TeamAggregate.Team", null)
                        .WithMany("Members")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("XactTodo.Domain.AggregatesModel.UserAggregate.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("XactTodo.Domain.AggregatesModel.TeamAggregate.Team", b =>
                {
                    b.HasOne("XactTodo.Domain.AggregatesModel.UserAggregate.User", null)
                        .WithMany()
                        .HasForeignKey("CreatorUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("XactTodo.Domain.AggregatesModel.UserAggregate.User", null)
                        .WithMany()
                        .HasForeignKey("LeaderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("XactTodo.Domain.AggregatesModel.UserAggregate.User", b =>
                {
                    b.HasOne("XactTodo.Domain.AggregatesModel.UserAggregate.User", null)
                        .WithMany()
                        .HasForeignKey("CreatorUserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("XactTodo.Domain.AggregatesModel.MatterAggregate.Matter", b =>
                {
                    b.Navigation("Evolvements");
                });

            modelBuilder.Entity("XactTodo.Domain.AggregatesModel.TeamAggregate.Team", b =>
                {
                    b.Navigation("Members");
                });
#pragma warning restore 612, 618
        }
    }
}
