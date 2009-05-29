﻿//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Validation Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation.Instrumentation;

namespace Microsoft.Practices.EnterpriseLibrary.Validation.TestSupport.TestClasses
{
    public class MockValidationInstrumentationProvider : IValidationInstrumentationProvider
    {
        public void FireValidationSucceeded(Type typeBeingValidated)
        {
            // Intentional no-op
        }

        public void FireValidationFailed(Type typeBeingValidated, ValidationResults validationResult)
        {
            // Intentional no-op
        }

        public void FireConfigurationFailure(ConfigurationErrorsException configurationException)
        {
            // Intentional no-op
        }

        public void FireConfigurationCalled(Type typeBeingValidated)
        {
            // Intentional no-op
        }

        public void FireValidationException(Type typeBeingValidated, string errorMessage, Exception exception)
        {
            // Intentional no-op
        }
    }
}
