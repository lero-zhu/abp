﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Volo.Abp.Aspects;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.AspNetCore.Mvc.Validation
{
    public class AbpValidationActionFilter : IAsyncActionFilter, ITransientDependency
    {
        private readonly ModelStateValidator _validator;

        public AbpValidationActionFilter(ModelStateValidator validator)
        {
            _validator = validator;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //TODO: Configuration to disable validation for controllers..?

            if (!context.ActionDescriptor.IsControllerAction() ||
                !ActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
            {
                await next();
                return;
            }

            using (AbpCrossCuttingConcerns.Applying(context.Controller, AbpCrossCuttingConcerns.Validation))
            {
                _validator.Validate(context.ModelState);
                await next();
            }
        }
    }
}
