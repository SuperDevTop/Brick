using UnityEngine;

public class DestroyBalls : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ball" && LevelManager.instance.gatherBalls == false)
        {
            Balls.instance.nArrivedBallsBack++;
            print("kkkkk");
            Destroy(collision.gameObject);
        }
    }
}
