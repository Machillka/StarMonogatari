using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class LightController : MonoBehaviour
{
    public LightDataListSO lightPattenDataset;

    private Light2D currentLight;
    private LightDetails currentLightPatten;

    private void Awake()
    {
        currentLight = GetComponent<Light2D>();
    }

    public void LightChangeShift(Seasons season, LightShifts lightShift, float timeDiff)
    {
        currentLightPatten = lightPattenDataset.GetLightDetails(season, lightShift);

        if (timeDiff < Settings.lightChangeDuration)
        {
            Color colorOffsets = (currentLight.color - currentLightPatten.color) / Settings.lightChangeDuration * timeDiff;

            currentLight.color += colorOffsets;

            DOTween.To(() => currentLight.color, c => currentLight.color = c, currentLightPatten.color, Settings.lightChangeDuration - timeDiff);
            DOTween.To(() => currentLight.intensity, i => currentLight.intensity = i, currentLightPatten.lightAmount, Settings.lightChangeDuration - timeDiff);
        }

        if (timeDiff >= Settings.lightChangeDuration)
        {
            currentLight.color = currentLightPatten.color;
            currentLight.intensity = currentLightPatten.lightAmount;
        }
    }
}
