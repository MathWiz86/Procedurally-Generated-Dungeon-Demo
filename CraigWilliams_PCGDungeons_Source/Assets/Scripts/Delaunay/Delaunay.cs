/**************************************************************************************************/
/*!
\file   Delaunay.cs
\author Simon Zeni, Adapted by Vazgriz, Adapted by Craig Williams
\par    Last Updated
        2021-03-27

\brief
  A file containing implementation of the Delaunay Triangulation Method, which is useful for
  determining paths to various points on a map.

\par Bug List

\par References
  - https://github.com/vazgriz/DungeonGenerator
  - https://github.com/vazgriz/DungeonGenerator/blob/master/Assets/Graphs.cs
  - https://github.com/vazgriz/DungeonGenerator/blob/master/Assets/Prim.cs
  - https://github.com/vazgriz/DungeonGenerator/blob/master/Assets/Scripts2D/Delaunay2D.cs
  - https://www.i-programmer.info/projects/61-algorithms/534-minimum-spanning-tree.html
*/
/**************************************************************************************************/

using System;
using System.Collections.Generic;

namespace Delaunay
{
  /************************************************************************************************/
  /// <summary>
  /// A class instance of the  Delaunay Triangulation Method, which is useful for determining paths
  /// to various points on a map.
  /// </summary>
  public class Delaunay
  {
    /// <summary>The Vertices [See: <see cref="Vertex"/>] that make up the map.</summary>
    public List<Vertex> Vertices { get; private set; }
    /// <summary>The <see cref="Edge"/>s that make up the map.</summary>
    public List<Edge> Edges { get; private set; }
    /// <summary>The <see cref="Triangle"/>s that make up the map.</summary>
    public List <Triangle> Triangles { get; private set; }

    /// <summary>
    /// The default constructor for a <see cref="Delaunay"/> instance.
    /// </summary>
    private Delaunay()
    {
      Edges = new List<Edge>();
      Triangles = new List<Triangle>();
    }

    /// <summary>
    /// A function used to create a triangulation map via a <see cref="Delaunay"/> Triangulation.
    /// </summary>
    private void PerformTriangulation()
    {
      // Make sure vertices have been set.
      if (Vertices == null || Vertices.Count <= 0)
        return;

      // We first need to create a super triangle. Get the minimum and maximum points.
      float minX = Vertices[0].Position.X;
      float minY = Vertices[0].Position.Y;
      float maxX = minX;
      float maxY = minY;

      // Calculate the new min and max coordinates. Each check must be performed.
      int count = Vertices.Count;
      for (int i = 0; i < count; i++)
      {
        System.Numerics.Vector3 position = Vertices[i].Position;

        if (position.X < minX)
          minX = position.X;
        if (position.X > maxX)
          maxX = position.X;
        if (position.Y < minY)
          minY = position.Y;
        if (position.Y > maxY)
          maxY = position.Y;
      }

      float absoluteMax = Math.Max(maxX - minX, maxY - minY) * 2.0f; // Get the max coordinate.

      // Create the final super triangle. (STP = Super Triangle Point)
      Vertex stp1 = new Vertex(new System.Numerics.Vector3(minX - 1, minY - 1, 0));
      Vertex stp2 = new Vertex(new System.Numerics.Vector3(minX - 1, maxY + absoluteMax, 0));
      Vertex stp3 = new Vertex(new System.Numerics.Vector3(maxX + absoluteMax, minY - 1, 0));

      Triangles.Add(new Triangle(stp1, stp2, stp3));

      // Iterate through all vertices again, using the count stored from earlier.
      for (int i = 0; i < count; i++)
      {
        Vertex vertex = Vertices[i]; // Get the current vertex.
        List<Edge> polygon = new List<Edge>(); // The current polygon being formed.

        int triCount = Triangles.Count;
        for (int j = 0; j < triCount; j++)
        {
          Triangle triangle = Triangles[j];

          // If the triangle contains the current vertex in its circumference, it is not valid.
          if (triangle.CircumferenceContainsXY(vertex.Position))
          {
            triangle.marked = true; // Mark the triangle.

            // Add the triangle to the polygon of bad edges.
            polygon.Add(new Edge(triangle.A, triangle.B));
            polygon.Add(new Edge(triangle.B, triangle.C));
            polygon.Add(new Edge(triangle.C, triangle.A));
          }
        }

        // Remove all bad triangles.
        Triangles.RemoveAll((Triangle t) => t.marked);

        // We now need to remove any edges that are shared.
        int polyCount = polygon.Count;
        for (int j = 0; j < polyCount; j++)
        {
          for (int k = j + 1; k < polyCount; k++)
          {
            Edge edgeJ = polygon[j];
            Edge edgeK = polygon[k];

            // Mark any shared edges to be removed.
            if (Edge.AlmostEqualXY(edgeJ, edgeK))
            {
              edgeJ.marked = true;
              edgeK.marked = true;
            }
          }
        }

        // Remove all bad edges.
        polygon.RemoveAll((Edge e) => e.marked);

        // Update the polygon count and add in all good triangles.
        polyCount = polygon.Count;
        for (int j = 0; j < polyCount; j++)
        {
          Edge edge = polygon[j];
          Triangles.Add(new Triangle(edge.U, edge.V, vertex));
        }
      }

      // Remove all triangles that contain any of the super triangle's vertices.
      Triangles.RemoveAll((Triangle t) => t.ContainsPosition(stp1.Position) || t.ContainsPosition(stp2.Position) || t.ContainsPosition(stp3.Position));

      // Create the final set of unique edges. A hash set cannot contain duplicates.
      // This probably could be replaced with a dictionary.
      int finalCount = Triangles.Count;
      HashSet<Edge> finalEdges = new HashSet<Edge>();

      for (int i = 0; i < finalCount; i++)
      {
        // Get the edges of the current triangle.
        Triangle triangle = Triangles[i];
        Edge edgeAB = new Edge(triangle.A, triangle.B);
        Edge edgeBC = new Edge(triangle.B, triangle.C);
        Edge edgeCA = new Edge(triangle.C, triangle.A);

        // For each edge, if it is unique to the hash set, it is unique to the final list.
        if (finalEdges.Add(edgeAB))
          Edges.Add(edgeAB);
        if (finalEdges.Add(edgeBC))
          Edges.Add(edgeBC);
        if (finalEdges.Add(edgeCA))
          Edges.Add(edgeCA);
      }
    }
    
