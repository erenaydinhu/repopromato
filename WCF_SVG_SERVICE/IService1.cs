using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCF_SVG_SERVICE
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "getString")]
        string teststring();


        [OperationContract]
        [WebInvoke(Method ="POST",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle =WebMessageBodyStyle.Bare,
           UriTemplate = "GetData")]
        string getSVG(String organisatie);

        [OperationContract(Name = "PostSampleMethod")]
        [WebInvoke(Method = "POST", 
     UriTemplate = "PostSampleMethod/New")]
        string PostSampleMethod(Stream data);


    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
