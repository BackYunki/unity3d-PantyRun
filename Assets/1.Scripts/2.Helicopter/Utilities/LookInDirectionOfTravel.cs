using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeliSimPack.Utilities
{
  public class LookInDirectionOfTravel : MonoBehaviour
  {

    [SerializeField]
    Rigidbody rigidBody;

    [SerializeField]
    Transform rootTransform;

    [SerializeField]
    float minimumXLocalRotation;

    [SerializeField]
    float maximumXLocalRotation;

    [SerializeField]
    float minimumYLocalRotation;

    [SerializeField]
    float maximumYLocalRotation;

    [SerializeField]
    float damping;

    bool isClamped;
    float previousVelcity;

    // Use this for initialization
    void Start()
    {
      isClamped = true;
      previousVelcity = 0;
    }

    private void Update()
    {
      transform.LookAt(rootTransform.position + rootTransform.forward * 100);
      transform.eulerAngles = new Vector3(10, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    // Update is called once per frame
    void fffUpdate()
    {
      if (!isClamped
        && rigidBody.velocity.magnitude <= 0.5f && previousVelcity > 0.5f)
      {
        isClamped = true;
      }
      else if (isClamped
        && rigidBody.velocity.magnitude > 1 && previousVelcity <= 1)
      {
        isClamped = false;
      }

      float oldXrot = angleTo180(transform.localEulerAngles.x);
      float oldYrot = angleTo180(transform.localEulerAngles.y);

      //Vector3 relativeVelocity = transform.InverseTransformDirection(rigidBody.velocity);

      //Vector3 localDirection = new Vector3(Mathf.Abs(relativeVelocity.x) > 1 ? relativeVelocity.x : 0,
      //                                     Mathf.Abs(relativeVelocity.y) > 1 ? relativeVelocity.y : 0,
      //                                     Mathf.Abs(relativeVelocity.z) > 1 ? relativeVelocity.z : 1);

      Vector3 globalDirection = /*isClamped ? transform.TransformDirection(new Vector3(0, 0, 1)) :*/ rigidBody.velocity;// transform.TransformDirection(localDirection);

      // look 100 meters ahead
      transform.LookAt(transform.position + globalDirection.normalized * 100.0f);

      float desiredXrot = angleTo180(transform.localEulerAngles.x);
      float desiredYrot = angleTo180(transform.localEulerAngles.y);

      if (desiredXrot < minimumXLocalRotation)
      {
        desiredXrot = minimumXLocalRotation;
      }
      else if (desiredXrot > maximumXLocalRotation)
      {
        desiredXrot = maximumXLocalRotation;
      }

      if (desiredYrot < minimumYLocalRotation)
      {
        desiredYrot = minimumYLocalRotation;
      }
      else if (desiredYrot > maximumYLocalRotation)
      {
        desiredYrot = maximumYLocalRotation;
      }

      float actualXrot = Mathf.Lerp(oldXrot, desiredXrot, damping * Time.deltaTime);
      float actualYrot = Mathf.Lerp(oldYrot, desiredYrot, damping * Time.deltaTime);

      transform.localEulerAngles = new Vector3(actualXrot, actualYrot, 0);

      previousVelcity = rigidBody.velocity.magnitude;
    }

    float angleTo180(float iAngle)
    {
      float angle = iAngle;
      while (angle > 180) { angle -= 360; }
      while (angle <= -180) { angle += 360; }
      return angle;
    }
  }
}