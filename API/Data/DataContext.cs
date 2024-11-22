using System;
using API.Entities;
using API.Extensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }

    public DbSet<UserLike> Likes { get; set; }

    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Apply any base class configurations first.
        base.OnModelCreating(builder);

        // Set a composite primary key using SourceUserId and TargetUserId.
        // This makes sure each "like" is unique between two users.
        builder.Entity<UserLike>()
            .HasKey(k => new { k.SourceUserId, k.TargetUserId });

        // Set up the relationship where a UserLike belongs to one SourceUser.
        // A SourceUser can have many likes (from other users).
        builder.Entity<UserLike>()
            .HasOne(s => s.SourceUser)  // Each UserLike has one SourceUser (the one who is liking).
            .WithMany(l => l.LikeUsers)  // A SourceUser can have many "likes".
            .HasForeignKey(s => s.SourceUserId)  // SourceUserId is the foreign key in UserLike.
            .OnDelete(DeleteBehavior.Cascade);  // If SourceUser is deleted, delete their likes too.

        // Set up the relationship where a UserLike belongs to one TargetUser.
        // A TargetUser can be liked by many SourceUsers.
        builder.Entity<UserLike>()
            .HasOne(s => s.TargetUser)  // Each UserLike has one TargetUser (the one being liked).
            .WithMany(l => l.LikedByUsers)  // A TargetUser can be liked by many SourceUsers.
            .HasForeignKey(s => s.TargetUserId)  // TargetUserId is the foreign key in UserLike.
            .OnDelete(DeleteBehavior.Cascade);  // If TargetUser is deleted, delete their likes too.


        builder.Entity<Message>()
        .HasOne(s => s.Recipient)
        .WithMany(x => x.MessagesRecieved)
        .OnDelete(DeleteBehavior.Restrict);

        
        builder.Entity<Message>()
        .HasOne(s => s.Sender)
        .WithMany(x => x.MessagesSent)
        .OnDelete(DeleteBehavior.Restrict);


    }
}

//What is entities framework
//It is object relational Mapper (ORM)
//translate our code into SQL commands that update our tables in database
//DbContext is an Entitry Framework that act as bridge between
//Entities and Database

//Features
//Querying, Change Tracking, Saving, Concurrency, Transaction
//Caching, built-in conventions, configuration, migrations
