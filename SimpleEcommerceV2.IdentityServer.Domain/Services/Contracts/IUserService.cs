﻿using SimpleEcommerceV2.Shared.InOut.Requests;

namespace SimpleEcommerceV2.IdentityServer.Domain.Services.Contracts
{
    public interface IUserService
    {
        Task<bool> CheckPasswordAsync(AuthRequest userRequest);
    }
}
