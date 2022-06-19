﻿using BookActivity.Api.Common.Constants;
using BookActivity.Api.Common.Extansions;
using BookActivity.Api.Common.Models;
using BookActivity.Application.Interfaces;
using BookActivity.Application.Models.DTO;
using BookActivity.Application.Models.DTO.Create;
using BookActivity.Application.Models.DTO.Read;
using BookActivity.Application.Models.DTO.Update;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookActivity.Api.Controllers
{
    [Route(ApiConstants.ActiveBookService)]
    public sealed class ActiveBookController : BaseController
    {
        private readonly IActiveBookService _activeBookService;

        public ActiveBookController(IActiveBookService activeBookService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _activeBookService = activeBookService;
        }

        [HttpPost(ApiConstants.AddActiveBookMethod)]
        public async Task<ActionResult> AddActiveBookAsync(CreateActiveBookDTO createActiveBookModel)
        {
            createActiveBookModel.UserId = /*GetCurrentUser().Id;*/ Guid.Parse("B13246FB-402D-4A55-C73B-08DA3B53C767");

            return (await _activeBookService.AddActiveBookAsync(createActiveBookModel)).ToActionResult();
        }

        [HttpPut(ApiConstants.UpdateNumberPagesReadMethod)]
        public async Task<ActionResult> UpdateNumberPagesReadAsync(UpdateNumberPagesReadDTO updateActiveBookModel)
        {
            return (await _activeBookService.UpdateActiveBookAsync(updateActiveBookModel)).ToActionResult();
        }

        [HttpDelete(ApiConstants.RemoveActiveBookMethod)]
        public async Task<ActionResult> RemoveActiveBookAsync(Guid activeBookId)
        {
            return (await _activeBookService.RemoveActiveBookAsync(activeBookId)).ToActionResult();
        }

        [HttpGet(ApiConstants.GetActiveBooksByIdsMethod)]
        public async Task<ApiResult<IEnumerable<ActiveBookDTO>>> GetaActiveBooksByIdsAsync(Guid[] activeBookIds)
        {
            return (await _activeBookService.GetByActiveBookIdFilterAsync(new PaginationModel
            {
                Take = activeBookIds.Length
            }, activeBookIds)).ToApiResult();
        }

        [HttpGet(ApiConstants.GetActiveBooksByUserIdMethod)]
        public async Task<ApiResult<IEnumerable<ActiveBookDTO>>> GetaActiveBooksByUserIdsAsync(Guid userId, PaginationModel filterModel)
        {
            return (await _activeBookService.GetByUserIdFilterAsync(filterModel, userId)).ToApiResult();
        }
    }
}