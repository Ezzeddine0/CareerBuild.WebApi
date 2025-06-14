﻿using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
	public interface IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
	{
		Task AddAsync(TEntity entity);
		void Update(TEntity entity);
		void Remove(TEntity entity);
		Task<TEntity?> GetByIdAsync(TKey id);
		Task<IEnumerable<TEntity>> GetAllAsync();

		#region Specification
		Task<TEntity?> GetByIdAsync(ISpecification<TEntity, TKey> specification);
		Task<IEnumerable<TEntity>> GetAllAsync(ISpecification<TEntity, TKey> specification);
		#endregion
	}

}
