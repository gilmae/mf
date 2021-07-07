using System;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace mf
{
    public static class INodeExtensions
    {
        public static void Traverse(this INode node, Action<INode, INode> evaluator)
        {
            foreach (var c in node.Descendents())
            {
                evaluator(node, c);
            }
        }
    }
}
