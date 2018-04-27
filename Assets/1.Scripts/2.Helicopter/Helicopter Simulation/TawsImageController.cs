using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HeliSimPack.HelicopterSimulation
{
  [RequireComponent(typeof(Rigidbody))]
  public class TawsImageController : MonoBehaviour
  {
    Image renderTarget; // the UI image used for displaying TAWS image

    Texture2D generatedImage; // the texture generated

    [SerializeField]
    [Tooltip("Is TAWS enabled at start")]
    bool isEnabledAtStart = true;

    [SerializeField]
    [Tooltip("Color representing the highest terrain on TAWS image")]
    Color highDensityRed = new Color(0.9294f, 0.1176f, 0.1412f, 1.0f);
    [SerializeField]
    [Tooltip("Color representing the second highest terrain on TAWS image")]
    Color highDensityYellow = new Color(0.9490f, 0.4353f, 0.0549f, 1.0f);
    [SerializeField]
    [Tooltip("Color representing the third highest terrain on TAWS image")]
    Color lowDensityYellow = new Color(0.7216f, 0.7020f, 0.0f, 1.0f);
    [SerializeField]
    [Tooltip("Color representing the fourth highest terrain on TAWS image")]
    Color highDensityGreen = new Color(0.4039f, 0.6471f, 0.1804f, 1.0f);
    [SerializeField]
    [Tooltip("Color representing the fifth highest terrain on TAWS image")]
    Color lowDensityGreen = new Color(0.1294f, 0.2078f, 0.1725f, 1.0f);
    [SerializeField]
    [Tooltip("Color used to show water or terrain with altitude of 0 if property \"Is Water Shown Blue\" is true")]
    Color waterBlue = new Color(0.0f, 0.0f, 0.7412f, 1.0f);

    [SerializeField]
    [Tooltip("Relative altitude in feet from which terrain will be shown in \"High Density Red\" color")]
    float highDensityRedLowerBoundary = 250;
    [SerializeField]
    [Tooltip("Relative altitude in feet from which terrain will be shown in \"High Density Yellow\" color")]
    float highDensityYellowLowerBoundary = 0;
    [SerializeField]
    [Tooltip("Relative altitude in feet from which terrain will be shown in \"Low Density Yellow\" color")]
    float lowDensityYellowLowerBoundary = -250;
    [SerializeField]
    [Tooltip("Relative altitude in feet from which terrain will be shown in \"High Density Green\" color")]
    float highDensityGreenLowerBoundary = -500;
    [SerializeField]
    [Tooltip("Relative altitude in feet from which terrain will be shown in \"Low Density Green\" color")]
    float lowDensityGreenLowerBoundary = -1000;
    [SerializeField]
    [Tooltip("If true, water and terrain at altitude 0 will be shown in \"Water Blue\" color")]
    bool isWaterShownBlue = true;

    [SerializeField]
    [Tooltip("Time it takes in seconds to refresh the whole image")]
    float refreshPeriod = 2.0f;
    [SerializeField]
    [Tooltip("Height of the image in pixels. Lower number will show bigger pixels")]
    int radiusInPixels = 20;
    [SerializeField]
    [Tooltip("How blurry will the image be")]
    FilterMode filterMode = FilterMode.Point;

    [SerializeField]
    [Tooltip("Set this property to highest the terrain could be (in feet). For performance keep this number as low as possible")]
    float maximumTerrainHeight = 5000; // to minimise lenght of raycast

    [SerializeField]
    [Tooltip("Layers composing the terrain. Only these layers will be used to generate the TAWS image.")]
    LayerMask terrainLayers = ~0;

    float lastAzimuthUpdated;
    float azimuthToUpdate;
    float degreesPerSecond; // angular speed of radial to update

    Rigidbody helicopterRb;

    float visibleRange;

    bool isTawsEnabled = false;
    bool firstPass = true;
    bool initializationDone = false;

    void Start()
    {
      helicopterRb = GetComponent<Rigidbody>();

      // find target
      ObjectIdentifiers.TawsRenderTarget target = FindObjectOfType<ObjectIdentifiers.TawsRenderTarget>();
      if (null != target)
      {
        renderTarget = target.GetComponent<Image>();
      }

      // create generated image
      generatedImage = new Texture2D(2 * radiusInPixels, radiusInPixels);
      generatedImage.filterMode = filterMode;

      // assign generated image to target
      if (null != renderTarget)
      {
        renderTarget.sprite = Sprite.Create(generatedImage, new Rect(0, 0, 2 * radiusInPixels, radiusInPixels), new Vector2(0.5f, 0.0f));
      }

      lastAzimuthUpdated = 0;
      azimuthToUpdate = 0;
      degreesPerSecond = 90.0f / refreshPeriod;

      clearImage();

      initializationDone = true;
    }

    void Update()
    {
      if (firstPass)
      {
        // Wait 1 seconds before starting taws otherwise first couple of radial are incomplete.
        if (Time.time > 1)
        {
          isTawsEnabled = isEnabledAtStart;
          firstPass = false;
        }
      }
      else if (isTawsEnabled)
      {
        lastAzimuthUpdated = azimuthToUpdate;
        updateAzimuth(azimuthToUpdate);

        // azimuth to update goes from 0 to 90 degrees and starts back at 0
        azimuthToUpdate = lastAzimuthUpdated + degreesPerSecond * Time.deltaTime;
        if (azimuthToUpdate > 90)
        {
          azimuthToUpdate = 0;
        }
      }
    }

    void updateAzimuth(float iAzimuth)
    {
      int xPixel = 0;
      int yPixel = 0;
      float angle = iAzimuth;

      // find each pixels on the radial we are updating
      for (int i = 0; i < radiusInPixels; ++i)
      {
        if (angle < 45)
        {
          yPixel = i;
          xPixel = (int)((float)(yPixel) * Mathf.Tan(iAzimuth * Mathf.Deg2Rad) + 0.5f);
        }
        else if (!Mathf.Approximately(angle, 90))
        {
          xPixel = i;
          yPixel = (int)((float)(xPixel) / Mathf.Tan(iAzimuth * Mathf.Deg2Rad) + 0.5f);
        }
        else
        {
          xPixel = i;
          yPixel = 0;
        }


        if (radiusInPixels + xPixel < 2 * radiusInPixels
          && yPixel < radiusInPixels
          && new Vector2(xPixel, yPixel).magnitude <= radiusInPixels + 1)
        {
          // update both sides at the time
          updatePixel(radiusInPixels + xPixel, yPixel, iAzimuth);
          updatePixel(radiusInPixels - xPixel, yPixel, -iAzimuth);
        }
      }

      // apply modification to texture
      generatedImage.Apply();
    }

    void updatePixel(int x, int y, float iAzimuth)
    {
      float _x = (float)(x - radiusInPixels);
      float _y = (float)(y);
      float range = Mathf.Sqrt(_x * _x + _y * _y) / radiusInPixels * visibleRange * 1852; // meters
      float heading = helicopterRb.rotation.eulerAngles.y;
      float bearing = iAzimuth + heading;

      // find world position corresponding to the pixel we update
      float worldX = helicopterRb.position.x + range * Mathf.Sin(bearing * Mathf.Deg2Rad);
      float worldZ = helicopterRb.position.z + range * Mathf.Cos(bearing * Mathf.Deg2Rad);

      // start raycast at calculated position and at the maximum terrain height (to minimize the lenght of raycasts)
      Vector3 raycastStartPos = new Vector3(worldX, maximumTerrainHeight / 3.28084f, worldZ);

      float terrainHeight = 0;
      RaycastHit hit;
      if (Physics.Raycast(raycastStartPos, Vector3.down, out hit, maximumTerrainHeight + 100, terrainLayers))
      {
        // get terrain height
        terrainHeight = maximumTerrainHeight - hit.distance * 3.28084f;
      }

      // get relative terrain height
      float terrainRelativeToOwnship = terrainHeight - helicopterRb.position.y * 3.28084f;

      Color pixelColor = Color.black;

      // process pixel color accoring to relative height
      if (Mathf.Abs(terrainHeight) < 5
        && isWaterShownBlue)
      {
        pixelColor = waterBlue;
      }
      else if (terrainRelativeToOwnship >= highDensityRedLowerBoundary)
      {
        pixelColor = highDensityRed;
      }
      else if (terrainRelativeToOwnship >= highDensityYellowLowerBoundary)
      {
        pixelColor = highDensityYellow;
      }
      else if (terrainRelativeToOwnship >= lowDensityYellowLowerBoundary)
      {
        pixelColor = lowDensityYellow;
      }
      else if (terrainRelativeToOwnship >= highDensityGreenLowerBoundary)
      {
        pixelColor = highDensityGreen;
      }
      else if (terrainRelativeToOwnship >= lowDensityGreenLowerBoundary)
      {
        pixelColor = lowDensityGreen;
      }

      // apply pixel color
      generatedImage.SetPixel(x, y, pixelColor);

    }

    void clearImage()
    {
      for (int i = 0; i < 2 * radiusInPixels; ++i)
      {
        for (int j = 0; j < radiusInPixels; ++j)
        {
          generatedImage.SetPixel(i, j, Color.black);
        }
      }
      generatedImage.Apply();
    }

    // stops rendering of taws image and resets parameters
    public void disableTaws()
    {
      isTawsEnabled = false;
      lastAzimuthUpdated = 90;
      azimuthToUpdate = 0;
      clearImage();
    }

    // starts rendering of taws image
    public void enableTaws()
    {
      isTawsEnabled = true;
    }

    public void changeRange(float iRange)
    {
      if (initializationDone)
      {
        // clear image when range is changed
        clearImage();
        azimuthToUpdate = 0;
      }
      visibleRange = iRange;
    }

    public float getAzimuth()
    {
      return azimuthToUpdate;
    }

    public bool isTawsOn()
    {
      return isTawsEnabled;
    }
  }
}