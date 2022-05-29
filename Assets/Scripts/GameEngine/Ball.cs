using UnityEngine;

public class Ball : MonoBehaviour {

    bool changeDir;
    Vector2 iDirection;
    float angle;
    string firstBorder;
    public bool ballsDown;

    bool collDetected;
    int nDir;
    Vector2 oppositeDir;
    Vector2 tDir;

    private bool ballThrow;
    private bool forceAdded;

    private bool firstTop;
    private float timer;

    bool collideFromLeft;
    bool collideFromTop;
    bool collideFromRight;
    bool collideFromBottom;

    bool isThisNewThrowPosition;
    bool isBallGathered;

    public bool gatherBalls;

    private void Update()
    {
        if(LevelManager.instance.gatherBalls && this.gameObject.name != "firstBall")
        {
            if (isBallGathered == false)
                GatherBallWhenButtonPressed();

            isBallGathered = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (LevelManager.instance.gatherBalls == false)
        {
            BallTowardToNextDirection(collision);
            AddForceToBall(collision);
            GatherBallToFirstBallTouchedBottom(collision);
        }
        if (collision.gameObject.name == "firstBall" && gameObject.name != "firstBall" && LevelManager.instance.gatherBalls)
        {
            LevelManager.instance.nGatherBalls++;

            Balls.instance.tnArrivedBallsBack.enabled = true;
            Balls.instance.tnArrivedBallsBack.text = "x" + LevelManager.instance.nGatherBalls;

        //Assign the position of firstBall to the text which count the number of ball which arrived back
            if (GameObject.Find("firstBall").transform.position.x < 1.4f)
            {
                Balls.instance.tnArrivedBallsBack.GetComponent<RectTransform>().position =
                      Camera.main.WorldToScreenPoint(new Vector2(GameObject.Find("firstBall").transform.position.x + 0.36f,
                      GameObject.Find("firstBall").transform.position.y + 0.19f));
            }
            else
            {
                Balls.instance.tnArrivedBallsBack.GetComponent<RectTransform>().position =
                     Camera.main.WorldToScreenPoint(new Vector2(GameObject.Find("firstBall").transform.position.x - 0.36f,
                     GameObject.Find("firstBall").transform.position.y + 0.19f));
            }
        //-------------------------------------------------------------------------------------

            Destroy(this.gameObject);
        }
    }
    private void BallTowardToNextDirection(Collider2D collision)
    {
        //print("step1");

        if (changeDir == false)
        {
            //print("step2");
            iDirection = new Vector2(-Balls.instance.sidesDir.x, Balls.instance.sidesDir.y);
            changeDir = true;
        }
        else
        {
            //print("step3");
            if (timer < Time.time && collision.gameObject.name != "ball(Clone)" && collision.gameObject.name != "bottom_pannel" && collision.gameObject.tag != "brick")
            {
                if (collision.gameObject.name == "left" || collision.gameObject.name == "right")
                    iDirection = new Vector2(-iDirection.x, iDirection.y);
                else if (collision.gameObject.name == "top" || collision.gameObject.name == "bottom")
                    iDirection = new Vector2(-(-iDirection.x), -iDirection.y);

                timer = Time.time + 0.0035f;
            }
            if (collision.gameObject.name == "border_left" || collision.gameObject.name == "border_right")
                iDirection = new Vector2(-iDirection.x, iDirection.y);
            else if (collision.gameObject.name == "border_top")
                iDirection = new Vector2(-(-iDirection.x), -iDirection.y);
        }
    }
    private void AddForceToBall(Collider2D collision)
    {
        if (collision.gameObject.name != "ball(Clone)" && collision.gameObject.name != "bottom_pannel" && collision.gameObject.tag != "brick")
        {
            ballThrow = true;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(iDirection.x * Balls.instance.speedBalls, iDirection.y * Balls.instance.speedBalls);
        }
    }
    private void GatherBallToFirstBallTouchedBottom(Collider2D collision)
    {
    //Decrease brick life with one everytime when a ball touch birck and also set color of brick in dependence of actual brick life
        if (collision.gameObject.tag == "brick")
        {
            LevelManager.instance.BallTouchedBrickSound();
            //print("dsfsdfsdfsdf");

            collision.gameObject.GetComponent<Brick>().brickLife--;
            collision.gameObject.GetComponent<Brick>().brickCollission.sortingOrder = 5;
            collision.gameObject.GetComponent<Brick>().initialColorG += collision.gameObject.GetComponent<Brick>().stepColor;

            LevelManager.instance.score++;
        }
     //-----------------------------------------------------------------------------------
        if (ballThrow && Balls.instance.firstBallTouchedBottom == false)
        {
            if (collision.gameObject.name == "bottom_pannel")
            {
                if (GameObject.Find("firstBall") != null)
                    GameObject.Find("firstBall").name = "previousFirstBall";

                Balls.instance.tnArrivedBallsBack.enabled = false;

                Destroy(gameObject.GetComponent<Rigidbody2D>());

                Balls.instance.firstBallTouchedBottom = true;

                Balls.instance.isFirstBallDestoyed = true;

                gameObject.name = "firstBall";

                if(GameObject.Find("firstBall").GetComponent<Ball>() != null)
                   Destroy(GameObject.Find("firstBall").GetComponent<Ball>());

                isThisNewThrowPosition = true;
            }
        }
        if (collision.gameObject.name == "bottom_pannel" && ballThrow)
        {
            Vector2 dirToFirstBall = new Vector2(gameObject.transform.position.x - GameObject.Find("firstBall").transform.position.x,
                                                 gameObject.transform.position.y - GameObject.Find("firstBall").transform.position.y);
            if (!forceAdded)
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-dirToFirstBall.x * Balls.instance.speedBalls,
                                                                             dirToFirstBall.y * Balls.instance.speedBalls);
                forceAdded = true;
            }
        }
        if (collision.gameObject.name == "firstBall" && isThisNewThrowPosition == false && this.gameObject.name != "firstBall" && ballThrow)
        {
            Balls.instance.tnArrivedBallsBack.enabled = true;
            Balls.instance.nArrivedBallsBack++;
            Balls.instance.tnArrivedBallsBack.text = "x" + Balls.instance.nArrivedBallsBack;

            if (GameObject.Find("firstBall").transform.position.x < 1.4f)
            {
                Balls.instance.tnArrivedBallsBack.GetComponent<RectTransform>().position =
                      Camera.main.WorldToScreenPoint(new Vector2(GameObject.Find("firstBall").transform.position.x + 0.36f,
                      GameObject.Find("firstBall").transform.position.y + 0.19f));
            }
            else
            {
                Balls.instance.tnArrivedBallsBack.GetComponent<RectTransform>().position =
                     Camera.main.WorldToScreenPoint(new Vector2(GameObject.Find("firstBall").transform.position.x - 0.36f,
                     GameObject.Find("firstBall").transform.position.y + 0.19f));
            }

            Destroy(this.gameObject);
        }
    }
    private void GatherBallWhenButtonPressed()
    {
        Vector2 direction = GameObject.Find("firstBall").transform.position - transform.position;

        if (GameObject.Find("firstBall").GetComponent<CircleCollider2D>() == null)
            GameObject.Find("firstBall").AddComponent<CircleCollider2D>();
        if (GameObject.Find("firstBall").GetComponent<Ball>() != null)
            Destroy(GameObject.Find("firstBall").GetComponent<Ball>());

        GameObject.Find("firstBall").GetComponent<CircleCollider2D>().enabled = true;
        GameObject.Find("firstBall").GetComponent<CircleCollider2D>().isTrigger = false;

        Destroy(GameObject.Find("firstBall").GetComponent<Rigidbody2D>());

        if (gameObject.GetComponent<Rigidbody2D>().velocity != null)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity =
                new Vector2(direction.x * Balls.instance.speedBalls, direction.y * Balls.instance.speedBalls);
        }
    }
}
