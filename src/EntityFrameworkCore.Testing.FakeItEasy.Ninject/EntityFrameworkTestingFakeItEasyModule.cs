//-----------------------------------------------------------------------------------------------------
// <copyright file="EntityFrameworkTestingFakeItEasyModule.cs" company="Scott Xu">
// Copyright (c) Scott Xu. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------------------------------

using EntityFrameworkCore.Testing.Ninject;

namespace EntityFrameworkCore.Testing.FakeItEasy.Ninject
{
    using global::Ninject.Activation.Strategies;

    /// <summary>
    /// EntityFramework Testing Module
    /// </summary>
    public class EntityFrameworkTestingFakeItEasyModule : EntityFrameworkTestingModule
    {
        /// <summary>
        /// Load the components.
        /// </summary>
        public override void Load()
        {
            this.Kernel.Components.Add<IActivationStrategy, FakeItEasyDbSetActivationStrategy>();

            base.Load();
        }
    }
}