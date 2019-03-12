﻿//-----------------------------------------------------------------------------------------------------
// <copyright file="FakeItEasyDbSetExtensions.cs" company="Scott Xu">
// Copyright (c) Scott Xu. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------------------------------

using FakeItEasy;

namespace EntityFrameworkCore.Testing.FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FakeItEasy;

    /// <summary>
    /// Extension methods for <see cref="DbSet{T}"/>.
    /// </summary>
    public static class FakeItEasyDbSetExtensions
    {
        /// <summary>
        /// Setup data to <see cref="DbSet{T}"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="dbSet">The <see cref="DbSet{T}"/>.</param>
        /// <param name="data">The seed data.</param>
        /// <param name="find">The find action.</param>
        /// <returns>The updated <see cref="DbSet{T}"/>.</returns>
        public static DbSet<TEntity> SetupData<TEntity>(this DbSet<TEntity> dbSet, ICollection<TEntity> data = null, Func<object[], TEntity> find = null)
            where TEntity : class
        {
            data = data ?? new List<TEntity>();
            find = find ?? (o => null);

            var query = new InMemoryAsyncQueryable<TEntity>(data.AsQueryable());

            A.CallTo(() => ((IQueryable<TEntity>)dbSet).Provider).ReturnsLazily(info => query.Provider);
            A.CallTo(() => ((IQueryable<TEntity>)dbSet).Expression).ReturnsLazily(info => query.Expression);
            A.CallTo(() => ((IQueryable<TEntity>)dbSet).ElementType).ReturnsLazily(info => query.ElementType);
            A.CallTo(() => ((IQueryable<TEntity>)dbSet).GetEnumerator()).ReturnsLazily(info => query.GetEnumerator());

            A.CallTo(() => ((IDbAsyncEnumerable<TEntity>)dbSet).GetAsyncEnumerator()).ReturnsLazily(info => query.GetAsyncEnumerator());

            A.CallTo(() => dbSet.AsNoTracking()).Returns(dbSet);
            A.CallTo(() => dbSet.Include(A<string>._)).Returns(dbSet);
            A.CallTo(() => dbSet.Find(A<object[]>._)).ReturnsLazily<TEntity, object[]>(objs => find(objs));

            A.CallTo(() => dbSet.FindAsync(A<object[]>._)).ReturnsLazily<Task<TEntity>, object[]>(objs => Task.Run(() => find(objs)));
            A.CallTo(() => dbSet.FindAsync(A<CancellationToken>._, A<object[]>._)).ReturnsLazily<Task<TEntity>, CancellationToken, object[]>((token, objs) => Task.Run(() => find(objs), token));

            A.CallTo(() => dbSet.Create()).ReturnsLazily(() => Activator.CreateInstance<TEntity>());

            A.CallTo(() => dbSet.Remove(A<TEntity>._)).ReturnsLazily<TEntity, TEntity>(entity =>
            {
                data.Remove(entity);
                return entity;
            });

            A.CallTo(() => dbSet.RemoveRange(A<IEnumerable<TEntity>>._)).ReturnsLazily<IEnumerable<TEntity>, IEnumerable<TEntity>>(entities =>
            {
                foreach (var entity in entities)
                {
                    data.Remove(entity);
                }

                return entities;
            });

            A.CallTo(() => dbSet.Add(A<TEntity>._)).ReturnsLazily<TEntity, TEntity>(entity =>
            {
                data.Add(entity);
                return entity;
            });

            A.CallTo(() => dbSet.Attach(A<TEntity>._)).ReturnsLazily<TEntity, TEntity>(entity =>
            {
                data.Add(entity);
                return entity;
            });

            A.CallTo(() => dbSet.AddRange(A<IEnumerable<TEntity>>._)).ReturnsLazily<IEnumerable<TEntity>, IEnumerable<TEntity>>(entities =>
            {
                foreach (var entity in entities)
                {
                    data.Add(entity);
                }

                return entities;
            });

            return dbSet;
        }
    }
}