﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entities.ErrorModel
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; } //boş olabilir(?)
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);

            ///<summary>
            ///Serialize:
            /// {
            /// message:" ,,,,,"
            /// statusCode:200
            /// }
            /// </summary>
        }
    }
}
