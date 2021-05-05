/**************************************************************************************************/
/*!
\file   MeshGeneration.cs
\author Braxton DeHate
\par    Last Updated
        2021-04-03

\brief
  Contains the rrefine responsible for creating appropriate meshes for a dungeon.

\par Bug List

\par references
*/
/**************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCGDungeon
{
  /// <summary>
  /// Manages generation of meshes for the dungeon.
  /// </summary>
  public static class MeshGeneration
  {
    /// <summary>
    /// World height of hallway meshes.
    /// </summary>
    private static float hallHeight = 1.0f;
    /// <summary>
    /// World height of room meshes.
    /// </summary>
    private static float roomHeight = 1.0f;
    /// <summary>
    /// World size of each tile on x and z.
    /// </summary>
    private static float tileSize = 1.0f;
    /// <summary>
    /// Intensity or amplitude of noise added to world mesh.
    /// </summary>
    private static float noiseIntensity = 0.3f;
    /// <summary>
    /// Density or "wavelength" of the noise added to the world mesh.
    /// </summary>
    private static float noiseDensity = 1.0f;
    /// <summary>
    /// Layers of noise added to the world mesh. Higher means more detail, only apparent
    /// with high enough <see cref="meshSubdivisions"/>.
    /// </summary>
    private static int noiseOctaves = 3;
    /// <summary>
    /// Base wall geometry is one quad per tile wall, then each triangle is subdivided
    /// this many times.
    /// </summary>
    private static int meshSubdivisions = 3;
    /// <summary>
    /// Material to be applied to the mesh renderer of the world.
    /// </summary>
    private static Material worldMat = Resources.Load<Material>("worldMat");

    /// <summary>
    /// Helper structure containing lists for each mesh attribute.
    /// </summary>
    private struct MeshData
    {
      public List<Vector3> vertices;
      public List<int> indices;
      public List<Vector3> normals;
    }

    /// <summary>
    /// Adds mesh components and creates the mesh start to finish for the dungeon.
    /// </summary>
    /// <param name="dungeonManager">
    /// Reference to a dungeonManager object with constructed children representing tiles.
    /// </param>
    public static void GenerateMesh(GameObject dungeonManager)
    {
      // Create renderer component if it does not already exist.
      MeshRenderer renderer = dungeonManager.GetComponent<MeshRenderer>();
      if (!renderer)
      {
        renderer = dungeonManager.gameObject.AddComponent<MeshRenderer>();
      }

      renderer.material = worldMat;

      // Create MeshFilter if it does not already exist.
      MeshFilter meshFilter = dungeonManager.GetComponent<MeshFilter>();
      if (!meshFilter)
        meshFilter = dungeonManager.gameObject.AddComponent<MeshFilter>();

      // Create new mesh and set the index format to 32 bit so we can have >65k triangles.
      meshFilter.mesh = new Mesh();
      meshFilter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

      // Create MeshCollider if it does not already exist
      MeshCollider meshCollider = dungeonManager.GetComponent<MeshCollider>();
      if (!meshCollider)
        meshCollider = dungeonManager.gameObject.AddComponent<MeshCollider>();

      // Initialize MeshData structure
      MeshData data = new MeshData();
      data.vertices = new List<Vector3>();
      data.indices = new List<int>();
      data.normals = new List<Vector3>();

      GenerateMeshForChildren(dungeonManager.transform, data);
      Geometry.SubdivideTriangles(meshSubdivisions, ref data.vertices, ref data.indices);
      Geometry.AddMeshNoise(noiseIntensity, noiseDensity, noiseOctaves, ref data.vertices);
      Geometry.CalculateFlatNormals(in data.vertices, in data.indices, out data.normals);

      meshFilter.mesh.SetVertices(data.vertices);
      meshFilter.mesh.SetTriangles(data.indices, 0);
      meshFilter.mesh.SetNormals(data.normals);

      Color32[] colors = new Color32[data.vertices.Count];
      for (int i = 0; i < data.vertices.Count; i++)
      {
        colors[i] = Color.grey;
      }

      meshFilter.mesh.colors32 = colors;

      meshCollider.sharedMesh = meshFilter.mesh;
    }

    /// <summary>
    /// Recursively adds mesh data corresponding to each child of the specified transform
    /// that contains a DungeonTile component.
    /// </summary>
    /// <param name="root">Transform to check all children of.</param>
    /// <param name="data">An initialized MeshData structure to output into.</param>
    private static void GenerateMeshForChildren(Transform root, MeshData data)
    {
      foreach (Transform child in root.transform)
      {
        DungeonRoom room = child.GetComponent<DungeonRoom>();

        if (room)
        {
          GenerateMeshForRoom(room, data);
        }
        else
        {
          DungeonHallTile hallTile = child.GetComponent<DungeonHallTile>();
          if (hallTile)
          {
            GenerateMeshForHallTile(hallTile, data);
          }
          else
          {
            GenerateMeshForChildren(child, data);
          }
        }
      }
    }

    /// <summary>
    /// Adds mesh data for all children of a dungeon room object containing a 
    /// DungeonRoomTile.
    /// </summary>
    /// <param name="room">A DungeonRoom component on an object parent to DungeonRoomTiles</param>
    /// <param name="data">An initialized MeshData structure to output into.</param>
    private static void GenerateMeshForRoom(DungeonRoom room, MeshData data)
    {
      foreach (Transform child in room.transform)
      {
        DungeonRoomTile roomTile = child.GetComponent<DungeonRoomTile>();

        if (roomTile)
          GenerateMeshForRoomTile(roomTile, data);
      }
    }

    /// <summary>
    /// Adds mesh data for a room tile.
    /// </summary>
    /// <param name="tile">DungeonRoomTile to create mesh data based on.</param>
    /// <param name="data">An initialized MeshData structure to output into.</param>
    private static void GenerateMeshForRoomTile(DungeonRoomTile tile, MeshData data)
    {
      GenerateMeshForTile(tile.TileWalls, roomHeight, data, tile.transform.position);
    }

    /// <summary>
    /// Adds mesh data for a hall tile.
    /// </summary>
    /// <param name="tile">DungeonHallTile to create mesh data based on.</param>
    /// <param name="data">An initialized MeshData structure to output into.</param>
    private static void GenerateMeshForHallTile(DungeonHallTile tile, MeshData data)
    {
      GenerateMeshForTile(tile.TileWalls, hallHeight, data, tile.transform.position);
    }

    /// <summary>
    /// Adds mesh data for a tile.
    /// </summary>
    /// <param name="walls">DungeonWall array to take data from.</param>
    /// <param name="height">Height to use with the tile</param>
    /// <param name="data">An initialized MeshData structure to output into.</param>
    /// <param name="root">Root position of the tile. Mesh data is offset by this value.</param>
    private static void GenerateMeshForTile(DungeonWall[] walls, float height, MeshData data,
      Vector3 root)
    {
      // Floor
      Geometry.CreateFloorQuad(new Vector3(tileSize, 0.0f, tileSize) + root,
          new Vector3(0.0f, 0.0f, 0.0f) + root, ref data.vertices, ref data.indices);

      // Ceiling
      Geometry.CreateFloorQuad(new Vector3(tileSize, height, 0.0f) + root,
          new Vector3(0.0f, height, tileSize) + root, ref data.vertices, ref data.indices);

      // -Z
      if (!walls[0].IsEmpty && walls[0].WallType == WallBasicType.BasicWall)
      {
        Geometry.CreateWallQuad(new Vector3(tileSize, 0.0f, 0.0f) + root,
            new Vector3(0.0f, height, 0.0f) + root, ref data.vertices, ref data.indices);
      }

      // +X
      if (!walls[1].IsEmpty && walls[1].WallType == WallBasicType.BasicWall)
      {
        Geometry.CreateWallQuad(new Vector3(tileSize, 0.0f, tileSize) + root,
            new Vector3(tileSize, height, 0.0f) + root, ref data.vertices, ref data.indices);
      }

      // +Z
      if (!walls[2].IsEmpty && walls[2].WallType == WallBasicType.BasicWall)
      {
        Geometry.CreateWallQuad(new Vector3(0.0f, 0.0f, tileSize) + root,
            new Vector3(tileSize, height, tileSize) + root, ref data.vertices, ref data.indices);
      }

      // -X
      if (!walls[3].IsEmpty && walls[3].WallType == WallBasicType.BasicWall)
      {
        Geometry.CreateWallQuad(new Vector3(0.0f, 0.0f, 0.0f) + root,
            new Vector3(0.0f, height, tileSize) + root, ref data.vertices, ref data.indices);
      }
    }
  }
}
