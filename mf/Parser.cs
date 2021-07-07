using System.Linq;
using AngleSharp.Html.Parser;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AngleSharp.Html.Dom;

namespace mf
{
    public class Parser
    {
        private readonly Regex rootClassNames = new Regex($"^h-([a-z0-9]+-)?[a-z]+(-[a-z]+)*$");
        private readonly Regex propertyClassNames = new Regex($"^(p|u|dt|e)-([a-z0-9]+-)?[a-z]+(-[a-z]+)*$");

        private HtmlParser _parser;
        private Document curdata;
        private Item curItem;
        private Uri baseUrl;
        private bool baseFound;


        public Parser()
        {
            _parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });
        }

        public Document Parse(string html, Uri baseUrl)
        {
            curdata = new Document();
            this.baseUrl = baseUrl;

            var doc = _parser.ParseDocument(html);
            var root = doc.DocumentElement;

            walk(root, baseUrl);

            return curdata;
        }

        private void walk(IElement node, Uri baseUrl)
        {
            string nodeName = node.NodeName.ToLower();
            Item priorItem = null;
            Item curItem = null;

            var classes = getClasses(node);
            var rootClasses = classes.Where(c => rootClassNames.IsMatch(c));
            if (rootClasses.Count() > 0)
            {
                curItem = new Item()
                {
                    Type = classes.ToArray(),
                    Properties = new Dictionary<string, object>()
                };

                if (this.curItem == null)
                {
                    this.curdata.Items.Add(curItem);
                }

                priorItem = this.curItem;
                this.curItem = curItem;
            }

            foreach (var child in node.Children)
            {
                walk(child, baseUrl);
            }

            if (curItem != null)
            {
                // implicit properties
            }

            var propertyClasses = classes.Where(c => propertyClassNames.IsMatch(c));
            foreach (string propertyClass in propertyClasses)
            {
                string[] class_parts = propertyClass.Split('-');
                string name = class_parts[1];
                string value = "";
                switch (class_parts[0])
                {
                    case "p":

                        value = node.ParseValueClassPattern();
                        if (!string.IsNullOrEmpty(value))
                        {
                            break;
                        }
                        if ((nodeName == "abbr" || nodeName == "link") && !string.IsNullOrEmpty(node.Attributes["title"].Value))
                        {
                            value = node.Attributes["title"].Value.Trim();
                        }
                        else if ((nodeName == "data" || nodeName == "input") && !string.IsNullOrEmpty(node.Attributes["value"].Value))
                        {
                            value = node.Attributes["value"].Value.Trim();
                        }
                        else if ((nodeName == "img" || nodeName == "area") && !string.IsNullOrEmpty(node.Attributes["alt"].Value))
                        {
                            value = node.Attributes["alt"].Value.Trim();
                        }
                        else
                        {
                            value = node.GetTextValue(baseUrl);
                        }

                        break;

                }
                if (this.curItem != null)
                {
                    this.curItem.Properties[name] = value;
                }
            }

        }

        private string[] getClasses(IElement node)
        {
            var classAttr = node.Attributes["class"];
            if (classAttr == null)
            {
                return new string[] { };
            }

            return classAttr.NodeValue.Split(' ');
        }
    }
}
