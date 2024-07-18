using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOS
{
    public class LoginTokenDto
    {
        public int UserAccountId { get; set; }

        public string UserPassword { get; set; } = null!;

        public string UserFullName { get; set; } = null!;

        public string? UserEmail { get; set; }

        public int? Role { get; set; }

        public string Token { get; set; }
    }
}
