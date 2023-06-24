﻿using Antanidoss.Specification.Abstract;
using BookActivity.Infrastructure.Data.Exceptions;
using NetDevPack.Domain;

namespace BookActivity.Infrastructure.Data.Validations
{
    internal static class SpecificationValidator
    {
        public static void ThrowExceptionIfNull<TEntity>(Specification<TEntity> specification) where TEntity : class
        {
            if (specification == null)
                throw new SpecificationNullException(nameof(TEntity));
        } 
    }
}