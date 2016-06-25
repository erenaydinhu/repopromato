using LibrarySVG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Linq;

namespace WCF_SVG_SERVICE
{
 
    public class Service1 : IService1
    {

        XMLReader xreader = new XMLReader();
        SVGCreator svgcreate = new SVGCreator();
        XDocument finalDocument = new XDocument();
        XElement finalElement = new XElement("final");
        public string getSVG(String organisatie)
        {

            //  XDocument x = XDocument.Parse(organisatie);

            //XMLReader reader = new XMLReader();
            //List<Level> levels = reader.readXMLOrganisatie(x);
            //SVGCreator svgCreate = new SVGCreator();
            //     File.WriteAllText(@"C:\Users\Eren_2\Desktop\message.txt", organisatie);

            return organisatie;

        }
        public string teststring()
        {

            return "it works";
        }


        public string PostSampleMethod(Stream data)
        {
            StreamReader reader = new StreamReader(data);
            string xmlString = reader.ReadToEnd();
            string returnValue = xmlString;

            XDocument x = new XDocument();
            x = XDocument.Parse(returnValue);
        

            List<Level> levels = xreader.readXMLOrganisatie(x);

            leveling(levels[0].subLevels);
            XElement final = svgcreate.createSVGOrganisatie(levels[0].subLevels);




         //   File.WriteAllText(@"C:\Users\Eren_2\Desktop\message.txt", returnValue);
         //   File.WriteAllText(@"C:\Users\Eren_2\Desktop\message2.html", final.ToString());


            // return the XMLString data

          
         //   finalElement.Add(svgcreate.createSVGOrganisatie(levels[0].subLevels[0].subLevels));

            finalDocument.Add(finalElement);

           
         //   finalDocument.Save(@"C:\Users\Eren_2\Desktop\testxml.txt");

            return final.ToString();

        }



        public void leveling(List<Level> levelList)
        {
            svgcreate = new SVGCreator();



            finalElement.Add(svgcreate.createSVGOrganisatie(levelList));

            foreach (Level l in levelList)
            {

              
                if ((l.subLevels != null) && (l.subLevels.Any()))
                {
                    
                    
                


                    
                    leveling(l.subLevels);



                }
                else
                {

                    List<Level> final = new List<Level>();
                    final.Add(l);
                    svgcreate = new SVGCreator();
                    finalElement.Add(svgcreate.createSVGOrganisatie(final));
                }
            }

        }














        public string GetSampleMethod(string strUserName)
        {
            StringBuilder strReturnValue = new StringBuilder();
            // return username prefixed as shown below
            strReturnValue.Append(string.Format("You have entered userName as {0}", strUserName));
            return strReturnValue.ToString();
        }
        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

   
    }
}
