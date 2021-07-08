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
        private readonly static Regex rootClassNames = new Regex($"^h-([a-z0-9]+-)?[a-z]+(-[a-z]+)*$");
        private readonly static Regex propertyClassNames = new Regex($"^(p|u|dt|e)-([a-z0-9-]+)?$");

        public static bool HasAttribute(this IElement node, string attr)
        {
            return !string.IsNullOrEmpty(node.Attributes[attr]?.Value);
        }

        public static string[] GetClasses(this IElement node)
        {
            var classAttr = node.Attributes["class"];
            if (classAttr == null)
            {
                return new string[] { };
            }

            return classAttr.NodeValue.Split(' ');
        }

        public static IEnumerable<string> GetPropertyClasses(this IElement node)
        {
            return node.GetClasses().Where(c => propertyClassNames.IsMatch(c));
        }

        public static IEnumerable<string> GetRootClasses(this IElement node)
        {
            return node.GetClasses().Where(c => rootClassNames.IsMatch(c));
        }

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

        public static (string, string) ParseEProperty(this IElement node, Uri baseUrl)
        {
            if (node == null)
            {
                return ("", "");
            }

            var htmlNode = node.Clone() as IElement;
            htmlNode.Traverse((INode parent, INode el) =>
            {
                if (el.NodeType == NodeType.Element)
                {
                    IElement element = el as IElement;
                    foreach (var attr in element.Attributes)
                    {
                        if (new[] { "src", "href" }.Contains(attr.Name) &&  Uri.IsWellFormedUriString(attr.Value, UriKind.Relative))
                        {
                            attr.Value = attr.Value.MakeAbsolute(baseUrl);
                        }

                    }
                }
            });
            string v = node.GetTextValue(baseUrl);
            
            return (System.Web.HttpUtility.HtmlDecode(v.Trim()), System.Web.HttpUtility.HtmlDecode(htmlNode.InnerHtml.Trim()));
        }

        public static string ParseImpliedName(this IElement node, Uri baseUrl)
        {
            Func<IElement, string> specialCaseEvaluator = (IElement el) =>
             {
                 string nodeName = el.NodeName.ToLower();
                 if (new[] { "img", "area" }.Contains(nodeName))
                 {
                     return el.GetAttribute("alt");
                 }
                 else if (nodeName == "abbr")
                 {
                     return el.GetAttribute("title");
                 }
                 return "";
             };

            string name = specialCaseEvaluator(node);
            
            if (name == "" && node.Children.Length == 1 )
            {
                var child = node.Children.First();
                if (child.GetRootClasses().Count() == 0)
                {
                    name = specialCaseEvaluator(child);
                }
                if (name == "" && child.Children.Length == 1 && child.Children.First().GetRootClasses().Count() == 0)
                {
                    var grandchild = child.Children.First();
                    name = specialCaseEvaluator(grandchild);
                }
            }

            if (string.IsNullOrEmpty(name))
            {
                name = node.GetTextValue(baseUrl);
            }
            return name.Trim();
        }

        public static object ParseImpliedPhoto(this IElement node, Uri baseUrl)
        {
            Func<IElement, (string, string)> specialCaseEvaluator = (IElement el) =>
            {
                string nodeName = el.NodeName.ToLower();
                if (new[] { "img" }.Contains(nodeName))
                {
                    return (el.GetAttribute("src"), el.GetAttribute("alt"));
                }
                else if (nodeName == "object")
                {
                    return (el.GetAttribute("data"), "");
                }
                return ("", "");
            };

            (string src, string alt) = specialCaseEvaluator(node);

            if (string.IsNullOrEmpty(src) && node.Children.Length == 1)
            {
                var child = node.Children.First();
                if (child.GetRootClasses().Count() == 0)
                {
                    (src, alt) = specialCaseEvaluator(child);
                }
                if (string.IsNullOrEmpty(src) && child.Children.Length == 1 && child.Children.First().GetRootClasses().Count() == 0)
                {
                    var grandchild = child.Children.First();
                    (src, alt) = specialCaseEvaluator(grandchild);
                }
            }

            if (!string.IsNullOrEmpty(src))
            {
                if (src.IsRelative())
                {
                    src = src.MakeAbsolute(baseUrl);
                }

                if (string.IsNullOrEmpty(alt))
                {
                    return src;
                }
                else
                {
                    return new Photo { Alt = alt, Value = src };
                }
            }
            return null;
        }

        public static string ParsePProperty(this IElement node, Uri baseUrl)
        {
            string nodeName = node.NodeName.ToLower();

            string value = node.ParseValueClassPattern();
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            if ((nodeName == "abbr" || nodeName == "link") && !string.IsNullOrEmpty(node.Attributes["title"]?.Value))
            {
                return node.Attributes["title"].Value.Trim();
            }
            else if ((nodeName == "data" || nodeName == "input") && !string.IsNullOrEmpty(node.Attributes["value"]?.Value))
            {
                return node.Attributes["value"].Value.Trim();
            }
            else if ((nodeName == "img" || nodeName == "area") && !string.IsNullOrEmpty(node.Attributes["alt"]?.Value))
            {
                return node.Attributes["alt"].Value.Trim();
            }
            else
            {
                return node.GetTextValue(baseUrl);
            }
        }

        public static (string, string) ParseUProperty(this IElement node, Uri baseUrl)
        {
            string nodeName = node.NodeName.ToLower();
            string value = node.ParseValueClassPattern();
            string name = "";

            if ((nodeName == "a" || nodeName == "area" || nodeName == "link")
                && !string.IsNullOrEmpty(node.Attributes["href"]?.Value))
            {
                value = node.Attributes["href"].Value;
            }
            else if (nodeName == "img" && node.HasAttribute("src"))
            {
                value = node.Attributes["src"].Value;
                name = node.Attributes["alt"]?.Value;
            }
            else if (new[] { "audio", "video", "iframe", "source" }.Contains(nodeName) && node.HasAttribute("src"))
            {
                value = node.Attributes["src"].Value;
            }
            else if (nodeName == "video" && node.HasAttribute("poster"))
            {
                value = node.Attributes["poster"].Value;
            }
            else if (nodeName == "object" && node.HasAttribute("data"))
            {
                value = node.Attributes["data"].Value;
            }
            else if (nodeName == "object" && node.HasAttribute("data"))
            {
                value = node.Attributes["data"].Value;
            }
            else if (!string.IsNullOrEmpty(value))
            {
                // Found Value Class Pattern
            }
            else if (nodeName == "abbr" && node.HasAttribute("title"))
            {
                value = node.Attributes["title"].Value;
            }
            else if (new[] { "data", "input" }.Contains(nodeName) && node.HasAttribute("value"))
            {
                value = node.Attributes["value"].Value;
            }
            else
            {
                value = node.GetTextValue(baseUrl);
            }

            return (value.MakeAbsolute(baseUrl), name);

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
                    string nodeName = el.NodeName.ToLower();
                    if (element.ClassList.Contains("value-title") && element.HasAttribute("title"))
                    {
                        addToValues(el, element.Attributes["title"].Value);
                    }
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


