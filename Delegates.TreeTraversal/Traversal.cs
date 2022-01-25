using System;
using System.Collections.Generic;

namespace Delegates.TreeTraversal
{
    public abstract class Traversal
    {
        private static readonly List<Job> EndJobs = new List<Job>();
        private static readonly List<Product> Products = new List<Product>();
        private static readonly List<int> BinaryTreeValues = new List<int>();

        public static IEnumerable<int> GetBinaryTreeValues(BinaryTree<int> data)
        {
            TreeTraversal(
                binaryTree => BinaryTreeValues.Add(binaryTree.Value),
                () =>
                {
                    if (data.Left != null)
                        GetBinaryTreeValues(data.Left);
                    if (data.Right != null)
                        GetBinaryTreeValues(data.Right);
                }, 
                data);
            return BinaryTreeValues;
        }

        public static IEnumerable<Job> GetEndJobs(Job data)
        {
            TreeTraversal(
                job =>
                {
                    if (job.Subjobs.Count == 0)
                        EndJobs.Add(job);
                },
                () => data.Subjobs.ForEach(job => GetEndJobs(job)),
                data);
            return EndJobs;
        }

        public static IEnumerable<Product> GetProducts(ProductCategory data)
        {
            TreeTraversal(
                productCategory => productCategory.Products.ForEach(product => Products.Add(product)),
                () => data.Categories.ForEach(productCategory => GetProducts(productCategory)),
                data);
            return Products;
        }

        private static void TreeTraversal<T>(
            Action<T> addValueToResultByCondition, 
            Action checkAnotherValues, 
            T data)
        {
            addValueToResultByCondition(data);
            checkAnotherValues();
        }
    }
}
