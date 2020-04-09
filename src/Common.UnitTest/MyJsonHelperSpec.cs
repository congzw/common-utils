using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Common
{
    [TestClass]
    public class MyJsonHelperSpec
    {
        [TestMethod]
        public void DeserializeObject_String_ShouldOk()
        {
            var myJsonHelper = new MyJsonHelper();
            var serializeObject = myJsonHelper.SerializeObject("ABC", true);
            serializeObject.ShouldEqual("ABC");

            myJsonHelper.DeserializeObject("ABC", "ddd").ShouldEqual("ABC");
            myJsonHelper.DeserializeObject(null, "ddd").ShouldEqual("ddd");
            myJsonHelper.DeserializeObject("", "ddd").ShouldEqual("ddd");
            myJsonHelper.DeserializeObject(" ", "ddd").ShouldEqual("ddd");
        }

        [TestMethod]
        public void DeserializeObject_String_Ext_ShouldOk()
        {
            var myJsonHelper = new MyJsonHelper();

            myJsonHelper.DeserializeObject<string>("ABC", "ddd").ShouldEqual("ABC");
            myJsonHelper.DeserializeObject<string>(null, "ddd").ShouldEqual("ddd");
            myJsonHelper.DeserializeObject<string>("", "ddd").ShouldEqual("ddd");
            myJsonHelper.DeserializeObject<string>(" ", "ddd").ShouldEqual("ddd");
        }

        [TestMethod]
        public void ParseXElementAsDynamic_Single_Element_ShouldOk()
        {
            var myJsonHelper = new MyJsonHelper();
            var loadXml = @"<Content DeviceIP=""192.168.1.10"">
                <Task BurnStatus=""0"">
                </Task>
            </Content>";

            var xDoc = XDocument.Parse(loadXml);
            var root = xDoc.Elements().First();
            var theObject = myJsonHelper.ParseXElementAsDynamic(root);

            JsonConvert.SerializeObject((object)theObject, Formatting.Indented).Log();

            var theContent = theObject.Content;
            string deviceIP = theContent.DeviceIP;
            deviceIP.Log();
            deviceIP.ShouldEqual("192.168.1.10");

            var theTask = theContent.Task;
            string burnStatus = theTask.BurnStatus;
            burnStatus.Log();
        }
        
        [TestMethod]
        public void ParseXElementAsDynamic_Array_Elements_ShouldOk()
        {
            var myJsonHelper = new MyJsonHelper();
            var loadXml = @"<Content DeviceIP=""192.168.1.10"">
                <Task BurnStatus=""0"">
                </Task>
                <Task BurnStatus=""1"">
                </Task>
            </Content>";


            var xDoc = XDocument.Parse(loadXml);
            var root = xDoc.Elements().First();
            var theObject = myJsonHelper.ParseXElementAsDynamic(root);

            JsonConvert.SerializeObject((object)theObject, Formatting.Indented).Log();

            var theContent = theObject.Content;
            string deviceIP = theContent.DeviceIP;
            deviceIP.Log();
            deviceIP.ShouldEqual("192.168.1.10");
            
            var theTask = theContent.Task;
            string burnStatus = theTask[0].BurnStatus;
            burnStatus.Log();
            string burnStatus2 = theTask[1].BurnStatus;
            burnStatus2.Log();
        }
    }
}
