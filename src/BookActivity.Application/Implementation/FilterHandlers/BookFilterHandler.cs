﻿using BookActivity.Domain.Filters.Models;
using BookActivity.Domain.Interfaces.Filters;
using BookActivity.Domain.Models;
using System.Linq;

namespace BookActivity.Application.Implementation.FilterHandlers
{
    internal sealed class BookFilterHandler : IFilterHandler<Book, BookFilterModel>
    {
        public IQueryable<Book> Handle(BookFilterModel bookFilterModel, IQueryable<Book> query)
        {
            if (query == null) return null;

            if (bookFilterModel.BookIds != null)
                query = bookFilterModel.BookIds.FilterSpec.ApplyFilter(query, bookFilterModel.BookIds.Value);

            if (bookFilterModel.Title != null)
                query = bookFilterModel.Title.FilterSpec.ApplyFilter(query, bookFilterModel.Title.Value);

            return query.Skip(bookFilterModel.Skip).Take(bookFilterModel.Take);
        }
    }
}
