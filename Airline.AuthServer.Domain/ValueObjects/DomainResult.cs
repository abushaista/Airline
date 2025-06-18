using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.AuthServer.Domain.ValueObjects;
public class DomainResult
{
    public bool Succeeded { get; init; }
    public IEnumerable<string> Errors { get; init; } = new List<string>();

    public static DomainResult Success() => new() { Succeeded = true };
    public static DomainResult Failed(IEnumerable<string> errors) => new() { Succeeded = false, Errors = errors };
}
