namespace CRM_Perf_BenchMark
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Microsoft.VisualStudio.TestTools.WebTesting;
	using System.Web.Script.Serialization;

	public class JsonHttpBodyWrapper // We can't inherit from StringHttpBody because test harness fails to complete long running requests because of it.
	{
		//public StringHttpBody Body = new StringHttpBody();

		//public JsonHttpBodyWrapper(object jsonObject):
		//	this(new JavaScriptSerializer().Serialize(jsonObject))
		//{
		//}

		//public JsonHttpBodyWrapper(string jsonString) :
		//	base()
		//{
		//	this.Body.ContentType = "application/json; charset=UTF-8";
		//	this.Body.BodyString = jsonString;
		//}
	}
}
