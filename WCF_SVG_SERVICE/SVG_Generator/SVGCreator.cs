using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LibrarySVG
{
    public class SVGCreator
    {

        int departmentCount;
        XNamespace xlink = "xlink";
        Random rnd = new Random();
        List<XElement> secondaryCirclesLineGlobal = new List<XElement>();
        List<XElement> jaartallenSVG = new List<XElement>();
        List<String> colors = new List<String>();
        List<String> chosenColors = new List<String>();
        double[] radi;
        int[] jaartallen;
        int jaartalverschil;
        int afdelingNummer = 0;
        int colorIndex;
        double degreeRatio;
        double degreeRatioFirst;
        double degreeRatioSecond;
        List<Level> levelList;
        int levelListCount;


        public XElement createSVGOrganisatie(List<Level> levelList)
        {

            int laagsteJaar = 2016;
            int hoogsteJaar = 2016;
            bool sublevelsBestaan = false;

            foreach (Level l in levelList)
            {
                if ((l.subLevels != null) && (l.subLevels.Any()))
                {
                    laagsteJaar = l.subLevels[0].startDatum;
                    hoogsteJaar = l.subLevels[0].eindDatum;
                    sublevelsBestaan = true;
                }
            }

            Console.WriteLine("laagstejaar = " + laagsteJaar);

            Console.WriteLine("hoogsteJaar = " + hoogsteJaar);
            if (sublevelsBestaan == true)
            {
                //fie            
                foreach (Level l in levelList)
                {
                    foreach (Level sl in l.subLevels)
                    {
                        Console.WriteLine("DONG IT");
                        if (sl.eindDatum > hoogsteJaar)
                        {
                            hoogsteJaar = sl.eindDatum;
                        }
                        if (sl.startDatum < laagsteJaar)
                        {
                            laagsteJaar = sl.startDatum;
                        }
                    }
                }

            }
            jaartalverschil = (hoogsteJaar - laagsteJaar) + 2;
            int ji = 0;
            double radiRatio = 110 / ((hoogsteJaar - laagsteJaar) + 2);
            Console.WriteLine("laagstejaar = " + laagsteJaar);

            Console.WriteLine("hoogsteJaar = " + hoogsteJaar);

            jaartallen = new int[(hoogsteJaar - laagsteJaar) + 2];
            radi = new double[(hoogsteJaar - laagsteJaar) + 2];
            for (int i = (laagsteJaar); i < hoogsteJaar + 2; i++)
            {

                jaartallen[ji] = i;
                radi[ji] = (radiRatio * (1 + (i - laagsteJaar)));
                ji++;

            }

            for (int i2 = 0; i2 < jaartallen.Count(); i2++)
            {


                Console.WriteLine(jaartallen[i2]);
                Console.WriteLine(radi[i2]);
            }

            Console.WriteLine("count : " + jaartallen.Count());
            Console.WriteLine("radi : " + radi.Count());
            Array.Reverse(jaartallen);
            foreach (Level l in levelList)
            {
                colors.Add("rgb(" + rnd.Next(255) + ", " + rnd.Next(255) + ", " + rnd.Next(255) + ")");

            }
            int[] aantalprojecten;
            List<int> projectNummers = new List<int>();
            List<XElement> afdelingNamen = new List<XElement>();
            XElement SVG = new XElement("svgmain");
            XElement svg = new XElement("svg");
            this.levelList = levelList;
            levelListCount = levelList.Count();
            departmentCount = levelListCount;
            aantalprojecten = new int[levelList.Count()];


            svg.Add(new XAttribute(XNamespace.Xmlns + "xlink", xlink));
            degreeRatio = (360 / levelListCount);
            // FIXED
            if (levelListCount == 1)
            {
                degreeRatio = 359;
                degreeRatioFirst = 359;
                degreeRatioSecond = 359;
            }
            if (levelListCount != 1)
            {
                degreeRatioFirst = (360 / (levelListCount - 1));
                degreeRatioSecond = (360 / (levelListCount + 1));
            }



            svg.Add(new XAttribute("height", "600"));
            svg.Add(new XAttribute("width", "600"));
            XNamespace nm = XNamespace.Get("hlink");
            svg.Add(nm);
            SVG.Add(svg);

            foreach (Level l in levelList)
            {

                colorIndex = rnd.Next(colors.Count());
                chosenColors.Add(colors[colorIndex]);
                colors.RemoveAt(colorIndex);
                projectNummers.Add(1);

            }

            double dashStart;

            for (int i = (radi.Count() - 1); i != 0; i--)
            {

                dashStart = (2 * Math.PI * radi[i]);
                svg = circleGen(colors, svg, radi[i], radi[i - 1], dashStart, jaartallen[i], i, projectNummers);
                afdelingNummer++;
            }


            foreach (XElement x in afdelingNamen)
            {

                svg.Add(x);

            }

            XElement defs = new XElement("defs");
            XElement marker = new XElement("marker");
            XElement pathDefs = new XElement("path");

            marker.Add(new XAttribute("id", "arrow"));
            marker.Add(new XAttribute("markerWidth", "10"));
            marker.Add(new XAttribute("markerHeight", "10"));
            marker.Add(new XAttribute("refx", "2"));
            marker.Add(new XAttribute("refy", "2"));
            marker.Add(new XAttribute("orient", "auto"));
            marker.Add(new XAttribute("markerUnits", "strokeWidth"));

            pathDefs.Add(new XAttribute("d", "M0,0 L0,4 L5,2 z"));
            pathDefs.Add(new XAttribute("fill", "rgb(0,0,0)"));

            marker.Add(pathDefs);
            defs.Add(marker);
            svg.Add(defs);
            svg = drawEdges(svg);

            XElement finalCircle = new XElement("circle");
            finalCircle.Add(new XAttribute("fill", "white"));
            finalCircle.Add(new XAttribute("stroke", "black"));
            finalCircle.Add(new XAttribute("stroke-width", "3"));
            finalCircle.Add(new XAttribute("r", radiRatio * 2));
            finalCircle.Add(new XAttribute("cx", "250"));
            finalCircle.Add(new XAttribute("cy", "250"));
            svg.Add(finalCircle);
            svg = drawDepartmentName(svg);
            svg.Add(secondaryCirclesLineGlobal);
            svg.Add(jaartallenSVG);
            return svg;



        }
        private XElement circleGen(List<String> colors, XElement svg, double radius, double nextradius, double dashStart, int jaartal, int jaarring, List<int> projectNummers2)
        {
            double radiusRatioEven = (degreeRatio * dashStart / 360);
            double radiusRatio = (degreeRatioFirst * dashStart / 360);
            double radiusRatio2 = (degreeRatioSecond * dashStart / 360);

            List<XElement> secondaryCircles = new List<XElement>();
            List<XElement> secondaryCirclesLine = new List<XElement>();
            List<int> projectNummers = projectNummers2;
            int segmentNumber = 0;

            for (int i = 0; i < levelListCount; i++)
            {
                String dasharray = "";


                if (levelListCount % 2 != 0)
                {
                    if ((i) <= (levelListCount / 2))
                    {

                        dasharray = dashStart - (radiusRatio * segmentNumber) + " " + dashStart;


                    }
                    else
                    {

                        dasharray = dashStart - (radiusRatio2 * (Convert.ToDouble(segmentNumber) + (1 + (0.002 * levelListCount)))) + " " + dashStart;

                    }
                }
                else
                {
                    dasharray = dashStart - (radiusRatioEven * segmentNumber) + " " + dashStart;

                }

                if (i == 0)
                {
                    dasharray = dashStart + " " + dashStart;
                }

                XElement circle = new XElement("circle");
                circle.Add(new XAttribute("r", radius));
                circle.Add(new XAttribute("cx", "250"));
                circle.Add(new XAttribute("cy", "250"));
                circle.Add(new XAttribute("fill-opacity", "0"));
                circle.Add(new XAttribute("stroke", chosenColors[i]));
                circle.Add(new XAttribute("stroke-width", (radius * 2)));
                circle.Add(new XAttribute("stroke-dasharray", dasharray));
                circle.Add(new XAttribute("stroke-opacity", "0.8"));
                svg.Add(circle);

                double[] jaarCoordinates = getCoords((radius - (radi[0] * 0.3)), (nextradius - (radi[0] * 0.3)), 1, 1, 1);

                if (i == 0)
                {
                    XElement g = new XElement("g");
                    g.Add(new XAttribute("transform", "rotate(" + ((degreeRatio * segmentNumber) * 2) + " , 250, 250)"));

                    XElement text = new XElement("text", jaartallen[jaarring]);
                    text.Add(new XAttribute("x", 250 + jaarCoordinates[0]));
                    text.Add(new XAttribute("y", 250 + jaarCoordinates[1]));
                    text.Add(new XAttribute("style", "font-family: arial black"));
                    text.Add(new XAttribute("fill", "white"));

                    text.Add(new XAttribute("stroke-width", 0.5));

                    text.Add(new XAttribute("stroke", "black"));
                    text.Add(new XAttribute("font-size", 10 - (jaartalverschil * 0.4)));
                    g.Add(text);
                    jaartallenSVG.Add(g);
                }
                segmentNumber++;
                int aantalprojecten = 0;

                if (levelList[i].subLevels != null)
                {
                    foreach (Level l in levelList[i].subLevels)
                    {
                        if (l.startDatum.Equals(jaartal))
                        {
                            aantalprojecten++;
                        }
                    }


                    for (int i2 = 0; i2 < levelList[i].subLevels.Count(); i2++)
                    {

                        if (levelList[i].subLevels[i2].startDatum.Equals(jaartal))
                        {
                            if (levelList[i].subLevels[i2].startDatum > levelList[i].subLevels[i2].eindDatum)
                            {
                                throw new Exception();
                            }
                            XElement[] line = secondaryCircleGen(radius, radi[Array.IndexOf(jaartallen, levelList[i].subLevels[i2].eindDatum)], nextradius, levelList[i].subLevels.Count(), projectNummers[i], i + 1, segmentNumber);

                            projectNummers2[i]++;
                            secondaryCircles.Add(line[0]);

                            secondaryCirclesLineGlobal.Add(line[1]);
                        }
                    }
                }
            }



            svg.Add(secondaryCircles);

            return svg;
        }


        private XElement drawEdges(XElement svg)
        {

            for (int i = 0; i < radi.Count(); i++)
            {

                XElement circle = new XElement("circle");
                circle.Add(new XAttribute("r", radi[i] * 2));
                circle.Add(new XAttribute("cx", "250"));
                circle.Add(new XAttribute("cy", "250"));
                circle.Add(new XAttribute("fill-opacity", "0"));
                circle.Add(new XAttribute("stroke-opacity", "0.7"));
                circle.Add(new XAttribute("stroke", "black"));
                circle.Add(new XAttribute("stroke-width", 1));
                svg.Add(circle);

            }
            return svg;
        }

        private XElement drawDepartmentName(XElement svg)
        {
            XElement pathDefs = new XElement("defs");
            svg.Add(pathDefs);

            for (int i = 0; i < levelListCount; i++)
            {

                if (levelListCount % 2 != 0)
                {
                    if ((i) < (levelListCount / 2))
                    {

                        double[] afdCoordinates = getCoords(radi[radi.Count() - 1] + 10, radi[radi.Count() - 1] + 7, 1, 1, (0.97 * ((degreeRatioSecond * 0.7) + (degreeRatioSecond) * ((i + 1) * 2))));
                        double[] afdCoordinatesCheckpoint = getCoords(radi[radi.Count() - 1] + 10, radi[radi.Count() - 1] + 11, 1, 1, (((degreeRatioSecond * 0.7) + (degreeRatioSecond * ((i + 1) * 2)) + ((15 - (levelListCount))))));
                        double[] afdCoordinatesFinal = getCoords(radi[radi.Count() - 1] + 10, radi[radi.Count() - 1] + 7, 1, 1, (((degreeRatioSecond * 0.7) + (degreeRatioSecond * ((i + 1) * 2)) + ((34 - (levelListCount))))));

                        svg = drawDepartmentNameDraw(afdCoordinatesFinal, afdCoordinatesCheckpoint, afdCoordinates, pathDefs, i, 2, svg);


                    }
                    else {
                        double[] afdCoordinates = getCoords(radi[radi.Count() - 1] + 10, radi[radi.Count() - 1] + 7, 1, 1, (0.97 * ((degreeRatioFirst) + (degreeRatioFirst) * (i * 2))) + i * 1);
                        double[] afdCoordinatesCheckpoint = getCoords(radi[radi.Count() - 1] + 10, radi[radi.Count() - 1] + 11, 1, 1, (((degreeRatioFirst) + (degreeRatioFirst * (i * 2)) + ((17 - (levelListCount))))));
                        double[] afdCoordinatesFinal = getCoords(radi[radi.Count() - 1] + 10, radi[radi.Count() - 1] + 7, 1, 1, (((degreeRatioFirst) + (degreeRatioFirst * (i * 2)) + ((34 - (levelListCount))))));
                        if (i >= (levelListCount / 2))
                        {



                            if (i != levelListCount - 1)
                            {

                                svg = drawDepartmentNameDraw(afdCoordinates, afdCoordinatesCheckpoint, afdCoordinatesFinal, pathDefs, i, 2, svg);
                            }
                            else
                            {
                                afdCoordinates = getCoords(radi[radi.Count() - 1] + 10, radi[radi.Count() - 1] + 7, 1, 1, 1.01 * (0.75 * ((degreeRatioSecond) + (levelListCount) - (levelListCount * 2))));
                                afdCoordinatesCheckpoint = getCoords(radi[radi.Count() - 1] + 10, radi[radi.Count() - 1] + 11, 1, 1, 1.1 * (((degreeRatioSecond) + (levelListCount) - (levelListCount * 1.25))));
                                afdCoordinatesFinal = getCoords(radi[radi.Count() - 1] + 10, radi[radi.Count() - 1] + 7, 1, 1, 1.2 * (((degreeRatioSecond) + (levelListCount))));
                                // FIXED

                                if (levelListCount == 1)
                                {

                                    afdCoordinates = getCoords(radi[radi.Count() - 1] + 10, radi[radi.Count() - 1] + 7, 1, 1, 1.01 * (0.75 * ((100) + (levelListCount) - (levelListCount * 3))));
                                    afdCoordinatesCheckpoint = getCoords(radi[radi.Count() - 1] + 10, radi[radi.Count() - 1] + 11, 1, 1, 1.1 * (((100) + (levelListCount) - (levelListCount * 1.25))));
                                    afdCoordinatesFinal = getCoords(radi[radi.Count() - 1] + 10, radi[radi.Count() - 1] + 7, 1, 1, 1.2 * (((100) + (levelListCount))));


                                }


                                Console.WriteLine("gogogo");
                                int colorInt;
                                if (chosenColors.Count != 1)
                                {
                                    colorInt = 2;
                                }
                                else
                                {
                                    colorInt = 1;
                                }

                                svg = drawDepartmentNameDraw(afdCoordinatesFinal, afdCoordinatesCheckpoint, afdCoordinates, pathDefs, i, colorInt, svg);
                            }
                        }
                        else
                        {

                            int colorInt;
                            if (chosenColors.Count != 1)
                            {
                                colorInt = 2;
                            }
                            else
                            {
                                colorInt = 1;

                            }
                            svg = drawDepartmentNameDraw(afdCoordinates, afdCoordinatesCheckpoint, afdCoordinatesFinal, pathDefs, i, colorInt, svg);
                        }
                    }
                }
                else
                {
                    double[] afdCoordinates = getCoords(radi[radi.Count() - 1] + 10, radi[radi.Count() - 1] + 7, 1, 1, (0.97 * ((degreeRatio) + (degreeRatio) * (i * 2))) + (1 * i));
                    double[] afdCoordinatesCheckpoint = getCoords(radi[radi.Count() - 1] + 10, radi[radi.Count() - 1] + 11, 1, 1, (((degreeRatio) + (degreeRatio * (i * 2)) + ((15 - (levelListCount))))));
                    double[] afdCoordinatesFinal = getCoords(radi[radi.Count() - 1] + 10, radi[radi.Count() - 1] + 7, 1, 1, (((degreeRatio) + (degreeRatio * (i * 2)) + ((34 - (levelListCount))))));
                    if (i >= (levelListCount / 2))
                    {

                        svg = drawDepartmentNameDraw(afdCoordinates, afdCoordinatesCheckpoint, afdCoordinatesFinal, pathDefs, i, 1, svg);

                    }
                    else
                    {


                        svg = drawDepartmentNameDraw(afdCoordinatesFinal, afdCoordinatesCheckpoint, afdCoordinates, pathDefs, i, 1, svg);

                    }

                }

            }
            return svg;

        }

        private XElement drawDepartmentNameDraw(double[] afdCoordinates, double[] afdCoordinatesCheckpoint, double[] afdCoordinatesFinal, XElement pathDefs, int i, int colorModifier, XElement svg)
        {


            XElement pathDefault = new XElement("path");
            pathDefault.Add(new XAttribute("id", "default" + i));
            pathDefault.Add(new XAttribute("d", "M " + (250 + afdCoordinates[0]) + " , " + (250 + afdCoordinates[1]) +
                " Q " + (250 + afdCoordinatesCheckpoint[0]) + " , " + (250 + afdCoordinatesCheckpoint[1])
                + " " + (250 + afdCoordinatesFinal[0]) + " , " + (250 + afdCoordinatesFinal[1])));
            pathDefs.Add(pathDefault);

            String color;
            color = chosenColors[chosenColors.Count - colorModifier];
            chosenColors.RemoveAt(chosenColors.Count - colorModifier);

            XElement text = new XElement("text");
            text.Add(new XAttribute("style", "font-family: arial"));
            text.Add(new XAttribute("font-size", 15 - (levelList[i].name.Length * 0.28)));
            text.Add(new XAttribute("fill", color));
            text.Add(new XAttribute("stroke", "black"));
            text.Add(new XAttribute("stroke-width", 0.4));
            text.Add(new XAttribute("fill-opacity", "0.7"));

            XAttribute tas = new XAttribute(xlink + "href", "#default" + i);
            XElement textpath = new XElement("textPath", tas, levelList[i].name);

            text.Add(textpath);
            svg.Add(text);
            return svg;

        }

        private XElement[] secondaryCircleGen(double radius, double endradius, double nextradius, int aantalprojecten, int projectNummer, int afdNummer, int segmentNumber)
        {

            XElement[] list = new XElement[2];

            double endradiusNextradius;
            if (Array.IndexOf(radi, endradius).Equals(0))
            {
                endradiusNextradius = radius;
            }
            else
            {
                endradiusNextradius = radi[Array.IndexOf(radi, endradius) - 1];
            }

            double[] startCoordinates;
            double[] startLineCoordinates;
            double[] endCoordinates;
            double chosenRatio;
            int chosenSegment;
            if (levelListCount % 2 != 0)
            {
                if ((afdNummer - 1) < (levelListCount / 2))
                {
                    chosenRatio = degreeRatioSecond;
                    chosenSegment = segmentNumber;



                }
                else
                {

                    chosenRatio = degreeRatioFirst;
                    chosenSegment = segmentNumber - 1;

                    if (afdNummer.Equals(levelListCount))
                    {
                        chosenRatio = degreeRatioSecond;
                        chosenSegment = segmentNumber + 1;

                    }
                }

            }
            else
            {
                chosenRatio = degreeRatio;
                chosenSegment = segmentNumber - 1;

            }
            startCoordinates = getCoords(radius, nextradius, aantalprojecten, projectNummer, chosenRatio);
            startLineCoordinates = getCoords(radius, nextradius - 7, aantalprojecten, projectNummer, chosenRatio);
            endCoordinates = getCoords(endradius, endradiusNextradius, aantalprojecten, projectNummer, chosenRatio);
            list = secondaryCircleGenDraw(startCoordinates, chosenSegment, endradius, startLineCoordinates, endCoordinates, chosenRatio);
            return list;
        }

        private XElement[] secondaryCircleGenDraw(double[] startCoordinates, int segmentNumber, double endradius, double[] startLineCoordinates, double[] endCoordinates, double degreeRatioInc)
        {
            XElement[] list = new XElement[2];

            XElement circle = new XElement("circle");
            circle.Add(new XAttribute("r", 7));
            circle.Add(new XAttribute("cx", 250 + startCoordinates[0]));
            circle.Add(new XAttribute("cy", 250 + startCoordinates[1]));
            circle.Add(new XAttribute("fill", "white"));
            circle.Add(new XAttribute("stroke", "black"));
            circle.Add(new XAttribute("stroke-width", 1));
            circle.Add(new XAttribute("transform", "rotate(" + (degreeRatioInc * segmentNumber) + " 250 250)"));


            if (!Array.IndexOf(radi, endradius).Equals(0))
            {
                XElement line = new XElement("line");
                line.Add(new XAttribute("x1", 250 + startLineCoordinates[0]));
                line.Add(new XAttribute("y1", 250 + startLineCoordinates[1]));
                line.Add(new XAttribute("x2", 250 + endCoordinates[0]));
                line.Add(new XAttribute("y2", 250 + endCoordinates[1]));
                line.Add(new XAttribute("style", "stroke:rgb(0,0,0);stroke-width:2"));
                line.Add(new XAttribute("transform", "rotate(" + (degreeRatioInc * segmentNumber) + " 250 250)"));
                line.Add(new XAttribute("marker-end", "url(#arrow)"));
                list[1] = line;
            }

            list[0] = circle;
            return list;

        }


        private double[] getCoords(double radius, double nextradius, int aantalprojecten, int projectNummer, double degreeRatio)
        {
            double trueRadius = (radius * 2);
            double endRadius = (nextradius * 2);

            double currentRadius = ((trueRadius - endRadius) / 2) + endRadius;

            double degreeToRadian = Math.PI * (degreeRatio) / 180.0;
            double oppositeSideCos = Math.Cos(degreeToRadian / (1 + aantalprojecten) * projectNummer);
            double XCoord = oppositeSideCos * currentRadius;


            double oppositeSideSin = Math.Sin(degreeToRadian / (1 + aantalprojecten) * projectNummer);
            double YCoord = oppositeSideSin * currentRadius;

            return new double[] { XCoord, YCoord };
        }


    }
}
