﻿using System.Collections;
using System.Linq;
using System.Web.Mvc;

namespace TaxiCameBack.Website.Application.Extension
{
    public static class ModelStateExtensions
    {
        public static IEnumerable Errors(this ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                return modelState.ToDictionary(kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()).Where(m => m.Value.Any());
            }
            return null;
        }

    }
}