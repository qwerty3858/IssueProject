using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IssueProject.Helpers.TokenOperations
{
    public class AuthClient
    {
        
            public string Id { get; set; }
            public string KeyId { get; set; }
            public string Issuer { get; set; }
            public string Secret { get; set; }

            public bool ValidateLifeTime { get; set; } = true;
            public int TokenLifeTime { get; set; }
            public int RefreshTokenLifeTime { get; set; }

    }
}