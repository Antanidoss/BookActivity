﻿using BookActivity.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using BookActivity.Domain.Interfaces;
using BookActivity.Infrastructure.Data.EF.Configuration;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using BookActivity.Domain.Core;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using BookActivity.Shared.Extensions;

namespace BookActivity.Infrastructure.Data.EF
{
    public sealed class BookActivityContext : IdentityDbContext<AppUser, AppRole, Guid>, IUnitOfWork
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<ActiveBook> ActiveBooks { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BookOpinion> BookOpinions { get; set; }
        public DbSet<BookNote> BookNotes { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        private readonly IMediatorHandler _mediatorHandler;
        private readonly IConfiguration _configuration;

        public BookActivityContext(IMediatorHandler mediatorHandler, IConfiguration configuration) : base()
        {
            _mediatorHandler = mediatorHandler;
            _configuration = configuration;
        }

        public async Task<bool> Commit()
        {
            var domainEntities = ChangeTracker
                .Entries<BaseEntity>()
                .Where(x => x.State != EntityState.Unchanged && x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .Cast<Event>()
                .ToList();

            using CancellationTokenSource cancellationTokenSource = new();
            var cancellationToken = cancellationTokenSource.Token;

            await _mediatorHandler.PublishEventsAsync(domainEvents.Where(e => e.WhenCallHandler == WhenCallHandler.BeforeOperation), cancellationToken);

            var success = await SaveChangesAsync() > 0;

            if (success)
                await _mediatorHandler.PublishEventsAsync(domainEvents.Where(e => e.WhenCallHandler == WhenCallHandler.AfterOperation), cancellationToken);

            return success;
        }

        public override int SaveChanges()
        {
            UpdateCreationAndUpdateTime();

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            UpdateCreationAndUpdateTime();

            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<ValidationResult>();
            modelBuilder.Ignore<Event>();

            modelBuilder.ApplyConfiguration(new AppUserConfiguration());
            modelBuilder.ApplyConfiguration(new SubscriberConfiguration());
            modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());
            modelBuilder.ApplyConfiguration(new BookConfiguration());
            modelBuilder.ApplyConfiguration(new AuthorConfiguration());
            modelBuilder.ApplyConfiguration(new BookNoteConfiguration());
            modelBuilder.ApplyConfiguration(new BookOpinionConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        //TODO: Logs doesn't work
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(_configuration.GetConnectionString("DefaultConnection"))
                .EnableSensitiveDataLogging(_configuration.GetUseSqlLogs());
        }

        private void UpdateCreationAndUpdateTime()
        {
            var addedEntities = ChangeTracker
                .Entries()
                .Where(e => e.State == EntityState.Added && e.Entity is BaseEntity);

            foreach (var entry in addedEntities)
            {
                var timeOfCreate = entry.Property(nameof(BaseEntity.TimeOfCreation)).CurrentValue;

                if (timeOfCreate == null || DateTime.Parse(timeOfCreate.ToString()).Year < DateTime.UtcNow.Year)
                    entry.Property(nameof(BaseEntity.TimeOfCreation)).CurrentValue = DateTime.UtcNow;
            }

            var updateEntities = ChangeTracker
                .Entries()
                .Where(e => e.State == EntityState.Modified && e.Entity is BaseEntity);

            foreach (var entry in updateEntities)
            {
                var timeOfUpdate = entry.Property(nameof(BaseEntity.TimeOfUpdate)).CurrentValue;

                if (timeOfUpdate == null || DateTime.Parse(timeOfUpdate.ToString()).Year < DateTime.UtcNow.Year)
                    entry.Property(nameof(BaseEntity.TimeOfUpdate)).CurrentValue = DateTime.UtcNow;
            }
        }
    }
}