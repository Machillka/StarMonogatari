using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private int _gameSecond;
    private int _gameMinute;
    private int _gameHour;
    private int _gameDay;
    private int _gameMonth;
    private int _gameYear;

    private Seasons _gameSeason = Seasons.Spring;

    private int _monthInSeason = 3;
    public bool IsGameClockPause;

    private float _tikTime;

    private void Update()
    {
        if (!IsGameClockPause)
        {
            _tikTime += Time.deltaTime;
            if (_tikTime >= Settings.secondThreshold)
            {
                _tikTime -= Settings.secondThreshold;
                UpdateGameTime();
            }
        }

        if (InputManager.Instance.IsShiftTimeButtonPressing)
        {
            // Debug.Log("Second" + _gameSecond + "Minutes:" + _gameMinute + "Hours" + _gameHour);
            // Debug.Log("Pressed");
            for (int i = 0; i < 360; i++)
            {
                UpdateGameTime();
            }
            // Debug.Log("Second" + _gameSecond + "Minutes:" + _gameMinute + "Hours" + _gameHour);
        }
    }

    private void Awake()
    {
        InitNewGameTime();
    }

    private void InitNewGameTime()
    {
        _gameSecond = 0;
        _gameMinute = 0;
        _gameHour = 7;
        _gameDay = 1;
        _gameMonth = 1;
        _gameSeason = Seasons.Spring;
        _gameYear = 2025;
    }

    private void UpdateGameTime()
    {
        //TODO:思考如何增加可读性和可维护性
        _gameSecond++;
        if (_gameSecond > Settings.secondHold)
        {
            _gameMinute++;
            _gameSecond = 0;
            if (_gameMinute > Settings.minuteHold)
            {
                _gameHour++;
                _gameMinute = 0;

                if (_gameHour > Settings.hourHold)
                {
                    _gameDay++;
                    _gameHour = 0;

                    if (_gameDay > Settings.dayHold)            // 经过一个月
                    {
                        _gameMonth++;
                        _gameDay = 1;

                        if (_gameMonth > Settings.monthHold)
                        {
                            _gameMonth = 1;
                        }

                        _monthInSeason--;
                        if (_monthInSeason == 0)
                        {
                            _monthInSeason = 3;

                            int seasonNumber = (int)_gameSeason;
                            seasonNumber++;

                            if (seasonNumber > Settings.seasonHold)
                            {
                                seasonNumber = 0;
                                _gameYear++;
                            }

                            _gameSeason = (Seasons)seasonNumber;

                            if (_gameYear > 9999)
                            {
                                Debug.Log("神仙");
                                _gameYear = 1;
                            }
                        }

                    }
                    // 每天刷新农作物和地图
                    EventHandler.CallGameDayChangeEvent(_gameDay, _gameSeason);
                }
                EventHandler.CallDataChangeEvent(_gameHour, _gameDay, _gameMonth, _gameYear, _gameSeason);
            }
            EventHandler.CallGameMinuteChangeEvent(_gameMinute, _gameHour);
        }
        // Debug.Log("Second" + _gameSecond + "Minnuts:" + _gameMinute);
    }
}
