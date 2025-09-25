using System;

namespace Marventa.Framework.Infrastructure.Http;

public class CircuitBreakerOpenException : Exception
{
    public CircuitBreakerOpenException(string message) : base(message) { }
}