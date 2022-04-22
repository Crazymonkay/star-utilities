using UnityEngine;

namespace RoadTurtleGames.StarUtilities
{
    public class StarComponent : MonoBehaviour
    {
        public StarProperties StarProperties;
        
        private MaterialPropertyBlock materialPropertyBlock;
        private Renderer renderer;

        private void Awake()
        {
            materialPropertyBlock = new MaterialPropertyBlock();
            renderer = GetComponent<Renderer>();
        }

        private void Update()
        {
            float temp = StarUtilities.GetNormalizedTemperature(StarProperties.Temperature);
            float luminosity = StarUtilities.GetNormalizedEstimatedSolarLuminosityExponent(StarProperties.Temperature, StarProperties.Radius);

            materialPropertyBlock.SetFloat("_StarTemperature", temp);
            materialPropertyBlock.SetFloat("_StarLuminosity", luminosity);

            renderer.SetPropertyBlock(materialPropertyBlock);
        }
    }
}
