using System.Text.Json.Serialization;
using FluentValidation.Results;
using MediatR;

namespace WebApiDotNet.Core.Messaging;

public abstract class BaseRequest<T> : IRequest<T> where T : Result
{
    [JsonIgnore]
    public ValidationResult ValidationResult { get; protected set; }

    public abstract bool EhValido();
}
