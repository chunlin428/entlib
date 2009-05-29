﻿//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Exception Handling Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;
using System.Collections.Generic;

namespace Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.WCF.Configuration
{
	/// <summary>
	/// Configuration data for the <see cref="FaultContractExceptionHandler"/> class.
	/// </summary>
	public class FaultContractExceptionHandlerData : ExceptionHandlerData
	{
		/// <summary>
		/// The attribute name for an alternative exception message for the specified FaultContract.
		/// </summary>
		public const string ExceptionMessageAttributeName = "exceptionMessage";

		/// <summary>
		/// The attribute name for the faultContract type.
		/// </summary>
		public const string FaultContractTypeAttributeName = "faultContractType";

		const string PropertyMappingsPropertyName = "mappings";

		/// <summary>
		/// Initializes a new instance of the <see cref="FaultContractExceptionHandlerData"/> class.
		/// </summary>
		public FaultContractExceptionHandlerData() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="FaultContractExceptionHandlerData"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public FaultContractExceptionHandlerData(string name)
			: this(name, string.Empty) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="FaultContractExceptionHandlerData"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="faultContractType">Type of the fault contract.</param>
		public FaultContractExceptionHandlerData(string name,
												 string faultContractType)
			: base(name, typeof(FaultContractExceptionHandler))
		{
			FaultContractType = faultContractType;
		}

		/// <summary>
		/// Get the attributes for the provider.
		/// </summary>
		/// <value>
		/// The attributes for the provider.
		/// </value>
		public NameValueCollection Attributes
		{
			get
			{
				NameValueCollection result = new NameValueCollection();

				foreach (FaultContractExceptionHandlerMappingData mapping in PropertyMappings)
				{
					result.Add(mapping.Name, mapping.Source);
				}

				return result;
			}
		}

		/// <summary>
		/// Gets or sets the exception message.
		/// </summary>
		/// <value>The exception message.</value>
		[ConfigurationProperty(ExceptionMessageAttributeName, IsRequired = false)]
		public string ExceptionMessage
		{
			get { return this[ExceptionMessageAttributeName] as string; }
			set { this[ExceptionMessageAttributeName] = value; }
		}

		/// <summary>
		/// Gets or sets the type of the fault contract.
		/// </summary>
		/// <value>The type of the fault contract.</value>
		[ConfigurationProperty(FaultContractTypeAttributeName, IsRequired = true)]
		public string FaultContractType
		{
			get { return this[FaultContractTypeAttributeName] as string; }
			set { this[FaultContractTypeAttributeName] = value; }
		}

		/// <summary>
		/// Gets the collection of <see cref="FaultContractExceptionHandlerMappingData"/> that represent the mappings from
		/// source properties on the exception to properties in the fault contract.
		/// </summary>
		[ConfigurationProperty(PropertyMappingsPropertyName)]
		public NamedElementCollection<FaultContractExceptionHandlerMappingData> PropertyMappings
		{
			get { return (NamedElementCollection<FaultContractExceptionHandlerMappingData>)this[PropertyMappingsPropertyName]; }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namePrefix"></param>
        /// <returns></returns>
        public override IEnumerable<TypeRegistration> GetRegistrations(string namePrefix)
        {
            yield return new TypeRegistration<IExceptionHandler>(
                () => new FaultContractExceptionHandler(Type.GetType(this.FaultContractType), ExceptionMessage, this.Attributes)
                )
                       {
                           Name = BuildName(namePrefix)
                       };
        }
	}
}
