﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace BookActivity.Application.Models.Dto.Create
{
    public sealed class CreateBookDto : BaseCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<Guid> AuthorIds { get; set; }
        public IFormFile Image { get; set; }

        public override void Validate()
        {
            
        }
    }
}