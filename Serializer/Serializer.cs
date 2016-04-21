using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using BddSharp.Kernel;
using BddSharp.AST;

namespace BddSharp.Serializer
{
    public class BddSerializer
    {
        private static Dictionary<int, bool> Visitor = new Dictionary<int, bool>();
        private static XmlDocument doc = new XmlDocument();

        public static void Serialize(Bdd root, string filename)
        {
            Visitor.Clear();
            doc.LoadXml("<document name=\"" + filename + "\" size=\"12,12\" />");

            //SerializeVarNames();

            SerializeNode(root);

            if(!System.IO.Directory.Exists("results"))
                System.IO.Directory.CreateDirectory("results");

            doc.Save("results\\" + filename + ".xml");

            // apply xsl and save text
            XslCompiledTransform transformer = new XslCompiledTransform();
            transformer.Load("Bdd.xslt");
            transformer.Transform("results\\" + filename + ".xml", "results\\" + filename + ".txt");

            // call graphviz
            string args = "-Tjpg -o \"results\\" + filename + ".jpg\" \"results\\" + filename + ".txt\"";
            System.Diagnostics.Process.Start("\"V:\\Graphviz\\bin\\dot.exe\"", args);
        }

        //Makes nice labels if you are using the frontend.
        private static void SerializeVarNames()
        {
            if (VarList.Vars.Count == 0)
            {
                return;     //not using frontend.
            }

            XmlNode vars = doc.CreateElement("vars");
            doc.AppendChild(vars);

            foreach (KeyValuePair<string, int> entry in VarList.Vars)
            {
                XmlNode node = doc.CreateElement("var");

                XmlElement nameNode = (XmlElement)doc.CreateNode("element", "name", "");
                nameNode.InnerText = entry.Key;
                XmlElement levelNode = (XmlElement)doc.CreateNode("element", "level", "");
                levelNode.InnerText = entry.Value.ToString();

                vars.AppendChild(nameNode);
                vars.AppendChild(levelNode);
                doc.ChildNodes.Item(0).AppendChild(node);
            }
        }

        private static void SerializeNode(Bdd u)
        {
            if (!Visitor.ContainsKey(u.U))
            {
                if (!u.Low.IsTerminal())
                    SerializeNode(u.Low);

                printNode(u);
                Visitor.Add(u.U, true);

                if (!u.High.IsTerminal())
                    SerializeNode(u.High);
            }
        }

        private static void printNode(Bdd u)
        {
            XmlNode node = doc.CreateElement("node");

            XmlElement uNode = (XmlElement)doc.CreateNode("element", "u", "");
            uNode.InnerText = u.ToString();
            XmlElement lowNode = (XmlElement)doc.CreateNode("element", "low", "");
            lowNode.InnerText = u.Low.ToString();
            XmlElement highNode = (XmlElement)doc.CreateNode("element", "high", "");
            highNode.InnerText = u.High.ToString();
            XmlElement levelNode = (XmlElement)doc.CreateNode("element", "level", "");
            levelNode.InnerText = u.Var.ToString();

            node.AppendChild(uNode);
            node.AppendChild(lowNode);
            node.AppendChild(highNode);
            node.AppendChild(levelNode);

            doc.ChildNodes.Item(0).AppendChild(node);

        }

    }
}
