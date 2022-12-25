﻿using Microsoft.AspNetCore.Identity;
using NetDevPack.Domain;
using System;
using System.Collections.Generic;

namespace BookActivity.Domain.Models
{
    public sealed class AppUser : IdentityUser<Guid>, IAggregateRoot
    {
        /// <summary>
        /// Relation of user with the subscription info
        /// </summary>
        public ICollection<AppUser> Subscriptions { get; set; }

        /// <summary>
        /// Relation of user with the book opinios
        /// </summary>
        public IEnumerable<BookOpinion> BookOpinions { get; set; }

        /// <summary>
        /// Relation of user with the notifications
        /// </summary>
        public ICollection<UserNotification> UserNotifications { get; set; }

        /// <summary>
        /// User avatar
        /// </summary>
        public byte[] AvatarImage { get; set; }

        public AppUser() : base()
        {
            Subscriptions = new List<AppUser>();
            BookOpinions = new List<BookOpinion>();
            UserNotifications = new List<UserNotification>();
        }
    }
}