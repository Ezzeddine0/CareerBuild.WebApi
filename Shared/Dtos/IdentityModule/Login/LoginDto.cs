﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.IdentityModule.Login
{
	public class LoginDto
	{
		[EmailAddress]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; } = default!;

		public string Password { get; set; } = default!;
	}
}
