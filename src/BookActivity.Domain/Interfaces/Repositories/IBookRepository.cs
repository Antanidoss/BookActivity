﻿using BookActivity.Domain.Filters.Models;
using BookActivity.Domain.Models;
using NetDevPack.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookActivity.Domain.Interfaces.Repositories
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> GetByFilterAsync(BookFilterModel filterModel);
        Task<int> GetCountByFilterAsync(BookFilterModel filterModel);

        void Add(Book book);
        void Update(Book book);
        void Remove(Book book);
    }
}