using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class SliderHandle : MonoBehaviour
{
    [SerializeField] private GameObject Slider;
    [SerializeField] private GameObject HitArea;
    [SerializeField] private GameObject StartPosition;
    [SerializeField] private GameObject MidpointPosition;
    [SerializeField] private GameObject EndPosition;
    [SerializeField] private GameObject InputUIAnchor;
    [SerializeField] private float HitAreaOffset;
    [SerializeField] private float GoodColliderChangeOffset;
    [SerializeField] private float PerfectColliderChangeOffset;
    [SerializeField] private float SliderSpeedOffset;

    private RectTransform HitTransform;

    private BoxCollider2D SliderCollider;
    private BoxCollider2D GoodCollider;
    private BoxCollider2D PerfectCollider;
    private RectTransform PerfectArea;
    private RectTransform GoodArea;

    private float SliderSpeed = 100.0f;
    private int Rounds;
    private readonly int MaxRounds = 9;
    private int GoodClicks;
    private int PerfectClicks;
    private bool Arrived = false;
    private float _notStirringTime;
    private float _notStirringThreshold = 1000.5f;
    
    private Vector3 _originalSliderPosition;
    private Vector3 _originalStartPosition;
    private Vector3 _originalEndPosition;
    private Vector3 _originalGoodColliderSize;
    private Vector3 _originalPerfectColliderSize;
    private Vector2 _originalGoodAreaSize;
    private Vector2 _originalPerfectAreaSize;
    private Vector3 _originalHitAreaLocation;
    private float _originalSliderSpeed;

    private Queue<Direction> InputDirections;
    private Direction CurrentInputDirection;
    private Direction _previousDirection;

    private enum Click { None, Miss, Good, Perfect }
    
    private enum Direction { Neutral, Left, Right }

    [SerializeField] private AudioSource PerfectSource;
    [SerializeField] private AudioSource GoodSource;
    [SerializeField] private AudioSource MissSource;

    private bool isBaking;
    public bool IsBaking
    {
        get => isBaking;
        set => isBaking = value;
    }

    public GameObject InputUIPosition => InputUIAnchor;

    public void Start()
    {
        HitTransform = HitArea.GetComponent<RectTransform>();
        SliderCollider = Slider.GetComponent<BoxCollider2D>();
        GoodCollider = HitArea.GetComponent<BoxCollider2D>();
        PerfectCollider = HitArea.transform.GetChild(0).GetComponent<BoxCollider2D>();
        
        GoodArea = HitArea.GetComponent<RectTransform>();
        PerfectArea = HitArea.transform.GetChild(0).GetComponent<RectTransform>();
        
        InputDirections = new Queue<Direction>();
        InputDirections.Enqueue(Direction.Right);
        InputDirections.Enqueue(Direction.Left);
        CurrentInputDirection = InputDirections.Peek();
        _previousDirection = CurrentInputDirection;
        
        _originalSliderPosition = Slider.transform.localPosition;
        _originalStartPosition = StartPosition.transform.localPosition;
        _originalEndPosition = EndPosition.transform.localPosition;
        _originalGoodColliderSize = GoodCollider.size;
        _originalPerfectColliderSize = PerfectCollider.size;
        _originalGoodAreaSize = GoodArea.sizeDelta;
        _originalPerfectAreaSize = PerfectArea.sizeDelta;
        _originalHitAreaLocation = HitArea.transform.localPosition;
        _originalSliderSpeed = SliderSpeed;
    }

    private void Update()
    {
        if (!isBaking) return;
        
        Debug.Log($"Round {Rounds}");
        
        if(Rounds != MaxRounds)
        {
            if(!Arrived)
                Slider.transform.localPosition = Vector3.MoveTowards(Slider.transform.localPosition, EndPosition.transform.localPosition, Time.deltaTime * SliderSpeed); // L to R
            else
                Slider.transform.localPosition = Vector3.MoveTowards(Slider.transform.localPosition, StartPosition.transform.localPosition, Time.deltaTime * SliderSpeed); // R to L

            if(GetDirection(InputManager.InputDirection.x) == CurrentInputDirection)
            {
                _notStirringTime = 0f;
                GoodClicks++;
            }
            else if(GetDirection(InputManager.InputDirection.x) == Direction.Neutral)
            {
                _notStirringTime += Time.deltaTime;
            }

            if (_notStirringTime >= _notStirringThreshold)
            {
                ChangeHitAreaParameters();
                _notStirringTime = 0f;
            }

            CheckArrived();
            if(_previousDirection != GetDirection(InputManager.InputDirection.x))
                CheckSliderPosition();
        }
        else
        {
            Rounds = 0; // Will not call update again because it is stopped in Confection.cs
            GoodClicks = 0;
            PerfectClicks = 0;
        }

        _previousDirection = GetDirection(InputManager.InputDirection.x);
    }

    private void CheckSliderPosition()
    {
        if (GetDirection(InputManager.InputDirection.x) != Direction.Neutral 
                    && GetDirection(InputManager.InputDirection.x) != CurrentInputDirection)
        {
            if (SliderCollider.IsTouching(PerfectCollider))
            {
                PerfectClicks++;
                Arrived = !Arrived;
                Rounds++;
                PerfectSource.Play();
                ChangeHitAreaParameters();
                Debug.Log("Perfect Direction Change!");
            }
            else if (SliderCollider.IsTouching(GoodCollider))
            {
                GoodClicks++;
                Arrived = !Arrived;
                Rounds++;
                GoodSource.Play();
                ChangeHitAreaParameters();
                Debug.Log("Good Direction Change!");
            }
            else
            {
                Rounds++;
                ChangeHitAreaParameters();
                MissSource.Play();
                Debug.Log("Missed Direction Change");
            }
            
            Debug.Log($"Round {Rounds}");
        }
    }

    private Direction GetDirection(float inputDirectionX)
    {
        Direction direction = Direction.Neutral;
        
        if (inputDirectionX >= 0.3f)
            direction = Direction.Right;
        else if (inputDirectionX <= -0.3f)
            direction = Direction.Left;

        return direction;
    }

    private void CheckArrived()
    {
        if (Slider.transform.localPosition == EndPosition.transform.localPosition)
        {
            Arrived = true;
            Rounds++;
            // ChangeHitAreaParameters();
        }

        if (Slider.transform.localPosition == StartPosition.transform.localPosition)
        {
            Arrived = false;
            Rounds++;
            // ChangeHitAreaParameters();
        }
    }

    private void ChangeHitAreaParameters()
    {
        var start = EndPosition.transform.localPosition;
        var endPosition = new Vector3(start.x - HitAreaOffset, start.y, 0.0f);
        
        var end = StartPosition.transform.localPosition;
        var startPosition = new Vector3(end.x + HitAreaOffset, end.y, 0.0f);
        
        HitArea.transform.localPosition = InputDirections.Peek() == Direction.Left ? 
            new Vector3(EndPosition.transform.localPosition.x - Random.Range(45f, 60f), Slider.transform.localPosition.y, 0.0f) : new Vector3(StartPosition.transform.localPosition.x + Random.Range(45f, 60f), Slider.transform.localPosition.y, 0.0f);

        EndPosition.transform.localPosition = endPosition;
        StartPosition.transform.localPosition = startPosition;

        GoodCollider.size = new Vector2(GoodCollider.size.x - GoodColliderChangeOffset, GoodCollider.size.y - GoodColliderChangeOffset);
        PerfectCollider.size = new Vector2(PerfectCollider.size.x - PerfectColliderChangeOffset, PerfectCollider.size.y - PerfectColliderChangeOffset);

        GoodArea.sizeDelta = new Vector2(GoodArea.sizeDelta.x - GoodColliderChangeOffset, GoodArea.sizeDelta.y);
        PerfectArea.sizeDelta =
            new Vector2(PerfectArea.sizeDelta.x - PerfectColliderChangeOffset, PerfectArea.sizeDelta.y);

        PerfectArea.transform.position = MidpointPosition.transform.position;

        SliderSpeed += SliderSpeedOffset;
            
        var tmp = InputDirections.Dequeue();
        InputDirections.Enqueue(tmp);
        CurrentInputDirection = InputDirections.Peek();
        InputUIManager.Instance.SwitchKeyJoystickDirection(CurrentInputDirection == Direction.Right);
    }

    private void OnDisable()
    {
        ResetUI();
    }

    private void ResetUI()
    {
        InputDirections.Clear();
        InputDirections.Enqueue(Direction.Right);
        InputDirections.Enqueue(Direction.Left);
        CurrentInputDirection = InputDirections.Peek();

        HitArea.transform.localPosition = _originalHitAreaLocation;
        
        Slider.transform.localPosition = _originalSliderPosition;
        StartPosition.transform.localPosition = _originalStartPosition;
        EndPosition.transform.localPosition = _originalEndPosition;
        GoodCollider.size = _originalGoodColliderSize;
        PerfectCollider.size = _originalPerfectColliderSize;
        GoodArea.sizeDelta = _originalGoodAreaSize;
        PerfectArea.sizeDelta = _originalPerfectAreaSize;
        SliderSpeed = _originalSliderSpeed;
        
        _notStirringTime = 0f;
        Rounds = 0;
        PerfectClicks = 0;
        GoodClicks = 0;
        _notStirringTime = 0;
    }

    public int GetGoodClicks()
    {
        return GoodClicks;
    }

    public int GetPerfectClicks()
    {
        return PerfectClicks;
    }

    public int GetTotalClicks()
    {
        return Rounds;
    }

    public int GetMissedTime()
    {
        return (int) _notStirringTime;
    }

    public int GetMaxClicks()
    {
        return MaxRounds;
    }
}
