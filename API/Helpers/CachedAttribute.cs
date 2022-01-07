using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveInsSeconds;
        public CachedAttribute(int timeToLiveInsSeconds)
        {
            _timeToLiveInsSeconds = timeToLiveInsSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //ne luam serviciul de cache
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            //ne generam cacheKey pe baza requestului
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            //cautam daca exista deja cacheuit response-ul
            var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

            //daca este deja cacheu-it response-ul i-l dam lui context si iesim
            if(!string.IsNullOrEmpty(cachedResponse))
            {
                var contentResult = new ContentResult
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                context.Result = contentResult;
                return;
            }

            //nu e cacheuit response-ul, mergem in controller
            var executedContext = await next(); //move to controller

            if(executedContext.Result is OkObjectResult okObjectResult)
            {
                // am ajuns ok in controller, am exxecutat de acolo si a returnat un ok, cacheuim ce a returnat
                await cacheService.CacheResponseAsync(cacheKey,okObjectResult.Value,
                 TimeSpan.FromSeconds(_timeToLiveInsSeconds));
            }


        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();

            keyBuilder.Append($"{request.Path}");

            foreach (var (key, value) in request.Query.OrderBy(x =>x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }

            return keyBuilder.ToString();
        }
    }
}