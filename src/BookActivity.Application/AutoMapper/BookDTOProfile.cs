﻿using AutoMapper;
using BookActivity.Application.Extensions;
using BookActivity.Application.Models.DTO.Create;
using BookActivity.Application.Models.DTO.Read;
using BookActivity.Application.Models.DTO.Update;
using BookActivity.Domain.Commands.BookCommands.AddBook;
using BookActivity.Domain.Commands.BookCommands.UpdateBook;
using BookActivity.Domain.Models;

namespace BookActivity.Application.AutoMapper
{
    internal sealed class BookDTOProfile : Profile
    {
        public BookDTOProfile()
        {
            CreateMap<Book, BookDTO>();
            CreateMap<BookDTO, Book>();

            CreateMap<CreateBookDTO, AddBookCommand>()
                .ForMember(b => b.ImageData, conf => conf.MapFrom(b => b.Image.ConvertToBuffer()));
            CreateMap<UpdateBookDTO, UpdateBookCommand>();
        }
    }
}