using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeliSimPack.Utilities
{
  public class smoothFollow : MonoBehaviour
  {

    // The target we are following
    public Transform target;
    Rigidbody rb;
    // The distance in the x-z plane to the target
    public float distance = 10.0f;
    // the height we want the camera to be above the target
    public float height = 5.0f;
    public float correctedHeightDamping;
    // How much we 
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;

    public LayerMask collisionLayers = -1;     // What the camera will collide with

    private Camera cam;
    private float defaultFieldOfView;

    //private float distanceOfCollision;

    float correctedHeight;
    float correctedHeightTarget;

    // Use this for initialization
    void Start()
    {
      cam = GetComponent<Camera>();
      defaultFieldOfView = cam.fieldOfView;
      rb = target.GetComponent<Rigidbody>();

      correctedHeight = height;
      correctedHeightTarget = height;
    }

    void Update()
    {
      if (null == rb)
      {
        rb = target.GetComponent<Rigidbody>();
      }

      CamUpdate();
    }

    void CamUpdate()
    {
      // Early out if we don't have a target
      if (!target || !cam.enabled || null == rb)
        return;

      if (rb.velocity.y < -5)
      {
        correctedHeightTarget = 2.5f * height;
      }
      else if (rb.velocity.y < 0)
      {
        correctedHeightTarget = -1.5f / 5f * height * rb.velocity.y + height;
      }
      else if (rb.velocity.y < 5)
      {
        correctedHeightTarget = -2.5f / 5f * height * rb.velocity.y + height;
      }
      else
      {
        correctedHeightTarget = -1.5f * height;
      }

      correctedHeight = Mathf.Lerp(correctedHeight, height, correctedHeightDamping * Time.deltaTime);

      float wantedRotationAngleY = target.eulerAngles.y;
      float wantedHeight = target.position.y + correctedHeight;
      float currentRotationAngleY = transform.eulerAngles.y;
      float currentHeight = transform.position.y;

      float currentFielOfView = cam.fieldOfView;

      // Damp the height and rotation around the y-axis
      currentHeight = Mathf.LerpAngle(currentHeight, wantedHeight, rotationDamping * Time.deltaTime);
      currentRotationAngleY = Mathf.LerpAngle(currentRotationAngleY, wantedRotationAngleY, rotationDamping * Time.deltaTime);
      currentFielOfView = Mathf.LerpAngle(currentFielOfView, defaultFieldOfView, rotationDamping * Time.deltaTime);

      // Convert the angle into a rotation
      Quaternion wantedRotation = Quaternion.Euler(0, wantedRotationAngleY, 0);

      // Set the position of the camera on the x-z plane to:
      // distance meters behind the target
      Vector3 desiredPosition = target.position;
      desiredPosition -= wantedRotation * Vector3.forward * distance;
      desiredPosition = new Vector3(desiredPosition.x, wantedHeight, desiredPosition.z);


      RaycastHit hit;

      // cast the Collision ray behind target to test for collisions
      if (Physics.Linecast(target.position, desiredPosition, out hit, collisionLayers))
      //if (Physics.Raycast(desiredPosition, target.position - desiredPosition, out hit, distanceOfCollision) && hit.transform.tag != "Player")
      {
        desiredPosition = hit.point;
      }

      // Set the height of the camera
      transform.position = desiredPosition;

      // Always look at the target
      transform.LookAt(target);

      cam.fieldOfView = currentFielOfView;
    }
  }
}