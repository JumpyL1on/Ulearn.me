using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Generics.BinaryTrees
{
    public class BinaryTree<T> : IEnumerable<T> where T : IComparable<T>
    {
        public BinaryTree(T value)
        {
            Value = value;
        }
		
        public BinaryTree() { }
		
        public T Value { get; set; }
        public BinaryTree<T> Left { get; set; }
        public BinaryTree<T> Right { get; set; }
		private List<T> nodes = new List<T>();
		public List<T> Nodes
        {
            get
            {
                return nodes;
            }
            set
            {
                for (var i = 0; i < nodes.Count; i++)
                    if (value[0].CompareTo(nodes[i]) <= 0)
                    {
                        nodes.Insert(i, value[0]);
                        return;
                    }
                nodes.Add(value[0]);
            }
        }
        public void Add(T value)
        {
            if (Value.CompareTo(default(T)) == 0) Value = value;
            else
			{
			    var binaryTree = this;
                while (true)
                {
					if (value.CompareTo(binaryTree.Value) <= 0 && binaryTree.Left != null)
                        binaryTree = binaryTree.Left;
                    if (value.CompareTo(binaryTree.Value) > 0 && binaryTree.Right != null)
                        binaryTree = binaryTree.Right;
                    if (value.CompareTo(binaryTree.Value) <= 0 && binaryTree.Left == null)
                    {
                        binaryTree.Left = new BinaryTree<T>(value);
                        break;
                    }
                    if (value.CompareTo(binaryTree.Value) > 0 && binaryTree.Right == null)
                    {
                        binaryTree.Right = new BinaryTree<T>(value);
                        break;
                    }
                }
		    }
			Nodes = new List<T> { value };
        }
		
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var e in nodes)
			    yield return e;
        }
		
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
	
    public class BinaryTree
    {
        public static BinaryTree<T> Create<T>(IEnumerable<T> collection) where T : IComparable<T>
        {
            var tree = new BinaryTree<T>();
            foreach (var e in collection)
                tree.Add(e);
            return tree;
        }
		
        public static BinaryTree<T> Create<T>(params T[] collection) where T : IComparable<T>
        {
            var tree = new BinaryTree<T>();
            foreach (var e in collection)
                tree.Add(e);
            return tree;
        }
    }
}
