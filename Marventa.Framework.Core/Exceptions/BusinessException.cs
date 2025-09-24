using System;

namespace Marventa.Framework.Core.Exceptions;

public class BusinessException : Exception
{
    public string Code { get; }

    public BusinessException(string message) : base(message)
    {
        Code = "BUSINESS_ERROR";
    }

    public BusinessException(string code, string message) : base(message)
    {
        Code = code;
    }

    public BusinessException(string message, Exception innerException) : base(message, innerException)
    {
        Code = "BUSINESS_ERROR";
    }

    public BusinessException(string code, string message, Exception innerException) : base(message, innerException)
    {
        Code = code;
    }
}