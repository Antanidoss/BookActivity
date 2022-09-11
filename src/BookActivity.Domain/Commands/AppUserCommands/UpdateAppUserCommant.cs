﻿using System;

namespace BookActivity.Domain.Commands.AppUserCommands
{
    public class UpdateAppUserCommand : AppUserCommand
    {
        public UpdateAppUserCommand() { }
        public UpdateAppUserCommand(Guid appUserId, byte[] avatarImage)
        {
            AppUserId = appUserId;
            AvatarImage = avatarImage;
        }
    }
}