using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Microsoft.Crm;

namespace CRM_Perf_BenchMark.SiteWideTesting.UnitTests
{
	/// <summary>
	/// Summary description for StartNotficationThread
	/// </summary>
	[TestClass]
	public class StartNotficationThread
	{
		DBNotificationsProvider dbNotificationProvider;
		NotificationEventType[] notificationEventTypeArray = { NotificationEventType.netInfo,
																	NotificationEventType.netConfigCreate,
																	NotificationEventType.netConfigUpdate,
																	NotificationEventType.netConfigAll,
																	NotificationEventType.netScaleGroupSetState,
																	NotificationEventType.netEncryptionConfigurationChange,
																	NotificationEventType.netOrganizationCreate,
																	NotificationEventType.netOrganizationSetState,
																	NotificationEventType.netOrganizationUpdate,
																	NotificationEventType.netOrganizationDelete,
																	NotificationEventType.netLocator
															   };

		public StartNotficationThread()
		{
			dbNotificationProvider = new DBNotificationsProvider();
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		//[TestInitialize()]
		//public void MyTestInitialize() {}
		//
		// Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup() {}        
		//
		#endregion

		[TestMethod]
		public void GetNotifications()
		{
			DateTime last30seconds = DateTime.Now - TimeSpan.FromSeconds(30);
			NotificationEvent[] notifications = dbNotificationProvider.GetNotifications(last30seconds);
		}

		[TestMethod]
		public void StartNotification()
		{
			TimeSpan timeSpan = new TimeSpan(2, 0, 0);
			NotificationEventHandler handler = new NotificationEventHandler(this.EventOccurred);
			NotificationManager.RegisterEventHandler(notificationEventTypeArray, handler, timeSpan);
			NotificationManager.StartNotificationsThread(dbNotificationProvider);
		}

		public void EventOccurred(NotificationEventArgs a)
		{
			System.Diagnostics.Debug.WriteLine("Event Occurred for org - " + a.OrganizationId.ToString() + " with event data " + a.EventData.ToString());
		}
	}
}