    /// <summary>
    /// A function used to create a minimum spanning tree out of a <see cref="Delaunay"/>
    /// Triangulation, via Prim's Algorithm.
    /// </summary>
    /// <param name="startIndex">The index of the starting edge to use. Defaults to 0.</param>
    /// <returns>Returns the final list of <see cref="Edge"/>s making up the tree.</returns>
    public HashSet<Edge> CreateMinimumSpanningTree(int startIndex = 0)
    {
      HashSet<Vertex> excluded = new HashSet<Vertex>(); // All excluded vertices.
      HashSet<Vertex> included = new HashSet<Vertex>(); // All included vertices.
      HashSet<Edge> finalEdges = new HashSet<Edge>(); // The final set of edges.
      
      // Add all vertices to the excluded set to start.
      int edgeCount = Edges.Count;

      for (int i = 0; i < edgeCount; i++)
      {
        Edge edge = Edges[i];
        excluded.Add(edge.U);
        excluded.Add(edge.V);
      }

      if (edgeCount > 0)
        included.Add(Edges[startIndex].U); // Add the starting vertex.
      
      // Iterate while there are still excluded vertices.
      while (excluded.Count > 0)
      {
        bool valid = false; // A check if the current pass is valid.
        Edge edge = null; // The selected edge to add to the final set.
        float minDistance = float.MaxValue; // The minimum distance recorded between vertices.

        // Iterate through all edges, finding the one with the smallest distance between vertices.
        for (int i = 0; i < edgeCount; i++)
        {
          Edge currentEdge = Edges[i]; // Get the current edge.
          bool UNotContained = !included.Contains(currentEdge.U);
          bool VNotContained = !included.Contains(currentEdge.V);

          // Progress if the current edge's vertices are not already included.
          if ((UNotContained && !VNotContained) || (VNotContained && !UNotContained))
          {
            // If the current edge has a smaller distance, we want to select this edge.
            if (currentEdge.SquareDistance < minDistance)
            {
              edge = currentEdge;
              valid = true;
              minDistance = currentEdge.SquareDistance;
            }
          }
        }

        // If no edge was chosen, the tree is complete.
        if (!valid)
          break;

        // Move the selected edge's vertices to the included set.
        excluded.Remove(edge.U);
        excluded.Remove(edge.V);
        included.Add(edge.U);
        included.Add(edge.V);

        finalEdges.Add(edge); // Append the selected edge.
      }

      
      return finalEdges;
    }

    /// <summary>
    /// A function that handles the creation of a <see cref="Delaunay"/> Triangulation instance.
    /// </summary>
    /// <param name="Vertices">The vertices [See: <see cref="Vertex"/>] that will be used
    /// to create the map.</param>
    /// <returns>Returns the final <see cref="Delaunay"/> Triangulation.</returns>
    public static Delaunay CreateDelaunayTriangulation(List<Vertex> Vertices)
    {
      Delaunay delaunay = new Delaunay(); // Create a fresh instance.
      delaunay.Vertices = new List<Vertex>(Vertices); // Copy the vertices.
      delaunay.PerformTriangulation(); // Perform the triangulation.
      return delaunay; // Return the finished map.
    }
  }
  /************************************************************************************************/
}