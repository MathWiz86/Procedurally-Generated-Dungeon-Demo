/**************************************************************************************************/
/*!
\file   Geometry.cs
\author Braxton DeHate
\par    Last Updated
        2021-04-03

\brief
  Library containing functionality useful for procedurally creating level geometry.

\par Bug List

\par references
*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCGDungeon
{
  public static class Geometry
  {
    /// <summary>
    /// Creates a quad oriented as a wall.
    /// </summary>
    /// <param name="cornerA">Bottom left corner of the wall.</param>
    /// <param name="cornerB">Top right corner of the wall.</param>
    /// <param name="vertices">Vertex array to add to.</param>
    /// <param name="indices">Index array to add to.</param>
    public static void CreateWallQuad(Vector3 cornerA, Vector3 cornerB,
        ref List<Vector3> vertices, ref List<int> indices)
    {
      // Corner C is above/below cornerA, at the height of B
      Vector3 cornerC = new Vector3(cornerA.x, cornerB.y, cornerA.z);
      // Corner D is above/below cornerB, at the height of A
      Vector3 cornerD = new Vector3(cornerB.x, cornerA.y, cornerB.z);

      CreateQuadIndependent(cornerA, cornerB, cornerC, cornerD, ref vertices, ref indices);
    }

    /// <summary>
    /// Creates a quad oriented as a floor.
    /// </summary>
    /// <param name="cornerA">Bottom left corner of the floor.</param>
    /// <param name="cornerB">Top right corner of the floor.</param>
    /// <param name="vertices">Vertex array to add to.</param>
    /// <param name="indices">Index array to add to.</param>
    public static void CreateFloorQuad(Vector3 cornerA, Vector3 cornerB,
        ref List<Vector3> vertices, ref List<int> indices)
    {
      // Corner C is aligned with A on x and y, and B on z.
      Vector3 cornerC = new Vector3(cornerA.x, cornerA.y, cornerB.z);
      // Corner D is aligned with B on x and y, and A on z.
      Vector3 cornerD = new Vector3(cornerB.x, cornerB.y, cornerA.z);

      CreateQuadIndependent(cornerA, cornerB, cornerC, cornerD, ref vertices, ref indices);
    }

    /// <summary>
    /// Creates an arbitrary quad from 4 points. Adds 4 vertices.
    /// </summary>
    /// <param name="cornerA">Bottom left corner.</param>
    /// <param name="cornerB">Top right corner.</param>
    /// <param name="cornerC">Top left corner.</param>
    /// <param name="cornerD">Bottom right corner.</param>
    /// <param name="vertices">Vertex array to add to.</param>
    /// <param name="indices">Index array to add to.</param>
    public static void CreateQuad(Vector3 cornerA, Vector3 cornerB, Vector3 cornerC, Vector3 cornerD,
        ref List<Vector3> vertices, ref List<int> indices)
    {
      int indexA = vertices.Count;
      int indexB = indexA + 1;
      int indexC = indexB + 1;
      int indexD = indexC + 1;

      vertices.Add(cornerA);
      vertices.Add(cornerB);
      vertices.Add(cornerC);
      vertices.Add(cornerD);

      indices.Add(indexA);
      indices.Add(indexC);
      indices.Add(indexB);

      indices.Add(indexB);
      indices.Add(indexD);
      indices.Add(indexA);
    }

    /// <summary>
    /// Creates an arbitrary quad from 4 points. Adds each triangle as independent vertices
    /// for flat shading. Corners A and B are added twice.
    /// </summary>
    /// <param name="cornerA">Bottom left corner.</param>
    /// <param name="cornerB">Top right corner.</param>
    /// <param name="cornerC">Top left corner.</param>
    /// <param name="cornerD">Bottom right corner.</param>
    /// <param name="vertices">Vertex array to add to.</param>
    /// <param name="indices">Index array to add to.</param>
    public static void CreateQuadIndependent(Vector3 cornerA, Vector3 cornerB, Vector3 cornerC, Vector3 cornerD,
        ref List<Vector3> vertices, ref List<int> indices)
    {
      int indexA = vertices.Count;
      int indexB = indexA + 1;
      int indexC = indexB + 1;
      int indexD = indexC + 1;
      int indexA2 = indexD + 1;
      int indexB2 = indexA2 + 1;

      vertices.Add(cornerA);
      vertices.Add(cornerB);
      vertices.Add(cornerC);
      vertices.Add(cornerD);
      vertices.Add(cornerA);
      vertices.Add(cornerB);

      indices.Add(indexA);
      indices.Add(indexC);
      indices.Add(indexB);

      indices.Add(indexB2);
      indices.Add(indexD);
      indices.Add(indexA2);
    }

    /// <summary>
    /// Subdivides a list of triangles with simple half-edge points.
    /// https://graphics.stanford.edu/~mdfisher/subdivision.html
    /// </summary>
    /// <param name="count">Number of times to subdivide all triangles.</param>
    /// <param name="vertices">Vertex array to subdivide.</param>
    /// <param name="indices">Indices of the triangles to subdivide. Corresponds to the supplied vertex array.</param>
    public static void SubdivideTriangles(int count, ref List<Vector3> vertices, ref List<int> indices)
    {
      if (count < 1)
        return;

      int initIndices = indices.Count;
      for(int i = 0; i < initIndices; i += 3)
      {
        int i1 = indices[i];
        int i2 = indices[i + 1];
        int i3 = indices[i + 2];

        Vector3 v1 = vertices[i1];
        Vector3 v2 = vertices[i2];
        Vector3 v3 = vertices[i3];

        // v[w][s] is the vertex halfway between w and s 
        Vector3 v12 = (v2 - v1) / 2 + v1;
        Vector3 v13 = (v3 - v1) / 2 + v1;
        Vector3 v23 = (v3 - v2) / 2 + v2;

        // Create the first triangle inplace
        // Vertex at i1 stays the same
        vertices[i2] = v12;
        vertices[i3] = v13;

        indices.Add(vertices.Count);
        vertices.Add(v2);
        indices.Add(vertices.Count);
        vertices.Add(v23);
        indices.Add(vertices.Count);
        vertices.Add(v12);

        indices.Add(vertices.Count);
        vertices.Add(v3);
        indices.Add(vertices.Count);
        vertices.Add(v13);
        indices.Add(vertices.Count);
        vertices.Add(v23);

        indices.Add(vertices.Count);
        vertices.Add(v12);
        indices.Add(vertices.Count);
        vertices.Add(v23);
        indices.Add(vertices.Count);
        vertices.Add(v13);
      }

      SubdivideTriangles(count - 1, ref vertices, ref indices);
    }

    /// <summary>
    /// Uses perlin noise to perturb the vertices in the mesh.
    /// </summary>
    /// <param name="intensity">Max amount to shift vertices by.</param>
    /// <param name="density">How much one world unit moves through the perlin noise function.</param>
    /// <param name="octaves">How many levels of detail are added to the mesh.</param>
    /// <param name="vertices">Vertices to shift.</param>
    public static void AddMeshNoise(float intensity, float density, int octaves,
      ref List<Vector3> vertices)
    {
      for(int i = 0; i < vertices.Count; i++)
      {
        Vector3 vert = vertices[i];

        float noise = 0;

        for(int octave = 0; octave < octaves; octave++)
        {
          float lacunarity = Mathf.Pow(2.0f, octave);
          float persistance = Mathf.Pow(0.5f, octave);
          noise += persistance * Mathf.PerlinNoise((vert.x + vert.y - vert.z) * density * lacunarity,
              (vert.x - vert.y + vert.z) * density * lacunarity);
        }

        Vector3 direction = new Vector3(
          Mathf.PerlinNoise(-vert.x + vert.y - vert.z, vert.x - vert.y + vert.z),
          Mathf.PerlinNoise(vert.x - vert.y + vert.z, -vert.x + vert.y - vert.z),
          Mathf.PerlinNoise(-vert.x + vert.y - vert.z, -vert.x - vert.y - vert.z));
        vertices[i] += direction.normalized * intensity * noise;
      }
    }

    /// <summary>
    /// Calculates normals for an array of vertices, assuming a 1:1 relationship
    /// between vertices and indices.
    /// </summary>
    /// <param name="vertices">Array of vertices corresponding to the indices.</param>
    /// <param name="indices">Triangle list corresponding to vertices.</param>
    /// <param name="normals">
    /// Output for created normals. List will be reinitialized and filled with
    /// vertices.Count number of normals.
    /// </param>
    public static void CalculateFlatNormals(in List<Vector3> vertices,
      in List<int> indices, out List<Vector3> normals)
    {
      normals = new List<Vector3>();
      for (int i = 0; i < indices.Count; i += 3)
      {
        Vector3 v1 = vertices[indices[i]] - vertices[indices[i + 1]];
        Vector3 v2 = vertices[indices[i]] - vertices[indices[i + 2]];

        Vector3 normal = Vector3.Cross(v1.normalized, v2.normalized);

        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);
      }
    }
  }
}

