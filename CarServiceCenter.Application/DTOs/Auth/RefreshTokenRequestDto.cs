using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarServiceCenter.Application.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        public string Token { get; set; } = string.Empty;
    }
}