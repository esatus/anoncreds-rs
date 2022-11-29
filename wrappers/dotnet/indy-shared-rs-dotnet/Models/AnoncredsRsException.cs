using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace anoncreds_rs_dotnet.Models
{
    public class AnoncredsRsException : Exception
    {
        public ErrorCode errorCode;

        public AnoncredsRsException(string message) : base(message)
        {

        }

        public AnoncredsRsException(string message, ErrorCode code) : base(message)
        {
            errorCode = code;
        }

        public static AnoncredsRsException FromSdkError(string message)
        {
            string msg = JsonConvert.DeserializeObject<Dictionary<string, string>>(message)["message"];
            string errCode = JsonConvert.DeserializeObject<Dictionary<string, string>>(message)["code"];
            return int.TryParse(errCode, out int errCodeInt)
                ? new AnoncredsRsException(
                    $"'{((ErrorCode)errCodeInt).ToErrorCodeString()}' error occured with ErrorCode '{errCode}' : {msg}.", (ErrorCode)errCodeInt)
                : new AnoncredsRsException("An unknown error code was received.", (ErrorCode)errCodeInt);
        }
    }
}