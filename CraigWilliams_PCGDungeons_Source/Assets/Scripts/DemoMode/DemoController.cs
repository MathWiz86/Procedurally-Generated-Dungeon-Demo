/**************************************************************************************************/
/*!
\file   DemoController.cs
\author Craig Williams
\par    Last Updated
        2021-04-09

\brief
  A file containing implementation of a demo player controller, used to look around the generated
  dungeon.

\par Bug List

\par References
*/
/**************************************************************************************************/

using PCGDungeon.UnityEditor;
using UnityEngine;

namespace PCGDungeon
{
  /************************************************************************************************/
  /// <summary>
  /// A player controller for looking around the mesh of the randomly generated dungeon.
  /// </summary>
  [RequireComponent(typeof(Rigidbody))] [RequireComponent(typeof(Collider))]
  public class DemoController : MonoBehaviour
  {
    /// <summary>The name of the horizontal <see cref="Input"/> axis.</summary>
    [Header("Input Properties")]
    [SerializeField] private string HorizontalAxis = "Horizontal";
    /// <summary>The name of the vertical <see cref="Input"/> axis.</summary>
    [SerializeField] private string VerticalAxis = "Vertical";
    /// <summary>The name of the mouse's X <see cref="Input"/> axis.</summary>
    [SerializeField] private string MouseXAxis = "Mouse X";
    /// <summary>The name of the mouse's Y <see cref="Input"/> axis.</summary>
    [SerializeField] private string MouseYAxis = "Mouse Y";

    /// <summary>The speed at which the controller moves around.</summary>
    [Header("Movement Properties")]
    [Space(20.0f)]
    [SerializeField] private float moveSpeed = 30.0f;

    /// <summary>The <see cref="Camera"/> of the controller.</summary>
    [Header("Camera Properties")]
    [Space(20.0f)]
    [SerializeField] private Camera controllerCamera;
    /// <summary>The sensitivity of the mouse when turning around.</summary>
    [SerializeField] private float turnSensitivity = 100.0f;
    /// <summary>The clamp for the angles the camera can reach vertically.</summary>
    [SerializeField] [ValueRange(-90.0f, 90.0f)] private Vector2 cameraClamp = new Vector2(-80.0f, 80.0f);

    /// <summary>The transform of the <see cref="controllerCamera"/>.</summary>
    private Transform cameraPivot;
    /// <summary>The <see cref="AudioListener"/> of the <see cref="controllerCamera"/>.</summary>
    private AudioListener cameraListener;
    /// <summary>The controller's <see cref="Rigidbody"/>.</summary>
    private Rigidbody comRigidbody;

    /// <summary>The inputs made for movement.</summary>
    private Vector2 movementInput;
    /// <summary>The inputs made for turning</summary>
    private Vector2 turningInput;
    /// <summary>The current rotation of the <see cref="controllerCamera"/>.</summary>
    private float currentCameraRotation = 0.0f;

    private void Awake()
    {
      // Get the necessary components off of the camera and controller.
      cameraPivot = controllerCamera.transform;
      cameraListener = controllerCamera.GetComponent<AudioListener>();
      comRigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
      // When this object is enabled, the camera and listener are enabled.
      controllerCamera.enabled = true;
      cameraListener.enabled = true;
      Cursor.lockState = CursorLockMode.Locked;
      currentCameraRotation = 0.0f;
    }

    private void OnDisable()
    {
      // When this object is disabled, the camera and listener are disabled.
      controllerCamera.enabled = false;
      cameraListener.enabled = false;
    }

    private void Update()
    {
      Cursor.lockState = CursorLockMode.None;
      GetInputs(); // Get the inputs in the update loop.
    }

    private void FixedUpdate()
    {
      // Handle the physics in the fixed update loop.
      HandleRotation();
      HandleMovement();
    }

    /// <summary>
    /// A function for getting the user's inputs, for use in the physics update.
    /// </summary>
    private void GetInputs()
    {
      // Get the inputs for moving around.
      movementInput = new Vector2(Input.GetAxisRaw(HorizontalAxis), Input.GetAxisRaw(VerticalAxis));

      // Get the inputs for turning. Since these are not raw inputs, we scale by delta time.
      turningInput = new Vector2(Input.GetAxis(MouseXAxis) * turnSensitivity * Time.deltaTime, Input.GetAxis(MouseYAxis) * turnSensitivity * Time.deltaTime);
    }

    /// <summary>
    /// A function for handling the rotation of the controller and the camera.
    /// </summary>
    private void HandleRotation()
    {
      transform.Rotate(Vector3.up * turningInput.x); // Rotate the controller based on the x axis.

      // Clamp the rotation of the camera and set it as the camera's rotation.
      currentCameraRotation = Mathf.Clamp(currentCameraRotation - turningInput.y, cameraClamp.x, cameraClamp.y);
      cameraPivot.localRotation = Quaternion.Euler(currentCameraRotation, 0.0f, 0.0f);
    }

    /// <summary>
    /// A function for handling moving the controller.
    /// </summary>
    private void HandleMovement()
    {
      // Create a movement vector based on the inputs and the direction the controller faces.
      Vector3 movement = (transform.right * movementInput.x + transform.forward * movementInput.y).normalized * moveSpeed;
      movement.y = comRigidbody.velocity.y; // Set the y velocity to be the same.
      comRigidbody.velocity = movement; // Update the Rigidbody's velocity.
    }
  }
  /************************************************************************************************/
}