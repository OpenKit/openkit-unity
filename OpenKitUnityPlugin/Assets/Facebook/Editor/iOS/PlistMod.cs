using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Text;

namespace UnityEditor.FacebookEditor
{
    public class PlistMod
    {
        private static XmlNode FindPlistDictNode(XmlDocument doc)
        {
            XmlNode curr = doc.FirstChild;
            while(curr != null)
            {
                if(curr.Name.Equals("plist") && curr.ChildNodes.Count == 1)
                {
                    XmlNode dict = curr.FirstChild;
                    if(dict.Name.Equals("dict"))
                        return dict;
                }
                curr = curr.NextSibling;
            }
            return null;
        }
        
        private static XmlElement AddChildElement(XmlDocument doc, XmlNode parent, string elementName, string innerText=null)
        {
            XmlElement newElement = doc.CreateElement(elementName);
            if(innerText != null && innerText.Length > 0)
                newElement.InnerText = innerText;
            
            parent.AppendChild(newElement);
            return newElement;
        }
        
        public static void UpdatePlist(string path, string appId)
        {
            string fileName = "Info.plist";
            string fullPath = Path.Combine(path, fileName);
            
            if(appId == null || appId.Length <= 0 || appId.Equals("0"))
            {
                Debug.LogError("You didn't specify a Facebook app ID.  Please add one using the Facebook menu in the main Unity editor.");
                return;
            }
            
            XmlDocument doc = new XmlDocument();
            doc.Load(fullPath);
            
            if(doc == null)
            {
                Debug.LogError("Couldn't load " + fullPath);
                return;
            }
            
            XmlNode dict = FindPlistDictNode(doc);
            if(dict == null)
            {
                Debug.LogError("Error parsing " + fullPath);
                return;
            }
            
            //add the app id to the plist
            //the xml should end up looking like this
            /*
            <key>FacebookAppID</key>
            <string>YOUR_APP_ID</string>
             */
            AddChildElement(doc, dict, "key", "FacebookAppID");
            AddChildElement(doc, dict, "string", appId);
            
            
            //here's how the custom url scheme should end up looking
            /*
             <key>CFBundleURLTypes</key>
             <array>
                 <dict>
                     <key>CFBundleURLSchemes</key>
                     <array>
                         <string>fbYOUR_APP_ID</string>
                     </array>
                 </dict>
             </array>
            */
            /*XmlElement urlSchemeKey = */AddChildElement(doc, dict, "key", "CFBundleURLTypes");
            XmlElement urlSchemeTop = AddChildElement(doc, dict, "array");
            {
                XmlElement urlSchemeDict = AddChildElement(doc, urlSchemeTop, "dict");
                {
                    /*XmlElement schemeKey = */AddChildElement(doc, urlSchemeDict, "key", "CFBundleURLSchemes");
                    
                    XmlElement innerArray = AddChildElement(doc, urlSchemeDict, "array");
                    {
                        /*XmlElement finallyTheSValue = */AddChildElement(doc, innerArray, "string", "fb" + appId);
                    }
                }
            }
            
            
            doc.Save(fullPath);
            
            //the xml writer barfs writing out part of the plist header.
            //so we replace the part that it wrote incorrectly here
            System.IO.StreamReader reader = new System.IO.StreamReader(fullPath);
            string textPlist = reader.ReadToEnd();
            reader.Close();
            
            int fixupStart = textPlist.IndexOf("<!DOCTYPE plist PUBLIC");
            if(fixupStart <= 0)
                return;
            int fixupEnd = textPlist.IndexOf('>', fixupStart);
            if(fixupEnd <= 0)
                return;
            
            string fixedPlist = textPlist.Substring(0, fixupStart);
            fixedPlist += "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">";
            fixedPlist += textPlist.Substring(fixupEnd+1);
            
            System.IO.StreamWriter writer = new System.IO.StreamWriter(fullPath, false);
            writer.Write(fixedPlist);
            writer.Close();
        }
    }
}