using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airline.AuthServer.Domain.Enums;
public enum LoginResult
{
    Succeeded,
    RequiresTwoFactor,
    IsLockedOut,
    InvalidCredentials
}
