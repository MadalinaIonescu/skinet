using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Errors
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string message =null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatucCode(statusCode);
        }

        public int StatusCode { get; set; }
        public string Message {get; set; }

         private string GetDefaultMessageForStatucCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "A bad request, you have made",
                401 => "Authorized, you are not",
                403 => "Forbidden from doing this, you are",
                404 => "Resource found, it was not",
                500 => "Errors are a path to the dark side. Errors lead to anger. Anger leads to hate. Hate leads to carrrer change.",
                _ => null
            };
        }

    }
}