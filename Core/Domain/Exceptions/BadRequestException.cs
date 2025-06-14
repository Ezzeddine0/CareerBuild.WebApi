﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
	public sealed class BadRequestException(List<string> error) : Exception("Validation Errors")
	{
		public List<string> Errors { get; set; } = error;
	}
}
