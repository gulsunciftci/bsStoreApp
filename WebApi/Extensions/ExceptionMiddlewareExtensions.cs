using Entities.ErrorModel;
using Entities.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Services.Contracts;
using System.Net;

namespace WebApi.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        //Extensions metotlar static olarak tanımlanır static classların bütün üyeleri static olur
        public static void ConfigureExceptionHandler(this WebApplication webApplication,ILoggerService loggerService)
        {
            webApplication.UseExceptionHandler(appError=>
            {
                appError.Run(async context=>
                {
                    context.Response.ContentType = "application/json"; //neyle çalıştığımı söyledim
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>(); //hata yakaladım
                    if(contextFeature is not null) //hata boş değilse
                    {
                        context.Response.StatusCode = contextFeature.Error switch
                        {
                            NotFoundException => StatusCodes.Status404NotFound,
                            _=> StatusCodes.Status500InternalServerError

                        };


                        loggerService.LogError($"Something went wrong: {contextFeature.Error}");
                        await context.Response.WriteAsync(new ErrorDetails()
                        {

                            StatusCode = context.Response.StatusCode,
                            Message=contextFeature.Error.Message

                        }.ToString()) ;
                    }
                });
            });
        }
    }
}
