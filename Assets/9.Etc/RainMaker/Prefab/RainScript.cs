//
// Rain Maker (c) 2015 Digital Ruby, LLC
// http://www.digitalruby.com
//

using UnityEngine;
using System.Collections;

namespace DigitalRuby.RainMaker
{
    public class RainScript : BaseRainScript
    {
        [Tooltip("The height above the camera that the rain will start falling from")]
        public float RainHeight = 25.0f;

        [Tooltip("How far the rain particle system is ahead of the player")]
        public float RainForwardOffset = -7.0f;

        private void UpdateRain()
        {
            // keep rain and mist above the player
            if (RainFallParticleSystem != null)
            {
                    var s = RainFallParticleSystem.shape;
                    s.shapeType = ParticleSystemShapeType.Box;
                    Vector3 pos = RainFallParticleSystem.transform.position;
                    pos.y -= RainHeight;
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            UpdateRain();
        }
    }
}