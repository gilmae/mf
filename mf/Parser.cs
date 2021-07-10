﻿using System.Linq;
using AngleSharp.Html.Parser;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;

namespace mf
{
    public class Parser
    {
        internal HtmlParser _parser;
        internal Document Document;
        internal Microformat currentItem;
        internal Uri baseUrl;


        public Parser()
        {
            _parser = new HtmlParser(new HtmlParserOptions
            {
                IsNotConsumingCharacterReferences = true,
            });
        }
    }
    public static class ParserExtensions
    {

        public static Document Parse(this Parser parser, string html, Uri baseUrl)

        {
            parser.Document = new Document();
            parser.baseUrl = baseUrl;

            var doc = parser._parser.ParseDocument(html);
            var baseInDoc = doc.QuerySelector("base['href']");
            if (baseInDoc != null && !string.IsNullOrEmpty(baseInDoc.NodeValue) && System.Uri.IsWellFormedUriString(baseInDoc.NodeValue, UriKind.Absolute))
            {
                parser.baseUrl = new Uri(baseInDoc.NodeValue);
            }
            var root = doc.DocumentElement;

            parser.walk(root);

            return parser.Document;
        }

        public static void walk(this Parser parser, IElement node)
        {
            Microformat priorItem = null;
            Microformat curItem = null;

            var rootClasses = node.GetRootClasses();
            if (rootClasses.Count() > 0)
            {
                curItem = new Microformat()
                {
                    Type = rootClasses.ToArray(),
                    Properties = new Dictionary<string, object[]>()
                };

                if (!string.IsNullOrEmpty(node.Id))
                {
                    curItem.Id = node.Id;
                }

                if (parser.currentItem == null)
                {
                    parser.Document.Items.Add(curItem);
                }
                else
                {
                    parser.currentItem.HasNestedMicroformats = true;
                }

                priorItem = parser.currentItem;
                parser.currentItem = curItem;
            }

            foreach (var child in node.Children)
            {
                parser.walk(child);
            }

            if (curItem != null)
            {
                if (!curItem.Properties.ContainsKey("name") && !curItem.HasEProperties && !curItem.HasPProperties)
                {
                    string name = node.ParseImpliedName(parser.baseUrl);
                    if (!string.IsNullOrEmpty(name))
                    {
                        curItem.Properties["name"] = new[] { name };
                    }
                }

                if (!curItem.Properties.ContainsKey("photo") && !curItem.HasUProperties)
                {
                    object photo = node.ParseImpliedPhoto(parser.baseUrl);
                    if (photo != null)
                    {
                        curItem.Properties["photo"] = new[] { photo };
                    }
                }

                if (!curItem.Properties.ContainsKey("url") && !curItem.HasUProperties)
                {
                    string url = node.ParseImpliedUrl(parser.baseUrl);
                    if (!string.IsNullOrEmpty(url))
                    {
                        curItem.Properties["url"] = new[] { url };
                    }
                }

                parser.currentItem = priorItem;
            }

            var propertyClasses = node.GetPropertyClasses();
            foreach (string propertyClass in propertyClasses)
            {
                // TODO Handle vendor prefixes
                string[] class_parts = propertyClass.Split('-');
                string name = class_parts[1];

                object obj = null;
                switch (class_parts[0])
                {
                    case "p":
                        if (parser.currentItem != null && !parser.currentItem.HasPProperties)
                        {
                            parser.currentItem.HasPProperties = true;
                        }
                        obj = node.ParsePProperty(parser.baseUrl);
                        break;
                    case "u":
                        if (parser.currentItem != null && !parser.currentItem.HasUProperties)
                        {
                            parser.currentItem.HasUProperties = true;
                        }
                        (string value, string alt) = node.ParseUProperty(parser.baseUrl);
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
                        break;
                    case "e":
                        if (parser.currentItem != null && !parser.currentItem.HasEProperties)
                        {
                            parser.currentItem.HasEProperties = true;
                        }
                        (value, alt) = node.ParseEProperty(parser.baseUrl);
                        if (string.IsNullOrEmpty(alt))
                        {
                            obj = value;
                        }
                        else
                        {
                            obj = new EmbeddedMarkup
                            {
                                Html = alt,
                                Value = value
                            };
                        }
                        break;

                }
                if (parser.currentItem != null)
                {
                    if (!parser.currentItem.Properties.ContainsKey(name))
                    {
                        parser.currentItem.Properties[name] = new[] { obj };
                    }
                    else
                    {
                        parser.currentItem.Properties[name] = parser.currentItem.Properties[name].Append(obj);
                    }

                }
            }

        }
    }
}
