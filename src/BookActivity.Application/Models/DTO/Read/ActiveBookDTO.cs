﻿using System;
using System.Collections.Generic;

namespace BookActivity.Application.Models.DTO.Read
{
    public sealed class ActiveBookDTO : BaseEntityDTO
    {
        public int TotalNumberPages { get; set; }
        public int NumberPagesRead { get; set; }
        public string UserName { get; set; }
        public Guid UserId { get; set; }
        public ICollection<BookNoteDTO> BookNotes { get; set; }
        public BookDTO Book { get; set; }
        public ActiveBookDTO() : base() { }
    }
}