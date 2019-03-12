//-----------------------------------------------------------------------------------------------------
// <copyright file="NSubstituteDbSetActivationStrategy.cs" company="Scott Xu">
// Copyright (c) Scott Xu. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------------------------------

namespace EntityFrameworkCore.Testing.NSubstitute.Ninject
{
    using System.Data.Entity;
    using EntityFrameworkCore.Testing.Ninject;
    using global::Ninject.Activation;

    /// <summary>
    /// <see cref="DbSet{T}"/> property injection strategy.
    /// </summary>
    public class NSubstituteDbSetActivationStrategy : DbSetActivationStrategy
    {
        /// <summary>
        /// Seed data for the <see cref="DbSet{T}"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="reference">The reference to the <see cref="DbSet{T}"/>.</param>
        protected override void ActivateDbSet(IContext context, InstanceReference reference)
        {
            dynamic substitute = reference.Instance;
            NSubstituteDbSetExtensions.SetupData(substitute);
        }
    }
}