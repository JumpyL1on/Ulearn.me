using System;

namespace FluentApi.Graph
{
    public enum NodeShape
    {
        Box,
        Ellipse
    }

    public class BaseBuilder
    {
        public EdgeBuilder AddEdge(string node1, string node2)
        {
            return new DotGraphBuilder().AddEdge(node1, node2);
        }

        public NodeBuilder AddNode(string node)
        {
            return new DotGraphBuilder().AddNode(node);
        }

        public string Build()
        {
            return new DotGraphBuilder().Build();
        }
    }

    public class EdgeBuilder : BaseBuilder
    {
        private readonly EdgeAttributes _edgeAttributes;

        public EdgeBuilder(EdgeAttributes edgeAttributes)
        {
            _edgeAttributes = edgeAttributes;
        }

        public DotGraphBuilder With(Action<EdgeAttributes> attributeAssignment)
        {
            attributeAssignment(_edgeAttributes);
            return new DotGraphBuilder();
        }
    }

    public class EdgeAttributes
    {
        private readonly GraphEdge _edge;

        public EdgeAttributes(GraphEdge edge)
        {
            _edge = edge;
        }

        public EdgeAttributes Label(string label)
        {
            _edge.Attributes["label"] = label;
            return this;
        }

        public EdgeAttributes FontSize(int fontSize)
        {
            _edge.Attributes["fontsize"] = fontSize.ToString();
            return this;
        }

        public EdgeAttributes Color(string color)
        {
            _edge.Attributes["color"] = color;
            return this;
        }

        public EdgeAttributes Weight(int weight)
        {
            _edge.Attributes["weight"] = weight.ToString();
            return this;
        }
    }

    public class NodeBuilder : BaseBuilder
    {
        private readonly NodeAttributes _nodeAttributes;

        public NodeBuilder(NodeAttributes nodeAttributes)
        {
            _nodeAttributes = nodeAttributes;
        }

        public DotGraphBuilder With(Action<NodeAttributes> attributeAssignment)
        {
            attributeAssignment(_nodeAttributes);
            return new DotGraphBuilder();
        }
    }

    public class NodeAttributes
    {
        private readonly GraphNode _node;

        public NodeAttributes(GraphNode node)
        {
            _node = node;
        }

        public NodeAttributes Label(string label)
        {
            _node.Attributes["label"] = label;
            return this;
        }

        public NodeAttributes FontSize(int fontSize)
        {
            _node.Attributes["fontsize"] = fontSize.ToString();
            return this;
        }

        public NodeAttributes Color(string color)
        {
            _node.Attributes["color"] = color;
            return this;
        }

        public NodeAttributes Shape(NodeShape shape)
        {
            _node.Attributes["shape"] = shape.ToString().ToLower();
            return this;
        }
    }

    public class DotGraphBuilder
    {
        private static Graph _graph;

        public static DotGraphBuilder DirectedGraph(string graphName)
        {
            _graph = new Graph(graphName, true, true);
            return new DotGraphBuilder();
        }

        public static DotGraphBuilder NondirectedGraph(string graphName)
        {
            _graph = new Graph(graphName, false, true);
            return new DotGraphBuilder();
        }

        public EdgeBuilder AddEdge(string node1, string node2)
        {
            return new EdgeBuilder(new EdgeAttributes(_graph.AddEdge(node1, node2)));
        }

        public NodeBuilder AddNode(string node)
        {
            return new NodeBuilder(new NodeAttributes(_graph.AddNode(node)));
        }

        public string Build()
        {
            return _graph.ToDotFormat();
        }
    }
}