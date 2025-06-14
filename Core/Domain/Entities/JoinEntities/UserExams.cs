﻿using Domain.Entities.IdentityModule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.JoinEntities
{
	public class UserExam : UserBaseEntity
	{
		public int AttemptCount { get; set; } = 1;
		public DateTimeOffset? LastAttemptDate { get; set; } = null;
		public DateTimeOffset? FinishedAt { get; set; } = null;
		public decimal Score { get; set; }
		public bool IsPassed { get; set; } = false;
		#region Relations
		public int ExamId { get; set; }
		public Exam Exams { get; set; } = default!;
		#endregion
	}
}
