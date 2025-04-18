using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class TimeUIController : MonoBehaviour
{
    [SerializeField] private RectTransform _dayAndNight;
    [SerializeField] private RectTransform _clockHolder;
    [SerializeField] private Image _seasonImage;
    [SerializeField] private TextMeshProUGUI _dateText;
    [SerializeField] private TextMeshProUGUI _timeText;

    public Sprite[] SeasonSprites;

    private List<GameObject> _clockBlocks = new List<GameObject>();

    void Awake()
    {
        for (int i = 0; i < _clockHolder.childCount; i++)
        {
            _clockBlocks.Add(_clockHolder.GetChild(i).gameObject);
            _clockHolder.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        EventHandler.GameMinuteEvent += OnMinuteChangeEvent;
        EventHandler.GameDateEvent += OnDateChangeEvent;
    }

    private void OnDisable()
    {
        EventHandler.GameMinuteEvent -= OnMinuteChangeEvent;
        EventHandler.GameDateEvent -= OnDateChangeEvent;
    }

    private void SwitchHourImage(int hour)
    {
        int index = hour / 4;

        // 重置色块
        if (index == 0)
        {
            foreach (var block in _clockBlocks)
            {
                block.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < _clockBlocks.Count; i++)
            {
                if (i < index + 1)
                {
                    _clockBlocks[i].SetActive(true);
                }
                else
                {
                    _clockBlocks[i].SetActive(false);
                }
            }
        }
    }

    private void DayNightImageRotate(int hour)
    {
        var target = new Vector3(0, 0, hour * 15 - 90);
        _dayAndNight.DORotate(target, 1f, RotateMode.Fast);
    }

    public void OnDateChangeEvent(int hour, int day, int month, int year, Seasons season)
    {
        _dateText.text = year.ToString("0000") + "年" + month.ToString("00") + "月" + day.ToString("00") + "日";
        _seasonImage.sprite = SeasonSprites[(int)season];

        SwitchHourImage(hour);
        DayNightImageRotate(hour);
    }

    public void OnMinuteChangeEvent(int minute, int hour)
    {
        _timeText.text = hour.ToString("00") + ":" + hour.ToString("00");
    }
}
