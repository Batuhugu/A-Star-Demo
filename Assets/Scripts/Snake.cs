using Pathfinding;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{
    public enum MovementStyle
    {
        AIPath,
        AILerp,
    }

    private Vector2 direction = Vector2.right;
    private List<SnakeSegment> segments = new List<SnakeSegment>();

    [SerializeField] SnakeSegment segmentPrefab;
    [SerializeField] int initialSize = 4;
    [SerializeField] GameObject segmentsContainer;
    [SerializeField] Sprite head_alive;
    [SerializeField] Sprite head_dead;
    [SerializeField] Image defaultSegmentColor;
    [Header("A*")]
    [SerializeField] MovementStyle movementStyle;
    [SerializeField] AIPath aiPath;
    [SerializeField] AILerp aiLerp;
    [SerializeField] AIDestinationSetter aiDestinationSetter;
    [SerializeField] GraphUpdateScene graphUpdate;

    private SnakeSegment head;
    private Vector2 _swipeStartPos;
    private Vector2 input;

    public float speed = 20f;
    public float speedMultiplier = 1f;

    private float nextUpdate;

    private void Awake()
    {
        //AstarPath.active.Scan();
        head = GetComponent<SnakeSegment>();
        segmentPrefab.GetComponent<SpriteRenderer>().color = defaultSegmentColor.color;

        if (head == null)
        {
            head = gameObject.AddComponent<SnakeSegment>();
            head.hideFlags = HideFlags.HideInInspector;
        }

        switch (movementStyle)
        {
            case MovementStyle.AIPath:
                aiPath.canMove = false;
                break;
            case MovementStyle.AILerp:
                aiLerp.canMove = false;
                break;
        }
    }

    private void Start()
    {
        aiDestinationSetter.target = GameObject.FindGameObjectWithTag("Food").transform;
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameWorking)
        {
            GetDirectionKeyboard();
            GetDirectionSwipe();
        }
    }

    private void GetDirectionKeyboard()
    {
        // Only allow turning up or down while moving in the x-axis
        if (head.direction.x != 0f)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                head.SetDirection(Vector2.up, Vector2.zero);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                head.SetDirection(Vector2.down, Vector2.zero);
            }
        }
        // Only allow turning left or right while moving in the y-axis
        else if (head.direction.y != 0f)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                head.SetDirection(Vector2.right, Vector2.zero);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                head.SetDirection(Vector2.left, Vector2.zero);
            }
        }
    }

    private void GetDirectionSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _swipeStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                Vector2 swipeEndPos = touch.position;
                Vector2 swipeDir = swipeEndPos - _swipeStartPos;
                swipeDir.Normalize();
                if (Mathf.Abs(swipeDir.x) > Mathf.Abs(swipeDir.y))
                {
                    // Horizontal swipe
                    if (swipeDir.x > 0 && direction != Vector2.left)
                    {
                        direction = Vector2.right;
                    }
                    else if (swipeDir.x < 0 && direction != Vector2.right)
                    {
                        direction = Vector2.left;
                    }
                }
                else
                {
                    // Vertical swipe
                    if (swipeDir.y > 0 && direction != Vector2.down)
                    {
                        direction = Vector2.up;
                    }
                    else if (swipeDir.y < 0 && direction != Vector2.up)
                    {
                        direction = Vector2.down;
                    }
                }
                head.SetDirection(direction, Vector2.zero);
            }
        }
    }

    private void FixedUpdate()
    {
        // Wait until the next update before proceeding
        if (Time.time < nextUpdate)
        {
            return;
        }

        // Set the new direction based on the input
        if (input != Vector2.zero)
        {
            direction = input;
        }

        if (GameManager.Instance.IsGameWorking)
        {
            for (int i = segments.Count - 1; i > 0; i--)
            {
                segments[i].Follow(segments[i - 1], i, segments.Count);
            }
            //HandleBoundaryTeleport();
            switch (movementStyle)
            {
                case MovementStyle.AIPath:
                    aiPath.canMove = true;
                    break;
                case MovementStyle.AILerp:
                    aiLerp.canMove = true;
                    break;
            }
        }
    }


    private void HandleBoundaryTeleport()
    {
        Rect safeArea = Screen.safeArea;
        // Get the current screen bounds
        float screenLeft = Mathf.Round(Camera.main.ScreenToWorldPoint(new Vector3(safeArea.xMin, 0, 0)).x);
        float screenRight = Mathf.Round(Camera.main.ScreenToWorldPoint(new Vector3(safeArea.xMax, 0, 0)).x);
        float screenBottom = Mathf.Round(Camera.main.ScreenToWorldPoint(new Vector3(0, safeArea.yMin, 0)).y);
        float screenTop = Mathf.Round(Camera.main.ScreenToWorldPoint(new Vector3(0, safeArea.yMax, 0)).y);

        // Check if the snake goes beyond the screen bounds
        if (this.transform.position.x > screenRight)
        {
            this.transform.position = new Vector3(screenLeft, this.transform.position.y, 0.0f);
        }
        else if (this.transform.position.x < screenLeft)
        {
            this.transform.position = new Vector3(screenRight, this.transform.position.y, 0.0f);
        }
        else if (this.transform.position.y > screenTop)
        {
            this.transform.position = new Vector3(this.transform.position.x, screenBottom, 0.0f);
        }
        else if (this.transform.position.y < screenBottom)
        {
            this.transform.position = new Vector3(this.transform.position.x, screenTop, 0.0f);
        }
        else
        {
            // Move the snake in the direction it is facing
            // Round the values to ensure it aligns to the grid
            float x = Mathf.Round(head.transform.position.x) + head.direction.x;
            float y = Mathf.Round(head.transform.position.y) + head.direction.y;

            head.transform.position = new Vector2(x, y);
            nextUpdate = Time.time + (1f / (speed * speedMultiplier));
        }
    }

    private void Grow()
    {
        SnakeSegment segment = Instantiate(segmentPrefab, segmentsContainer.transform);
        segment.Follow(segments[segments.Count - 1], segments.Count, segments.Count + 1);
        segments.Add(segment);
    }

    public void ResetState(bool isMenubutton)
    {
        ChangeHead(isMenubutton);
        if (!isMenubutton)
        {
            AudioManager.Instance.UpdateMusicPitch(true, false);
        }
        GameManager.Instance.score = 0;
        // Set the initial direction of the snake, starting at the origin
        // (center of the grid)
        head.SetDirection(Vector2.right, Vector2.zero);
        head.transform.position = Vector3.zero;

        // Start at 1 to skip destroying the head
        for (int i = 1; i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }

        // Clear the list then add the head as the first segment
        segments.Clear();
        segments.Add(head);
        if (!isMenubutton)
        {
            // -1 since the head is already in the list
            for (int i = 0; i < initialSize - 1; i++)
            {

                Grow();
            }
        }
    }

    private void ChangeHead(bool isMenubutton)
    {
        if (GameManager.Instance.IsGameWorking || isMenubutton)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = head_alive;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = head_dead;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
        {
            Grow();
            aiDestinationSetter.target = GameObject.FindGameObjectWithTag("Food").transform;
            AudioManager.Instance.PlaySFX(ClipType.Eat);
            GameManager.Instance.score++;
            GameManager.Instance.UpdateScore();
            //AstarPath.active.Scan();
            //graphUpdate.GetComponent<GraphUpdateScene>().Apply();

            //var graphToScan = AstarPath.active.data.gridGraph;
            //AstarPath.active.Scan(graphToScan);
        }
        else if (other.tag == "Obstacle")
        {
            aiPath.canMove = false;
            aiLerp.canMove = false; 
            GameManager.Instance.GameOver();
            ChangeHead(false);
        }
    }
}



