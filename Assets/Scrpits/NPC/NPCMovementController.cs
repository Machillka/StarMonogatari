using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Farm.Astar;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class NPCMovementController : MonoBehaviour
{
    public ScheduleDataListSO schduleData;
    private SortedSet<ScheduleDataListSO> _scheduleSet;
    private ScheduleDetails _currentSchedule;

    private string _currentSceneName;
    private string _targetSceneName;
    private Vector3Int _currentGridPosition;
    private Vector3Int _targetGridPosition;

    public string StartScene { set => _currentSceneName = value; }

    [Header("Movement")]
    public float normalSpeed = 2f;
    private float _minSpeed = 1f;
    private float _maxSpeed = 3f;
    private Vector2 _direction;
    private Grid _grid;
    public bool isMoveing;

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _coll;
    private Animator _anim;

    private Stack<MovementStep> _movementSteps;

    private bool _isInitialized;

    private TimeSpan GameTime => TimeManager.Instance.GameTime;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _coll = GetComponent<BoxCollider2D>();
        _anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        _grid = FindAnyObjectByType<Grid>();
        CheckVisiable();

        if (!_isInitialized)
        {
            InitNPC();
            _isInitialized = true;
        }
    }

    private void CheckVisiable()
    {
        if (_currentSceneName == SceneManager.GetActiveScene().name)
        {
            SetActiveInScene();
        }
        else
        {
            SetInactiveInScene();
        }
    }

    private void InitNPC()
    {
        _targetSceneName = _currentSceneName;

        // 保持在格子中心
        _currentGridPosition = _grid.WorldToCell(transform.position);
        transform.position = new Vector3(
            _currentGridPosition.x + Settings.gridCellSize / 2f,
            _currentGridPosition.y + Settings.gridCellSize / 2f,
            0
        );
    }

    private void BuildPath(ScheduleDetails schedule)
    {
        _movementSteps.Clear();
        _currentSchedule = schedule;

        if (schedule.targetScene == _currentSceneName)
        {
            AStar.Instance.BuildPath(schedule.targetScene, (Vector2Int)_currentGridPosition, schedule.targetGridPosition, _movementSteps);
        }

        if (_movementSteps.Count > 1)
        {
            UpdateTimeOnPath();
        }
    }

    private void UpdateTimeOnPath()
    {
        MovementStep previousStep = null;
        TimeSpan currentGameTime = GameTime;

        foreach (MovementStep step in _movementSteps)
        {
            if (previousStep == null)
            {
                previousStep = step;
            }

            step.hour = currentGameTime.Hours;
            step.minute = currentGameTime.Minutes;
            step.second = currentGameTime.Seconds;

            TimeSpan gridMovementStepTime;

            if (IsMovementInDiagonal(step, previousStep))
            {
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / normalSpeed / Settings.secondThreshold));
            }
            else
            {
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.secondThreshold));
            }

            currentGameTime = currentGameTime.Add(gridMovementStepTime);
            previousStep = step;
        }
    }

    /// <summary>
    /// 判断是否走斜方向
    /// </summary>
    /// <param name="currentStep"></param>
    /// <param name="previousStep"></param>
    /// <returns></returns>
    private bool IsMovementInDiagonal(MovementStep currentStep, MovementStep previousStep)
    {
        return (currentStep.gridCoordinate.x != previousStep.gridCoordinate.x) && (currentStep.gridCoordinate.y != previousStep.gridCoordinate.y);
    }

    #region 设置显示状态
    private void SetActiveInScene()
    {
        _spriteRenderer.enabled = true;
        _coll.enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void SetInactiveInScene()
    {
        _spriteRenderer.enabled = false;
        _coll.enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }
    #endregion
}
