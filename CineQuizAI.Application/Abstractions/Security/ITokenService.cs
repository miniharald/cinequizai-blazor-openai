using System;
using System.Collections.Generic;

namespace CineQuizAI.Application.Abstractions.Security
{
    // TODO: extend with refresh tokens, roles, etc.
    public interface ITokenService
    {
        string CreateToken(Guid userId, string userName, IEnumerable<string>? roles = null);
    }
}
