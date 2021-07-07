using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Text.RegularExpressions;

namespace mf
{
    public static class IElementExtensions
    {
        public static string GetTextValue(this IElement node, Uri baseUrl)
        {
            node.Traverse((INode parent, INode el) =>
            {
                if (el.NodeType == NodeType.Element)
                {
                    IElement element = el as IElement;
                    string nodeName = el.NodeName.ToLower();
                    if (nodeName == "script" || nodeName == "style")
                    {
                        el.RemoveFromParent();
                    }
                    else if (nodeName == "img")
                    {
                        if (element.Attributes["alt"] != null)
                        {
                            string value = element.Attributes["alt"].NodeValue;
                            el.Parent.ReplaceChild(parent.Owner.CreateTextNode(value), el);
                        }
                        else if (element.Attributes["src"] != null)
                        {
                            string value = element.Attributes["src"].NodeValue.MakeAbsolute(baseUrl);
                            el.Parent.ReplaceChild(parent.Owner.CreateTextNode($" {value} "), el);
                        }
                    }
                }
            });
            return node.TextContent.Trim();
        }

        public static string ParseValueClassPattern(this IElement node)
        {
            if (node == null) {
                return "";
            }

            
            var values = new List<string>();
            var processed = new List<INode>();

            Action<INode, string> addToValues = (INode n, string v) =>
            {
                values.Add(v);
                processed.Add(n);
            };

            node.Traverse((INode parent, INode el) =>
            {
                if (el.NodeType == NodeType.Element && !processed.Contains(el.ParentElement))
                {
                    IElement element = el as IElement;
                    string nodeName = el.NodeName;
                    if (element.ClassList.Contains("value"))
                    {
                        if (nodeName == "img" || nodeName == "area")
                        {
                            if (element.Attributes["alt"] != null)
                            {
                                addToValues(el, element.Attributes["alt"].NodeValue);
                            }
                        }
                        else if (nodeName == "data")
                        {
                            if (element.Attributes["value"] != null)
                            {
                                addToValues(el, element.Attributes["value"].NodeValue);
                            }
                            else
                            {
                                addToValues(el, node.InnerHtml);
                            }
                        }
                        else if (nodeName == "abbr")
                        {
                            if (element.Attributes["title"]!= null)
                            {
                                addToValues(el, element.Attributes["title"].NodeValue);
                            }
                            else
                            {
                                addToValues(el, node.InnerHtml);
                            }
                        }
                        else
                        {
                            addToValues(el, element.InnerHtml);
                        }
                    }
                }
            });
            return string.Join("", values.ToArray().Select(v => Regex.Replace(Regex.Replace(v, @"\s{2,}", " "), @">\s+<", "><").Trim()));
        }
    }
}


