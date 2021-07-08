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
            Item priorItem = null;
            Item curItem = null;

            var rootClasses = node.GetRootClasses();
            if (rootClasses.Count() > 0)
            {
                curItem = new Item()
                {
                    Type = rootClasses.ToArray(),
                    Properties = new Dictionary<string, object[]>()
                };

                if (!string.IsNullOrEmpty(node.Id))
                {
                    curItem.Id = node.Id;
                }

                if (this.curItem == null)
                {
                    this.curdata.Items.Add(curItem);
                }
                else
                {
                    this.curItem.HasNestedMicroformats = true;
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
                if (!curItem.Properties.ContainsKey("name") && !curItem.HasEProperties && !curItem.HasPProperties)
                {
                    string name = node.ParseImpliedName(baseUrl);
                    if (!string.IsNullOrEmpty(name))
                    {
                        curItem.Properties["name"] = new[] { name };
                    }
                }

                this.curItem = priorItem;
            }

            var propertyClasses = node.GetPropertyClasses();
            foreach (string propertyClass in propertyClasses)
            {
                string[] class_parts = propertyClass.Split('-');
                string name = class_parts[1];
                string value = "";
                string alt = "";
                switch (class_parts[0])
                {
                    case "p":
                        if (this.curItem != null && !this.curItem.HasPProperties)
                        {
                            this.curItem.HasPProperties = true;
                        }
                        value = node.ParsePProperty(baseUrl);
                        break;
                    case "u":
                        if (this.curItem != null && !this.curItem.HasUProperties)
                        {
                            this.curItem.HasUProperties = true;
                        }
                        (value, alt) = node.ParseUProperty(baseUrl);
                            break;
                    case "e":
                        if (this.curItem != null && !this.curItem.HasEProperties)
                        {
                            this.curItem.HasEProperties = true;
                        }
                        
                        break;

                }
                if (this.curItem != null)
                {
                    object obj = null;
                    if (string.IsNullOrEmpty(alt))
                    {
                        obj = value;
                    }
                    else
                    {
                        obj = new Photo
                        {
                            Alt = alt,
                            Value = value
                        };
                    }

                    if (!this.curItem.Properties.ContainsKey(name))
                    {
                        this.curItem.Properties[name] = new[] { obj };
                    }
                    else
                    {
                        this.curItem.Properties[name] = this.curItem.Properties[name].Append(obj);
                    }

                }
            }

        }

        

        
    }
}
