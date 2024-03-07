﻿using BookActivity.Domain.Models;
using BookActivity.Infrastructure.Data.EF;
using BookActivity.Infrastructure.Data.Intefaces;
using BookActivity.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver.Linq;
using BookActivity.Infrastructure.Data.Graphql.Extensions;
using BookActivity.Infrastructure.Data.Graphql.Queries;
using BookActivity.Domain.Interfaces;

namespace BookActivity.Infrastructure.Data
{
    public sealed class ModuleConfiguration : IModuleConfiguration
    {
        public IServiceCollection ConfigureDI(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDbContext, BookActivityContext>();
            services.AddDbContext<BookActivityContext>();

            services.AddIdentity<AppUser, AppRole>(option =>
            {
                option.User.RequireUniqueEmail = true;
                option.Password.RequireDigit = false;
                option.Password.RequiredLength = 6;
                option.Password.RequireUppercase = false;
                option.Password.RequireLowercase = false;
            }).AddEntityFrameworkStores<BookActivityContext>().AddDefaultTokenProviders();

            ConfigureMongoDb(services, configuration);
            ConfigureGraphQL(services);

            return services;
        }

        public void CreateDatabasesIfNotExist(IServiceScope serviceScope, IConfiguration configuration)
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<IDbContext>() as DbContext;

            if (!configuration.GetValue<bool>("UseInMemoryDatabase") && context.Database.CanConnect())
                return;

            context.Database.EnsureCreated();

            if (context is BookActivityContext)
            {
                var initializer = serviceScope.ServiceProvider.GetService<IDbInitializer>();
                if (initializer != null)
                {
                    initializer.InitializeAsync(context as BookActivityContext, serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>()).Wait();
                }
            }
        }

        private void ConfigureGraphQL(IServiceCollection services)
        {
            services.AddGraphQLServer()
                .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true)
                .AddQueryType(q => q.Name("Query"))
                .AddType<BookQuery>()
                .AddType<BookNoteQuery>()
                .AddType<ActiveBookQuery>()
                .AddType<UserQuery>()
                .AddType<BookOpinionQuery>()
                .AddType<AuthorQuery>()
                .AddType<NotificationQuery>()
                .AddProjections()
                .AddFiltering()
                .AddSorting()
                .AddTypeExtension<BookExtensions>()
                .AddTypeExtension<BookOpinionExtensions>()
                .AddTypeExtension<UserExtensions>();
        }

        private void ConfigureMongoDb(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("EventStoreConnection");
            MongoClientSettings settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.LinqProvider = LinqProvider.V3;

            services.AddSingleton(new MongoClient(settings).GetDatabase("BookActivityEvent"));

            ObjectSerializer objectSerializer = new(type => ObjectSerializer.DefaultAllowedTypes(type) || type.FullName.Contains(nameof(BookActivity)));
            BsonSerializer.RegisterSerializer(objectSerializer);

            GuidSerializer guidSerializer = new(BsonType.String);
            BsonSerializer.RegisterSerializer(guidSerializer);

            DateTimeSerializer dateTimeSerializer = new(BsonType.DateTime);
            BsonSerializer.RegisterSerializer(dateTimeSerializer);

            ConventionPack conventionPack = new() { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);
        }
    }
}
