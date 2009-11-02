﻿//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
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
using System.Text;
using System.ComponentModel;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Console.Wpf.ViewModel.Services;
using Microsoft.Practices.Unity;

namespace Console.Wpf.ViewModel.BlockSpecifics
{
    public class TraceListenerElementCollectionViewModel : ElementCollectionViewModel, IElementExtendedPropertyProvider
    {
        
        public TraceListenerElementCollectionViewModel(ElementViewModel parentElementModel, PropertyDescriptor declaringProperty)
            : base(parentElementModel, declaringProperty)
        {
        }

        public bool CanExtend(ElementViewModel subject)
        {
            return typeof(TraceSourceData).IsAssignableFrom(subject.ConfigurationType);
        }


        public override Type[] PolymorphicCollectionElementTypes
        {
            get
            {
                return base.PolymorphicCollectionElementTypes.Union(
                            new Type[]{
                                typeof(SystemDiagnosticsTraceListenerData), 
                                typeof(CustomTraceListenerData)
                            }).ToArray();
            }
        }

        public IEnumerable<Property> GetExtendedProperties(ElementViewModel subject)
        {
            yield return ContainingSection.CreateProperty<NewTraceListenerProperty>( 
                        new ParameterOverride("component", subject),
                        new ParameterOverride("listenerCollection", this), 
                        new ParameterOverride("traceSourceViewModel", subject));
        }

        private class NewTraceListenerProperty : Property
        {
            private readonly TraceListenerElementCollectionViewModel listenerCollection;
            private readonly ElementViewModel traceSourceViewModel;
            private readonly string selectEntryText = "[Select Listener]";        // todo: get from resource

            public NewTraceListenerProperty(IServiceProvider serviceProvider, object component, TraceListenerElementCollectionViewModel listenerCollection, ElementViewModel traceSourceViewModel)
                : base(serviceProvider, component, null)
            {
                this.listenerCollection = listenerCollection;
                this.traceSourceViewModel = traceSourceViewModel;
            }

            public override string Category
            {
                get
                {
                    return "Trace Listeners";   //todo: get from resource
                }
            }

            public override string PropertyName
            {
                get
                {
                    return "NewTraceListener";
                }
            }

            public override string DisplayName
            {
                get
                {
                    return "Connect to Listener";
                }
            }

            public override bool HasSuggestedValues
            {
                get
                {
                    return true;
                }
            }

            public override IEnumerable<object> SuggestedValues
            {
                get
                {
                    return new[] { selectEntryText }
                        .Concat(listenerCollection.ChildElements
                                .Where(e => typeof(TraceListenerData).IsAssignableFrom(e.ConfigurationType))
                                .Select(e => e.Name)
                                .Except(
                                    traceSourceViewModel.DescendentElements()
                                        .Where(r => typeof(TraceListenerReferenceData).IsAssignableFrom(r.ConfigurationType))
                                        .Select(r => r.Name))
                                        )
                        .ToArray();
                }
            }

            public override object Value
            {
                get
                {
                    return selectEntryText;
                }
                set
                {
                    if (value != null && !string.Equals(value.ToString(), selectEntryText, StringComparison.OrdinalIgnoreCase))
                    {
                        var referenceCollection =
                            traceSourceViewModel.ChildElements.OfType<ElementCollectionViewModel>().Where(
                                e =>
                                typeof(NamedElementCollection<TraceListenerReferenceData>).IsAssignableFrom(
                                    e.ConfigurationType)).First();
                        var newElement = referenceCollection.AddNewCollectionElement(typeof(TraceListenerReferenceData));
                        newElement.Property("Name").Value = value;
                    }
                }
            }
        }
    }
}