using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace anoncreds_rs_dotnet.Models
{
    public class AnoncredsRsException : Exception
    {
        public AnoncredsRsException(string message) : base(message)
        {

        }

        public static AnoncredsRsException FromSdkError(string message)
        {
            string msg = JsonConvert.DeserializeObject<Dictionary<string, string>>(message)["message"];
            string errCode = JsonConvert.DeserializeObject<Dictionary<string, string>>(message)["code"];
            return int.TryParse(errCode, out int errCodeInt)
                ? new AnoncredsRsException(
                    $"'{((ErrorCode)errCodeInt).ToErrorCodeString()}' error occured with ErrorCode '{errCode}' : {msg}.")
                : new AnoncredsRsException("An unknown error code was received.");
        }
    }
}