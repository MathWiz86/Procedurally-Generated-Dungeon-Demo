/**************************************************************************************************/
/*!
\file   DemoCamera.cs
\author Craig Williams
\par    Last Updated
        2021-04-09

\brief
  A file containing implementation of a demo camera, whichi is used as an overhead view of the
  entire dungeon.

\par Bug List

\par References
*/
/**************************************************************************************************/

using System.Collections;
using UnityEngine;

namespace PCGDungeon
{
  /************************************************************************************************/
  /// <summary>
  /// A camera controller used when showing the generated dungeon from above.
  /// </summary>
  [RequireComponent(typeof(Camera))] [RequireComponent(typeof(AudioListener))]
  public class DemoCamera : MonoBehaviour
  {
    /// <summary>The name of the horizontal <see cref="Input"/> axis.</summary>
    [SerializeField] private string HorizontalAxis = "Horizontal";
    /// <summary>The name of the vertical <see cref="Input"/> axis.</summary>
    [SerializeField] private string VerticalAxis = "Vertical";
    /// <summary>The name of the zooming <see cref="Input"/> axis.</summary>
    [SerializeField] private string ZoomAxis = "ZoomDemoCamera";

    /// <summary>The world Y distance of the camera.</summary>
    [SerializeField] [Min(1)] private float CameraDistance = 10.0f;
    /// <summary>The minimum size the camera can reach while zooming.</summary>
    [SerializeField] [Min(1)] private float MinSize = 1;
    /// <summary>The speed of the camera's movement.</summary>
    [SerializeField] [Range(2.0f, 10.0f)] private float MoveSpeed = 5.0f;
    /// <summary>The speed of the camera's zooming.</summary>
    [SerializeField] [Range(1.0f, 5.0f)] private float ZoomSpeed = 2.0f;

    /// <summary>The maxmimum size the camera can reach while zooming.</summary>
    private float MaxSize = 1;
    /// <summary>The camera's <see cref="Camera"/> component.</summary>
    private Camera comCamera;
    /// <summary>The camera's <see cref="AudioListener"/> component.</summary>
    private AudioListener comListener;
    /// <summary>The current size of the dungeon, from the <see cref="DungeonManager"/>.</summary>
    private Vector2Int currentDungeonSize;

    private void Awake()
    {
      // Get the necessary components.
      comCamera = GetComponent<Camera>();
      comListener = GetComponent<AudioListener>();
    }

    private void OnEnable()
    {
      // When this object is enabled, the camera and listener are enabled.
      comCamera.enabled = true;
      comListener.enabled = true;
    }

    private void OnDisable()
    {
      // When this object is disabled, the camera and listener are disabled.
      comCamera.enabled = false;
      comListener.enabled = false;
    }

    private void FixedUpdate()
    {
      MoveCamera(); // Move the camera around.
      ZoomCamera(); // Zoom the camera in and out.
    }

    /// <summary>
    /// A function for setting the <see cref="DemoCamera"/> to a proper size and position
    /// for the generated dungeon.
    /// </summary>
    public void ReadyDemoCamera()
    {
      currentDungeonSize = DungeonManager.GetDungeonSize(); // Get the dungeon size.

      // Set the position to the middle of the dungeon.
      transform.position = new Vector3(currentDungeonSize.x / 2.0f, CameraDistance, currentDungeonSize.y / 2.0f);

      // Set the orthographic size. The max orthographic size is the max axis.
      MaxSize = System.Math.Max(currentDungeonSize.x, currentDungeonSize.y) / 2.0f;
      comCamera.orthographicSize = MaxSize;
    }

    /// <summary>
    /// A function for handling the camera's movement.
    /// </summary>
    private void MoveCamera()
    {
      // Get the horizontal and vertical inputs. Flip the horizontal due to the perspective.
      float horizontal = -Input.GetAxisRaw(HorizontalAxis);
      float vertical = Input.GetAxisRaw(VerticalAxis);

      // Create a movement vector from the inputs and move the camera in world space.
      Vector3 movement = new Vector3(vertical, 0, horizontal).normalized * Time.deltaTime * MoveSpeed;
      transform.Translate(movement, Space.World);

      // Clamp the position based on the overall size of hte dungeon.
      float finalX = Mathf.Clamp(transform.position.x, 0, currentDungeonSize.y);
      float finalZ = Mathf.Clamp(transform.position.z, 0, currentDungeonSize.x);

      // Update the final position, keeping the same camera distance vertically.
      transform.position = new Vector3(finalX, CameraDistance, finalZ);
    }

    /// <summary>
    /// A funciton for handling the camera zooming.
    /// </summary>
    private void ZoomCamera()
    {
      float zoom = Input.GetAxisRaw(ZoomAxis); // Get the zoom direction.
      zoom *= Time.deltaTime * ZoomSpeed; // Update the zoom based on speed and time.

      // Update the camera's size, clamped to the min and max size.
      comCamera.orthographicSize = Mathf.Clamp(comCamera.orthographicSize - zoom, MinSize, MaxSize);
    }
  }
  /************************************************************************************************/
}