﻿using Shared.Dtos.SkillModule;

namespace Shared.Dtos
{
	public class CourseDto : BaseDto
	{
		public string? CourseUrl { get; set; } // optional
		public int DurationInHours { get; set; }
		public string? DifficultyLevel { get; set; }
		public ICollection<SkillDto>? Skills { get; set; }
	}
}
