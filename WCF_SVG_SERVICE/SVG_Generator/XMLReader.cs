using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace LibrarySVG
{


    public class Bedrijf
    {
        public List<Organisatie> organisaties = new List<Organisatie>();
        public List<Object> secondTierList = new List<Object>();
    }

    public class Organisatie
    {
        public String name { get; set; }
        public List<Afdeling> afdelingen = new List<Afdeling>();
        public List<Object> secondTierList = new List<Object>();


    }

    public class Afdeling
    {
        public String name { get; set; }
        public List<Project> projecten = new List<Project>();
        public List<Object> secondTierList = new List<Object>();
    }

    public class Project
    {
        public String name { get; set; }
        public int beginDate { get; set; }
        public int endDate { get; set; }



    }


    public class Level
    {
        public string name { get; set; }
        public string elementName { get; set; }
        public int elementID { get; set; }
        public int startDatum { get; set; }
        public int eindDatum { get; set; }
        public List<Level> subLevels { get; set; }

        //--Unknown attributes komen hierin

        public Level()
        {

        }



        public void AddChild(Level sub)
        {
            if (!HasChild(sub))
            {
                this.subLevels.Add(sub);
            }
        }
        public Boolean SubLevelIsNull()
        {
            bool b = true;
            if (subLevels != null)
            {
                b = false;
            }
            return b;
        }
        public Boolean SubLevelIsEmpty()
        {
            bool b = true;
            if (!SubLevelIsNull())
            {
                if (subLevels.Count > 0)
                {
                    b = false;
                }
            }
            return b;
        }
        public Boolean HasChild(Object o)
        {
            bool b = false;
            if (!SubLevelIsEmpty())
            {
                if (subLevels.Contains(o))
                {
                    b = true;
                }
            }
            return b;
        }

        /*public void AddExpando(string attribuut, string attribuutinhoud)
        {
            (IDictionary)expando = new ExpandoObject() as IDictionary<string, Object>;
            expando.Add
        }
        */



    }




    public class XMLReader
    {

        private XDocument XDoc;

        public bool hasChildren(XDocument XDoc2)
        {


            var queryResultList = (from data in XDoc2.Descendants("afdeling")
                                   where data.Parent.Attribute("name").Value.Equals("bedrijf1")
                                   //&& (String)data.Parent.Element("name").ElementValueNull() == name
                                   select (new Level
                                   {
                                       name = data.Element("project").Value
                                       //Name = data.Element("name").ElementValueNull(),
                                       //elementID = data.AttributeValueNull("id"),
                                       //startDate = data.Element("startDate").ElementValueNull(),
                                       //endDate = data.Element("startDate").ElementValueNull()
                                   }));

            if (queryResultList.Any())
            {
                return true;
            }
            else {
                return false;
            }
        }


        public List<Level> readXMLOrganisatie(XDocument XDoc)
        {

            this.XDoc = XDoc;
            List<Level> org = new List<Level>();



            if (XDoc.Descendants("organisatie").Any())
            {

                var bedrijf = XDoc.Elements("bedrijf").Select(b => new Level
                {


                    subLevels = b.Elements("organisatie").Select(o => new Level
                    {
                        name = (String)o.Attribute("name"),

                        subLevels = o.Elements("afdeling").Select(a => new Level
                        {

                            name = (String)a.Attribute("name"),

                            subLevels = a.Elements("project").Select(p => new Level
                            {
                                name = (String)p.Attribute("name"),
                                startDatum = (int)p.Attribute("beginDatum"),
                                eindDatum = (int)p.Attribute("eindDatum")

                            }).ToList()



                        }).ToList()



                    }).ToList()


                });


                foreach (Level l in bedrijf)
                {
                    org = l.subLevels;
                }

            }
            else
            {

                readXMLAfdeling();

            }
            return org;
        }

        public void readXMLAfdeling()
        {





            if (XDoc.Descendants("Afdeling").Any())
            {

            }
            else
            {
                readXMLProject();
            }

        }

        public void readXMLProject()
        {






        }


    }
}
