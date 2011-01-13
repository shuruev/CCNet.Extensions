using System.Collections.Generic;
using QuickGraph;
using QuickGraph.Algorithms;

namespace CCNet.ProjectNotifier
{
	/// <summary>
	/// Working with graphs.
	/// </summary>
	public class GraphHelper
	{
		/// <summary>
		/// Removes explicit edges from specified graph.
		/// </summary>
		public static void RemoveExplicitEdges<T>(AdjacencyGraph<T, Edge<T>> graph)
		{
			graph.RemoveEdgeIf(delegate(Edge<T> edge)
				{
					var temp = graph.Clone();
					temp.RemoveEdge(edge);

					return HasPath(temp, edge.Source, edge.Target);
				});
		}

		/// <summary>
		/// Checks whether there is a path from one vertex to another.
		/// </summary>
		private static bool HasPath<T>(IVertexAndEdgeListGraph<T, Edge<T>> graph, T source, T target)
		{
			var dfs = graph.ShortestPathsDijkstra(edge => 1, source);
			IEnumerable<Edge<T>> path;
			bool result = dfs.Invoke(target, out path);
			return result;
		}
	}
}
