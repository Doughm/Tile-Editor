//This class reads and writes to XML files

using System.Xml;
using System.IO;

using UnityEditor;
using UnityEngine;

class XMLeditor
{
    XmlDocument reader = new XmlDocument();

    //takes the file path and if the file exists open it
    public XMLeditor(string file)
    {
        if (File.Exists(file) == true)
        {
            reader.Load(file);
        }
    }

    //returns a value of an element with a given name in the root element.
    public string findValue(string type, string name, string value)
    {
        foreach (XmlNode xmlNode in reader.DocumentElement.ChildNodes)
        {
            if (xmlNode.LocalName == type && xmlNode.Attributes["name"].Value == name)
            {
                return xmlNode.Attributes[value].Value;
            }
        }
        return "getValue() error";
    }

    //returns a value of an element with a given index.
    public string findValue(int index, string value)
    {
        int counter = 0;
        foreach (XmlNode xmlNode in reader.DocumentElement.ChildNodes)
        {
            if (counter == index)
            {
                return xmlNode.Attributes[value].Value;
            }
            counter++;
        }
        return "getValue() error";
    }

    //returns the type of an element from a given name.
    public string findType(string name)
    {
        foreach (XmlNode xmlNode in reader.DocumentElement.ChildNodes)
        {
            if (xmlNode.Attributes["name"].Value == name)
            {
                return xmlNode.LocalName;
            }
        }
        return "findType(string) error";
    }

    //returns the type of an element from a given index.
    public string findType(int index)
    {
        int counter = 0;
        foreach (XmlNode xmlNode in reader.DocumentElement.ChildNodes)
        {
            if (counter == index)
            {
                return xmlNode.LocalName;
            }
            counter++;
        }
        return "findType(int) error";
    }

    //returns a string containing all values of a given type in the root element.
    public string listContents(string type, string value)
    {
        string tempStr = "";
        foreach (XmlNode xmlNode in reader.DocumentElement.ChildNodes)
        {
            if (xmlNode.LocalName == type)
            {
                tempStr += xmlNode.Attributes[value].Value + '\n';
            }
        }
        if (tempStr == "")
        {
            return "listContents() error";
        }
        tempStr = tempStr.TrimEnd('\n');
        return tempStr;
    }

    //returns a string containing all values of all items in the root element.
    public string listContents(string value)
    {
        string tempStr = "";
        foreach (XmlNode xmlNode in reader.DocumentElement.ChildNodes)
        {
            tempStr += xmlNode.Attributes[value].Value + '\n';
        }
        if (tempStr == "")
        {
            return "listContents() error";
        }
        tempStr = tempStr.TrimEnd('\n');
        return tempStr;
    }

    //returns the number of elements the root element has.
    public int numberOfElements()
    {
        return reader.DocumentElement.ChildNodes.Count;
    }

    //returns the number of elements with given type.
    public int numberOfElements(string type)
    {
        int tempInt = 0;
        foreach (XmlNode xmlNode in reader.DocumentElement.ChildNodes)
        {
            if (xmlNode.LocalName == type)
            {
                tempInt++;
            }
        }
        return tempInt;
    }

    //creates a level file at given path name
    public void saveLevelFile(string fileName, string[] tileName, int[] xPos, int[] yPos)
    {
        if (tileName.Length == xPos.Length && xPos.Length == yPos.Length)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlDeclaration xmldecl;
            xmldecl = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);

            XmlElement root = xmlDoc.DocumentElement;
            xmlDoc.InsertBefore(xmldecl, root);


            XmlNode rootNode = xmlDoc.CreateElement("Level");
            xmlDoc.AppendChild(rootNode);
            XmlNode userNode;
            XmlAttribute attribute;

            for (int i = 0; i < tileName.Length; i++)
            {
                userNode = xmlDoc.CreateElement("Tile");

                attribute = xmlDoc.CreateAttribute("TileType");
                attribute.Value = tileName[i];
                userNode.Attributes.Append(attribute);

                attribute = xmlDoc.CreateAttribute("Xpos");
                attribute.Value = xPos[i].ToString();
                userNode.Attributes.Append(attribute);

                attribute = xmlDoc.CreateAttribute("Ypos");
                attribute.Value = yPos[i].ToString();
                userNode.Attributes.Append(attribute);

                rootNode.AppendChild(userNode);
            }

            xmlDoc.Save(fileName);
        }
    }
}