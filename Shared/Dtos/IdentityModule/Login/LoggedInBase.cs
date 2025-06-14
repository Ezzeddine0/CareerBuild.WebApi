﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.IdentityModule.Login
{
	public abstract class LoggedInBase
	{
		public string UserName { get; set; } = default!;
		public string Email { get; set; } = default!;
		public string? PictureUrl { get; set; } = default!;
		public AddressDto Address { get; set; } = default!;
		public string Token { get; set; } = default!;
	}
}
