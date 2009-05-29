﻿//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Logging Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Common.TestSupport.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Logging.Filters;
using Microsoft.Practices.EnterpriseLibrary.Logging.Instrumentation;
using Microsoft.Practices.EnterpriseLibrary.Logging.Tests.TraceListeners;
using Microsoft.Practices.EnterpriseLibrary.Logging.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Tests
{
    /// <summary>
    /// Summary description for LogWriterFixture
    /// </summary>
    [TestClass]
    public class LogWriterFixture
    {
        const string TotalLoggingEventsRaised = "Total Logging Events Raised";
        const string TotalTraceListenerEntriesWritten = "Total Trace Listener Entries Written";
        const string counterCategoryName = "Enterprise Library Logging Counters";

        const string applicationInstanceName = "applicationInstanceName";
        const string instanceName = "Foo";
        string formattedInstanceName;

        EnterpriseLibraryPerformanceCounter totalLoggingEventsRaised;
        EnterpriseLibraryPerformanceCounter totalTraceListenerEntriesWritten;
        EnterpriseLibraryPerformanceCounter totalTraceOperationsStartedCounter;
        AppDomainNameFormatter nameFormatter;
        ILoggingInstrumentationProvider instrumentationProvider;
        ITracerInstrumentationProvider tracerInstrumentationProvider;

        [TestInitialize]
        public void SetUp()
        {
            nameFormatter = new AppDomainNameFormatter(applicationInstanceName);
            instrumentationProvider = new LoggingInstrumentationProvider(instanceName, true, true, true, applicationInstanceName);
            tracerInstrumentationProvider = new TracerInstrumentationProvider(true, false, false, string.Empty);
            formattedInstanceName = nameFormatter.CreateName(instanceName);
            totalLoggingEventsRaised = new EnterpriseLibraryPerformanceCounter(counterCategoryName, TotalLoggingEventsRaised, formattedInstanceName);
            totalTraceListenerEntriesWritten = new EnterpriseLibraryPerformanceCounter(counterCategoryName, TotalTraceListenerEntriesWritten, formattedInstanceName);

            // Use AppDomainFriendlyName for the instance name
            nameFormatter = new AppDomainNameFormatter();
            formattedInstanceName = nameFormatter.CreateName(instanceName);

            totalTraceOperationsStartedCounter = new EnterpriseLibraryPerformanceCounter(TracerInstrumentationProvider.counterCategoryName, TracerInstrumentationProvider.TotalTraceOperationsStartedCounterName, formattedInstanceName);
        }

        [TestMethod]
        public void TotalTraceOperationsStartedCounterIncremented()
        {
            tracerInstrumentationProvider.FireTraceOperationStarted("foo");

            long expected = 1;
            Assert.AreEqual(expected, totalTraceOperationsStartedCounter.Value);
        }

        [TestMethod]
        public void TotalLoggingEventsRaisedCounterIncremented()
        {
            instrumentationProvider.FireLogEventRaised();

            long expected = 1;
            Assert.AreEqual(expected, totalLoggingEventsRaised.Value);
        }

        [TestMethod]
        public void TotalTraceListenerEntriesWrittenIncremented()
        {
            instrumentationProvider.FireTraceListenerEntryWrittenEvent();

            long expected = 1;
            Assert.AreEqual(expected, totalTraceListenerEntriesWritten.Value);
        }

        [TestMethod]
        public void CanCreateLogWriterUsingConstructor()
        {
            LogWriter writer = new LogWriter(new List<ILogFilter>(), new Dictionary<string, LogSource>(), new LogSource("errors"), "default");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreationOfLogWriterUsingConstructorWithNullFiltersThrows()
        {
            LogWriter writer = new LogWriter(null, new Dictionary<string, LogSource>(), new LogSource("errors"), "default");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreationOfLogWriterUsingConstructorWithNullTraceSourcesThrows()
        {
            LogWriter writer = new LogWriter(new List<ILogFilter>(), (IDictionary<string, LogSource>)null, new LogSource("errors"), "default");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreationOfLogWriterUsingConstructorWithNullErrorsTraceSourceThrows()
        {
            LogWriter writer = new LogWriter(new List<ILogFilter>(), new Dictionary<string, LogSource>(), null, "default");
        }

        [TestMethod]
        public void CanCreateLogWriterUsingContainer()
        {
            LogWriter writer = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();
            Assert.IsNotNull(writer);
        }

        [TestMethod]
        public void CanGetLogFiltersByType()
        {
            ICollection<ILogFilter> filters = new List<ILogFilter>();

            ICollection<string> categories = new List<string>();
            categories.Add("cat1");
            categories.Add("cat2");
            categories.Add("cat3");
            categories.Add("cat4");
            filters.Add(new CategoryFilter("category", categories, CategoryFilterMode.AllowAllExceptDenied));
            filters.Add(new PriorityFilter("priority", 100));
            filters.Add(new LogEnabledFilter("enable", true));

            LogWriter writer = new LogWriter(filters, new Dictionary<string, LogSource>(), new LogSource("errors"), "default");
            CategoryFilter categoryFilter = writer.GetFilter<CategoryFilter>();
            PriorityFilter priorityFilter = writer.GetFilter<PriorityFilter>();
            LogEnabledFilter enabledFilter = writer.GetFilter<LogEnabledFilter>();

            Assert.IsNotNull(categoryFilter);
            Assert.AreEqual(4, categoryFilter.CategoryFilters.Count);
            Assert.IsNotNull(priorityFilter);
            Assert.AreEqual(100, priorityFilter.MinimumPriority);
            Assert.IsNotNull(enabledFilter);
            Assert.IsTrue(enabledFilter.Enabled);
        }

        [TestMethod]
        public void CanGetLogFiltersByNameAndType()
        {
            ICollection<ILogFilter> filters = new List<ILogFilter>();

            ICollection<string> categories = new List<string>();
            categories.Add("cat1");
            categories.Add("cat2");
            categories.Add("cat3");
            categories.Add("cat4");
            filters.Add(new CategoryFilter("category", categories, CategoryFilterMode.AllowAllExceptDenied));
            filters.Add(new PriorityFilter("priority1", 100));
            filters.Add(new LogEnabledFilter("enable", true));
            filters.Add(new PriorityFilter("priority2", 200));

            LogWriter writer = new LogWriter(filters, new Dictionary<string, LogSource>(), new LogSource("errors"), "default");
            PriorityFilter priorityFilter1 = writer.GetFilter<PriorityFilter>("priority1");
            PriorityFilter priorityFilter2 = writer.GetFilter<PriorityFilter>("priority2");

            Assert.IsNotNull(priorityFilter1);
            Assert.AreEqual(100, priorityFilter1.MinimumPriority);
            Assert.IsNotNull(priorityFilter2);
            Assert.AreEqual(200, priorityFilter2.MinimumPriority);
        }

        [TestMethod]
        public void VerifyTraceListenerPerfCounter()
        {
            Logger.Reset();

            string counterName = "Trace Listener Entries Written/sec";
            int initialCount = GetCounterValue(counterName);
            LogEntry entry = new LogEntry();
            entry.Severity = TraceEventType.Error;
            entry.Message = "";
            entry.Categories.Add("FormattedCategory");
            Logger.Write(entry);
            int loggedCount = GetCounterValue(counterName);
            Assert.AreEqual(1, loggedCount - initialCount);
            initialCount = loggedCount;
            Logger.Write(entry);
            loggedCount = GetCounterValue(counterName);
            Assert.AreEqual(1, loggedCount - initialCount);
        }

        int GetCounterValue(string counterName)
        {
            string categoryName = "Enterprise Library Logging Counters";
            string instanceName = new AppDomainNameFormatter().CreateName("Total");
            if (PerformanceCounterCategory.InstanceExists(instanceName, categoryName))
            {
                using (PerformanceCounter counter = new PerformanceCounter())
                {
                    counter.CategoryName = categoryName;
                    counter.CounterName = counterName;
                    counter.InstanceName = instanceName;
                    return (int)counter.RawValue;
                }
            }
            return 0;
        }

        [TestMethod]
        public void WmiFiredWhenDeliveryToErrorSourceFails()
        {
            TraceListener badTraceListener = new BadTraceListener(new Exception("test exception"));
            LogSource badSource = new LogSource("badSource");
            badSource.Listeners.Add(badTraceListener);

            Dictionary<string, LogSource> logSources = new Dictionary<string, LogSource>();
            logSources.Add("foo", badSource);

            LogWriter writer = new LogWriter(new List<ILogFilter>(), logSources, badSource, "foo", new LoggingInstrumentationProvider(false, false, true, "applicationInstanceName"));

            using (WmiEventWatcher eventListener = new WmiEventWatcher(1))
            {
                writer.Write(CommonUtil.GetDefaultLogEntry());

                eventListener.WaitForEvents();

                Assert.AreEqual(1, eventListener.EventsReceived.Count);
                Assert.AreEqual("LoggingFailureLoggingErrorEvent", eventListener.EventsReceived[0].ClassPath.ClassName);
                string exceptionMessage = (string)eventListener.EventsReceived[0].GetPropertyValue("ExceptionMessage");
                Assert.IsTrue(-1 != exceptionMessage.IndexOf("test exception"));
                Assert.IsNotNull(eventListener.EventsReceived[0].GetPropertyValue("ErrorMessage"));
            }
        }

        [TestMethod]
        public void EventLogWrittenWhenDeliveryToErrorSourceFails()
        {
            TraceListener badTraceListener = new BadTraceListener(new Exception("test exception"));
            LogSource badSource = new LogSource("badSource");
            badSource.Listeners.Add(badTraceListener);

            Dictionary<string, LogSource> logSources = new Dictionary<string, LogSource>();
            logSources.Add("foo", badSource);

            ILoggingInstrumentationProvider instrumentationProvider =  new LoggingInstrumentationProvider(false, true, false, "applicationInstanceName");
            LogWriter writer = new LogWriter(new List<ILogFilter>(), logSources, badSource, "foo", instrumentationProvider);

            writer.Write(CommonUtil.GetDefaultLogEntry());

            string lastEventLogEntry = CommonUtil.GetLastEventLogEntry();
            Assert.IsTrue(-1 != lastEventLogEntry.IndexOf("test exception"));
        }

        [TestMethod]
        public void CanGetLogFiltersByName()
        {
            ICollection<ILogFilter> filters = new List<ILogFilter>();

            ICollection<string> categories = new List<string>();
            categories.Add("cat1");
            categories.Add("cat2");
            categories.Add("cat3");
            categories.Add("cat4");
            filters.Add(new CategoryFilter("category", categories, CategoryFilterMode.AllowAllExceptDenied));
            filters.Add(new PriorityFilter("priority1", 100));
            filters.Add(new LogEnabledFilter("enable", true));
            filters.Add(new PriorityFilter("priority2", 200));

            LogWriter writer = new LogWriter(filters, new Dictionary<string, LogSource>(), new LogSource("errors"), "default");
            ILogFilter categoryFilter = writer.GetFilter("category");
            ILogFilter priorityFilter = writer.GetFilter("priority2");

            Assert.IsNotNull(categoryFilter);
            Assert.AreEqual(typeof(CategoryFilter), categoryFilter.GetType());
            Assert.IsNotNull(priorityFilter);
            Assert.AreEqual(typeof(PriorityFilter), priorityFilter.GetType());
        }
    }
}
