﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WebGallery.Data;

#nullable disable

namespace WebGallery.Data.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ArtworkHashtag", b =>
                {
                    b.Property<Guid>("ArtworksId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("HashtagsId")
                        .HasColumnType("uuid");

                    b.HasKey("ArtworksId", "HashtagsId");

                    b.HasIndex("HashtagsId");

                    b.ToTable("ArtworkHashtag", (string)null);
                });

            modelBuilder.Entity("WebGallery.Data.Entities.Artwork", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("AllowComments")
                        .HasColumnType("boolean");

                    b.Property<string>("CompressedFrontPictureUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsFeatured")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsOriginalWork")
                        .HasColumnType("boolean");

                    b.Property<int>("OpenTo")
                        .HasColumnType("integer");

                    b.Property<LocalDate>("PublishedAt")
                        .HasColumnType("date");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("TotalLikes")
                        .HasColumnType("bigint");

                    b.Property<long>("TotalViews")
                        .HasColumnType("bigint");

                    b.Property<Guid>("UserProfileId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserProfileId");

                    b.ToTable("Artwork", (string)null);
                });

            modelBuilder.Entity("WebGallery.Data.Entities.Bookmark", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ArtworkId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserProfileId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ArtworkId");

                    b.HasIndex("UserProfileId");

                    b.ToTable("Bookmark", (string)null);
                });

            modelBuilder.Entity("WebGallery.Data.Entities.Hashtag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("TotalUses")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Hashtag", (string)null);
                });

            modelBuilder.Entity("WebGallery.Data.Entities.Like", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ArtworkId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserProfileId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ArtworkId");

                    b.HasIndex("UserProfileId");

                    b.ToTable("Like", (string)null);
                });

            modelBuilder.Entity("WebGallery.Data.Entities.Picture", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ArtworkId")
                        .HasColumnType("uuid");

                    b.Property<string>("FullPictureUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ArtworkId");

                    b.ToTable("Picture", (string)null);
                });

            modelBuilder.Entity("WebGallery.Data.Entities.UserProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<LocalDate>("BirthDate")
                        .HasColumnType("date");

                    b.Property<Guid>("CognitoUserId")
                        .HasColumnType("uuid");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Gender")
                        .HasColumnType("integer");

                    b.Property<string>("Occupation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ProfileCoverPictureUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ProfilePictureUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TotalArtworks")
                        .HasColumnType("integer");

                    b.Property<int>("TotalFollowers")
                        .HasColumnType("integer");

                    b.Property<int>("TotalFollowing")
                        .HasColumnType("integer");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UserProfile", (string)null);
                });

            modelBuilder.Entity("ArtworkHashtag", b =>
                {
                    b.HasOne("WebGallery.Data.Entities.Artwork", null)
                        .WithMany()
                        .HasForeignKey("ArtworksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebGallery.Data.Entities.Hashtag", null)
                        .WithMany()
                        .HasForeignKey("HashtagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebGallery.Data.Entities.Artwork", b =>
                {
                    b.HasOne("WebGallery.Data.Entities.UserProfile", "UserProfile")
                        .WithMany("Artworks")
                        .HasForeignKey("UserProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserProfile");
                });

            modelBuilder.Entity("WebGallery.Data.Entities.Bookmark", b =>
                {
                    b.HasOne("WebGallery.Data.Entities.Artwork", "Artwork")
                        .WithMany()
                        .HasForeignKey("ArtworkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebGallery.Data.Entities.UserProfile", "UserProfile")
                        .WithMany()
                        .HasForeignKey("UserProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artwork");

                    b.Navigation("UserProfile");
                });

            modelBuilder.Entity("WebGallery.Data.Entities.Like", b =>
                {
                    b.HasOne("WebGallery.Data.Entities.Artwork", "Artwork")
                        .WithMany()
                        .HasForeignKey("ArtworkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebGallery.Data.Entities.UserProfile", "UserProfile")
                        .WithMany()
                        .HasForeignKey("UserProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artwork");

                    b.Navigation("UserProfile");
                });

            modelBuilder.Entity("WebGallery.Data.Entities.Picture", b =>
                {
                    b.HasOne("WebGallery.Data.Entities.Artwork", "Artwork")
                        .WithMany("Pictures")
                        .HasForeignKey("ArtworkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Artwork");
                });

            modelBuilder.Entity("WebGallery.Data.Entities.Artwork", b =>
                {
                    b.Navigation("Pictures");
                });

            modelBuilder.Entity("WebGallery.Data.Entities.UserProfile", b =>
                {
                    b.Navigation("Artworks");
                });
#pragma warning restore 612, 618
        }
    }
}
