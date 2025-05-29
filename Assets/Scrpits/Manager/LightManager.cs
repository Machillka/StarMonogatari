using UnityEngine;

public class LightManager : MonoBehaviour
{
    private LightController[] sceneLights;
    private LightShifts currentLightShift;
    private Seasons currentSeason;
    private float timeDifferent;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.LightShiftChangeEvent += OnLightShiftChangeEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.LightShiftChangeEvent -= OnLightShiftChangeEvent;
    }

    private void OnLightShiftChangeEvent(Seasons season, LightShifts lightShift, float timeDiff)
    {
        currentSeason = season;
        timeDifferent = timeDiff;

        if (currentLightShift != lightShift)
        {
            currentLightShift = lightShift;

            foreach (var light in sceneLights)
            {
                light.LightChangeShift(currentSeason, currentLightShift, timeDifferent);
            }
        }
    }

    private void OnAfterSceneLoadedEvent()
    {
        sceneLights = FindObjectsByType<LightController>(FindObjectsSortMode.None);

        foreach (var light in sceneLights)
        {
            light.LightChangeShift(currentSeason, currentLightShift, timeDifferent);
        }
    }
}
