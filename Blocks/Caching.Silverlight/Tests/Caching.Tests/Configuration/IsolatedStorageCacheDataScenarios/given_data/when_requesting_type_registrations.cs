﻿//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Caching Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Caching.Runtime.Caching;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Caching.Tests.Configuration.IsolatedStorageCacheDataScenarios.given_data
{
    [TestClass]
    public class when_requesting_type_registrations : Context
    {
        private IEnumerable<TypeRegistration> registrations;

        protected override void Act()
        {
            base.Act();

            this.registrations = this.data.GetRegistrations(null);
        }

        [TestMethod]
        public void then_has_single_registration()
        {
            Assert.AreEqual(1, this.registrations.Count());
        }

        [TestMethod]
        public void then_registration_has_name()
        {
            Assert.AreEqual("test name", this.registrations.First().Name);
            Assert.IsTrue(this.registrations.First().IsPublicName);
        }

        [TestMethod]
        public void then_registration_is_not_default()
        {
            Assert.IsFalse(this.registrations.First().IsDefault);
        }

        [TestMethod]
        public void then_registration_is_singleton()
        {
            Assert.AreEqual(TypeRegistrationLifetime.Singleton, this.registrations.First().Lifetime);
        }

        [TestMethod]
        public void then_registration_is_for_ObjectCache_service()
        {
            Assert.AreSame(typeof(ObjectCache), this.registrations.First().ServiceType);
        }

        [TestMethod]
        public void then_registration_is_for_IsolatedStorageCache_implementation()
        {
            Assert.AreSame(typeof(IsolatedStorageCache), this.registrations.First().ImplementationType);
        }

        [TestMethod]
        public void then_registration_has_expected_parameters()
        {
            Assert.AreEqual(6, this.registrations.First().ConstructorParameters.Count());
            Assert.AreEqual("test name", ((ConstantParameterValue)this.registrations.First().ConstructorParameters.ElementAt(0)).Value);
            Assert.AreEqual(100, ((ConstantParameterValue)this.registrations.First().ConstructorParameters.ElementAt(1)).Value);
            Assert.AreEqual(75, ((ConstantParameterValue)this.registrations.First().ConstructorParameters.ElementAt(2)).Value);
            Assert.AreEqual(65, ((ConstantParameterValue)this.registrations.First().ConstructorParameters.ElementAt(3)).Value);
            Assert.AreEqual(TimeSpan.FromSeconds(45), ((ConstantParameterValue)this.registrations.First().ConstructorParameters.ElementAt(4)).Value);
            Assert.IsInstanceOfType(((ConstantParameterValue)this.registrations.First().ConstructorParameters.ElementAt(5)).Value, typeof(MockSerializer));
        }

        [TestMethod]
        public void then_registration_has_no_properties()
        {
            Assert.AreEqual(0, this.registrations.First().InjectedProperties.Count());
        }
    }
}