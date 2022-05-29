using UnityEngine;
using UnityEngine.UI;

public class Balls : MonoBehaviour {

    public static Balls instance { get; set; }

    public float rateTimeBetweenBalls;
    public float speedBalls;
    public LayerMask layer;
    public int numberBalls;
    public Rigidbody2D ball;
    public Text tnArrivedBallsBack;

    private float time;
    private bool isTouchPosInGameArea;
    private Vector2 posSecondVector;
    private Vector2 targetPosition;
    private Vector2 direction;
    private Vector2 secondDirection;
    private Vector2 target;
    private Vector2 eye;
    private bool check;
    private LineRenderer line;

    [HideInInspector] public Vector2 sidesDir;
    [HideInInspector] public Vector2 topDir;
    [HideInInspector] public Vector2 newDir;
    [HideInInspector] public bool firstBallTouchedBottom;
    [HideInInspector] public int nArrivedBallsBack;
    [HideInInspector] public int ballNumberThrown;
    [HideInInspector] public bool isFirstBallDestoyed;
    [HideInInspector] public float angle;
    [HideInInspector] public bool startThrowBalls;
    [HideInInspector] public bool levelConstructionDown;
    [HideInInspector] public bool newThrowBalls;

    Rigidbody2D clone;
    RaycastHit2D hit;
    Ray2D ray;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        startThrowBalls = false;
        newThrowBalls = true;

        nArrivedBallsBack = 1;
        ballNumberThrown = 0;

        tnArrivedBallsBack.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(new Vector2(GameObject.Find("firstBall").transform.position.x + 0.36f,GameObject.Find("firstBall").transform.position.y + 0.19f));
        tnArrivedBallsBack.enabled = true;
        tnArrivedBallsBack.text = "x" + numberBalls;

        line = GameObject.Find("TrajectoryLine").GetComponent<LineRenderer>(); //Line which we will use for drawing trajectory of balls
    }
    void FixedUpdate ()
    {
#if UNITY_EDITOR
        posSecondVector = Input.mousePosition;
#endif

#if UNITY_IPHONE || UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Vector2 touchPos = Input.GetTouch(0).position;
            posSecondVector = touchPos;
        }
#endif
        if (startThrowBalls == false)
        {
            if ((Screen.height / 5.5f < posSecondVector.y) && ((Screen.height - (Screen.height / 5.5f)) > posSecondVector.y))
                isTouchPosInGameArea = true;
            else
            {
                isTouchPosInGameArea = false;
                line.enabled = false;
            }
        }

        if (LevelManager.instance.gatherBalls == false && isTouchPosInGameArea)
        {
            if (time < Time.time && ballNumberThrown < numberBalls && startThrowBalls && newThrowBalls)
            {
                BallsInstantiate();

                time = rateTimeBetweenBalls + Time.time;
                ballNumberThrown++;
                tnArrivedBallsBack.enabled = false;

                line.enabled = false;
            }
            if (nArrivedBallsBack >= numberBalls)
            {
                ballNumberThrown = 0;
                nArrivedBallsBack = 1;
                numberBalls++;

                firstBallTouchedBottom = false;
                newThrowBalls = true;
                startThrowBalls = false;

                LevelManager.instance.gatherBalls = false;
                LevelManager.instance.gatherButton.SetActive(false);
                LevelManager.instance.twoButtons.SetActive(true);
                levelConstructionDown = false;

                if (GameObject.Find("firstBall") != null)
                    Destroy(GameObject.Find("firstBall").GetComponent<CircleCollider2D>());

                if (!levelConstructionDown)
                {
                    LevelManager.instance.DownLevelConstruction();
                    levelConstructionDown = true;
                }
            }
#if UNITY_EDITOR
            if (Input.GetMouseButton(0) && ballNumberThrown == 0 && newThrowBalls && startThrowBalls == false)
            {
                line.enabled = true;
                direction = Direction(Camera.main.ScreenToWorldPoint(posSecondVector));
                secondDirection = newDir;
                Trajectory();
            }
            if (Input.GetMouseButtonUp(0))
            {
                startThrowBalls = true;
                LevelManager.instance.gatherButton.SetActive(true);
                LevelManager.instance.twoButtons.SetActive(false);
            }
#endif

#if UNITY_IPHONE || UNITY_ANDROID
            if (Input.touchCount > 0 && ballNumberThrown == 0 && newThrowBalls && startThrowBalls == false)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Moved:
                        line.enabled = true;
                        direction = Direction(Camera.main.ScreenToWorldPoint(posSecondVector));
                        secondDirection = newDir;
                        Trajectory();
                        break;
                    case TouchPhase.Ended:
                        startThrowBalls = true;
                        LevelManager.instance.gatherButton.SetActive(true);
                        LevelManager.instance.twoButtons.SetActive(false);
                        break;
                }
            }
#endif

            if (ballNumberThrown >= numberBalls && firstBallTouchedBottom == false)
            {
                tnArrivedBallsBack.enabled = false;
                levelConstructionDown = false;
            }
            if(ballNumberThrown >= numberBalls && firstBallTouchedBottom)
                Destroy(GameObject.Find("previousFirstBall"));
        }
    }
    private void BallsInstantiate()
    {
        if(GameObject.Find("previousFirstBall") == null)
           clone = Instantiate(ball,GameObject.Find("firstBall").transform.position,Quaternion.identity);
        else
           clone = Instantiate(ball, GameObject.Find("previousFirstBall").transform.position, Quaternion.identity);

        clone.velocity = new Vector2(direction.normalized.x * speedBalls, direction.normalized.y * speedBalls);
    }
    private void Trajectory()
    {
        hit = Physics2D.Raycast(GameObject.Find("firstBall").transform.position, Direction(Camera.main.ScreenToWorldPoint(posSecondVector)),100f,layer);

        line.SetPosition(0, GameObject.Find("firstBall").transform.position);
        line.SetPosition(1,hit.point);

        Dir();

        line.SetPosition(2, ray.GetPoint(1.5f));
    }
    public Vector2 Direction(Vector2 targetPos)
    {
        eye = GameObject.Find("firstBall").transform.position;
        target = targetPos;

        direction = new Vector2(target.x - eye.x, target.y - eye.y);

        return direction;
    }
    private void Dir()
    {
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (hit.collider.gameObject.name == "border_top")
            newDir = new Vector2(Mathf.Sin((angle + 90) * Mathf.Deg2Rad), Mathf.Cos((angle + 90) * Mathf.Deg2Rad));
        else
            newDir = new Vector2(-Mathf.Sin((angle + 90) * Mathf.Deg2Rad), -Mathf.Cos((angle + 90) * Mathf.Deg2Rad));

        sidesDir = new Vector2(-Mathf.Sin((angle + 90) * Mathf.Deg2Rad), -Mathf.Cos((angle + 90) * Mathf.Deg2Rad));
        topDir = new Vector2(Mathf.Sin((angle + 90) * Mathf.Deg2Rad), Mathf.Cos((angle + 90) * Mathf.Deg2Rad));

        ray = new Ray2D(hit.point, newDir);
    }
}
