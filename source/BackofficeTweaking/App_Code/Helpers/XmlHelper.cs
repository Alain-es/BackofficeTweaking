using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

// Code taken from: https://packageactioncontrib.codeplex.com/SourceControl/latest#PackageActionsContrib/Helpers/XmlHelper.cs

namespace BackofficeTweaking.Helpers
{
    public class XmlHelper
    {
        /// <summary>
        /// Gets the value from an attribute or returns an empty string if it wasn't specified
        /// </summary>
        public static string GetAttributeValueFromNode(XmlNode node, string attributeName)
        {
            return GetAttributeValueFromNode<string>(node, attributeName, string.Empty);
        }

        /// <summary>
        /// Gets the value from an attribute or returns defaultValue if it wasn't specified
        /// </summary>
        public static string GetAttributeValueFromNode(XmlNode node, string attributeName, string defaultValue)
        {
            return GetAttributeValueFromNode<string>(node, attributeName, defaultValue);
        }

        /// <summary>
        /// Gets the value from an attribute, if no value or empty, it returns your default value (everything converted to the right type).
        /// </summary>
        public static T GetAttributeValueFromNode<T>(XmlNode node, string attributeName, T defaultValue)
        {
            if (node.Attributes[attributeName] != null)
            {
                string result = node.Attributes[attributeName].InnerText;
                if (string.IsNullOrEmpty(result))
                    return defaultValue;

                return (T)Convert.ChangeType(result, typeof(T));
            }
            return defaultValue;
        }


    }

}