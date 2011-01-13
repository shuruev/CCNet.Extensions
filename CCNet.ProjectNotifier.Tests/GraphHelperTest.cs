using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickGraph;

namespace CCNet.ProjectNotifier.Tests
{
	[TestClass]
	public class GraphHelperTest
	{
		[TestMethod]
		public void Removing_Explicit_Edges_1()
		{
			var graph = new AdjacencyGraph<int, Edge<int>>();
			graph.AddVertex(1);
			graph.AddVertex(2);
			graph.AddVertex(3);
			graph.AddEdge(new Edge<int>(1, 2));
			graph.AddEdge(new Edge<int>(2, 3));
			graph.AddEdge(new Edge<int>(1, 3));

			GraphHelper.RemoveExplicitEdges(graph);

			Assert.AreEqual(2, graph.EdgeCount);
			Assert.IsTrue(graph.ContainsEdge(1, 2));
			Assert.IsTrue(graph.ContainsEdge(2, 3));
		}

		[TestMethod]
		public void Removing_Explicit_Edges_2()
		{
			var graph = new AdjacencyGraph<int, Edge<int>>();
			graph.AddVertex(1);
			graph.AddVertex(2);
			graph.AddVertex(3);
			graph.AddVertex(4);
			graph.AddVertex(5);
			graph.AddVertex(6);
			graph.AddVertex(7);
			graph.AddEdge(new Edge<int>(1, 3));
			graph.AddEdge(new Edge<int>(1, 4));
			graph.AddEdge(new Edge<int>(1, 6));
			graph.AddEdge(new Edge<int>(3, 6));
			graph.AddEdge(new Edge<int>(4, 6));
			graph.AddEdge(new Edge<int>(2, 4));
			graph.AddEdge(new Edge<int>(2, 5));
			graph.AddEdge(new Edge<int>(2, 7));
			graph.AddEdge(new Edge<int>(4, 7));
			graph.AddEdge(new Edge<int>(5, 7));

			GraphHelper.RemoveExplicitEdges(graph);

			Assert.AreEqual(8, graph.EdgeCount);
			Assert.IsTrue(graph.ContainsEdge(1, 3));
			Assert.IsTrue(graph.ContainsEdge(1, 4));
			Assert.IsTrue(graph.ContainsEdge(2, 4));
			Assert.IsTrue(graph.ContainsEdge(2, 5));
			Assert.IsTrue(graph.ContainsEdge(3, 6));
			Assert.IsTrue(graph.ContainsEdge(4, 6));
			Assert.IsTrue(graph.ContainsEdge(4, 7));
			Assert.IsTrue(graph.ContainsEdge(5, 7));
		}
	}
}
