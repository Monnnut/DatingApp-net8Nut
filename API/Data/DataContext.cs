using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
}

//What is entities framework
//It is object relational Mapper (ORM)
//translate our code into SQL commands that update our tables in database
//DbContext is an Entitry Framework that act as bridge between
//Entities and Database

//Features
//Querying, Change Tracking, Saving, Concurrency, Transaction
//Caching, built-in conventions, configuration, migrations
