using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MarketingSystem.Core.Errors
{
    public class CoreException : Exception
    {
        public CoreError Error { get; set; }

        public string SerializedErrors => JsonConvert.SerializeObject(Error, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

        public CoreException()
        {
        }

        public CoreException(string field, string message)
        {
            Error = new CoreError(new List<CoreValidationError> { new CoreValidationError(field, message) });
        }

        public CoreException(string message, int statusCode = 400)
        {
            Error = new CoreError(new List<CoreValidationError> { new CoreValidationError(message) }, statusCode);
        }

        public CoreException(CoreValidationError validationError)
        {
            Error = new CoreError(new List<CoreValidationError> { validationError });
        }

        public CoreException(IEnumerable<CoreValidationError> validationErrors)
        {
            Error = new CoreError(validationErrors);
        }
    }
}