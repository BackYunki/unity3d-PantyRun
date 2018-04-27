using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeliSimPack.HelicopterSimulation
{
  [RequireComponent(typeof(RotorRotator))]
  [RequireComponent(typeof(EngineController))]
  public class SoundController : MonoBehaviour
  {

    AudioSource engineSound;
    AudioSource rotorSound;

    RotorRotator rotorRotator;
    EngineController engineController;

    float heardEngineSpeed = 0;

    // Use this for initialization
    void Start()
    {
      if (null != GetComponentInChildren<ObjectIdentifiers.EngineSound>())
      {
        engineSound = GetComponentInChildren<ObjectIdentifiers.EngineSound>().GetComponent<AudioSource>();
      }

      if (null != GetComponentInChildren<ObjectIdentifiers.RotorSound>())
      {
        rotorSound = GetComponentInChildren<ObjectIdentifiers.RotorSound>().GetComponent<AudioSource>();
      }

      rotorRotator = GetComponent<RotorRotator>();
      engineController = GetComponent<EngineController>();

      if (null != rotorSound)
      {
        rotorSound.volume = 0;
        rotorSound.Play();
      }

      if (null != engineSound)
      {
        engineSound.volume = 0;
        engineSound.Play();
      }
    }

    void FixedUpdate()
    {
      if (null != rotorSound)
      {
        float rotorPitch = rotorRotator.getRotorSpeed() / 100.0f;
        float rotorVolume = Mathf.Max(0, rotorRotator.getRotorSpeed() / 100.0f - 0.4f) * (1.0f / (1.0f - 0.4f));

        rotorSound.pitch = rotorPitch;
        rotorSound.volume = rotorVolume;
      }

      if (null != engineSound)
      {
        float realEngineSpeed = engineController.getEngineSpeed();
        bool increasingSpeed = realEngineSpeed > heardEngineSpeed;

        if (increasingSpeed)
        {
          heardEngineSpeed = Mathf.Lerp(heardEngineSpeed, realEngineSpeed < 0.1f ? 0 : realEngineSpeed, 1.0f * Time.fixedDeltaTime);
        }
        else
        {
          heardEngineSpeed = Mathf.Lerp(heardEngineSpeed, realEngineSpeed < 0.1f ? 0 : realEngineSpeed, 0.2f * Time.fixedDeltaTime);
        }

        float enginePitch = 0.5f + heardEngineSpeed / 100.0f / 2;
        float engineVolume = heardEngineSpeed / 100.0f;

        engineSound.pitch = enginePitch;
        engineSound.volume = engineVolume;
      }
    }
  }
}