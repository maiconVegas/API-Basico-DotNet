using System.Net;
using FluentValidation.Results;

namespace WebApiDotNet.Core.Messaging;

public class Result
{
    public ValidationResult ValidationResult { get; set; }
    public HttpStatusCode? ErrorStatusCode { get; set; }
    public bool Success => !ErrorStatusCode.HasValue;
}

public class Result<T> : Result
{
    public T Value { get; set; }
}
