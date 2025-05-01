using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Farm.Astar;
using System;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class NPCMovementController : MonoBehaviour
{
    public ScheduleDataListSO schduleData;
    private SortedSet<ScheduleDataListSO> _scheduleSet;
    private ScheduleDetails _currentSchedule;

    public string _currentSceneName;
    private string _targetSceneName;
    private Vector3Int _currentGridPosition;
    private Vector3Int _targetGridPosition;
    private Vector3Int _nextGridPosition;
    private Vector3 _nextWorldPositon;

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

    private bool _isNPCMove;
    private bool _isSceneLoaded;

    private TimeSpan GameTime => TimeManager.Instance.GameTime;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _coll = GetComponent<BoxCollider2D>();
        _anim = GetComponent<Animator>();
        _movementSteps = new Stack<MovementStep>();
    }

    #region enable and disable
    private void OnEnable()
    {
        EventHandler.BeforeSceneLoadedEvent += OnBeforeSceneLoadedEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneLoadedEvent -= OnBeforeSceneLoadedEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }
    #endregion


    private void FixedUpdate()
    {
        if (_isSceneLoaded)
            Movement();
    }

    private void OnBeforeSceneLoadedEvent()
    {
        _isSceneLoaded = true;
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
        _isSceneLoaded = true;
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

    private void Movement()
    {
        if (!_isNPCMove)
        {
            if (_movementSteps.Count > 0)
            {
                MovementStep step = _movementSteps.Pop();

                _currentSceneName = step.sceneName;

                CheckVisiable();

                _nextGridPosition = (Vector3Int)step.gridCoordinate;

                TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);

                MoveToGridPosition(_nextGridPosition, stepTime);
            }
        }
    }

    private void MoveToGridPosition(Vector3Int gridPos, TimeSpan stepTime)
    {
        StartCoroutine(MoveRoutine(gridPos, stepTime));
    }

    private IEnumerator MoveRoutine(Vector3Int gridPos, TimeSpan stepTime)
    {
        _isNPCMove = true;
        _nextWorldPositon = GetWorldPosition(gridPos);
        if (stepTime > GameTime)
        {
            // 用于移动的时间
            float timeToMove = (float)(stepTime.TotalSeconds - GameTime.TotalSeconds);
            float distance = Vector3.Distance(transform.position, _nextWorldPositon);

            float speed = Mathf.Max(_minSpeed, distance / timeToMove / Settings.secondThreshold);
            if (speed <= _maxSpeed)
            {
                while (Vector3.Distance(transform.position, _nextWorldPositon) > Settings.pixelSize)
                {
                    _direction = (_nextWorldPositon - transform.position).normalized;
                    Vector2 posOffset = new Vector2(_direction.x * speed * Time.fixedDeltaTime, _direction.y * speed * Time.fixedDeltaTime);
                    _rb.MovePosition(_rb.position + posOffset);
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        _rb.position = _nextWorldPositon;
        _currentGridPosition = gridPos;
        _nextGridPosition = _currentGridPosition;
        _isNPCMove = false;
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

    public void BuildPath(ScheduleDetails schedule)
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

    /// <summary>
    /// 通过网格坐标得到在世界的坐标
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    private Vector3 GetWorldPosition(Vector3Int gridPos)
    {
        Vector3 worldPos = _grid.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize / 2f, worldPos.y + Settings.gridCellSize / 2f);
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
