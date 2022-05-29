using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Paymentwall;

public class Payment : MonoBehaviour
{
    public float price;
    PWBrick brick;

    // Start is called before the first frame update
    void Start()
    {
        PWBase.SetApiMode(API_MODE.LIVE);             
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Click()
    {
        AfterPayment.Instance.payAmount = price;        

        if (price == 0.09f)
        {
            brick = new PWBrick(price, "USD", "Kilanerin Games", "Buy 100 Jewels");
        }
        else if (price == 0.19f)
        {
            brick = new PWBrick(price, "USD", "Kilanerin Games", "Buy 300 Jewels");
        }
        else if (price == 0.49f)
        {
            brick = new PWBrick(price, "USD", "Kilanerin Games", "Buy 800 Jewels");
        }
        else if (price == 0.99f)
        {
            brick = new PWBrick(price, "USD", "Kilanerin Games", "Buy 1200 Jewels");
        }
        else if (price == 1.49f)
        {
            brick = new PWBrick(price, "USD", "Kilanerin Games", "Buy 2000 Jewels");
        }
        else if (price == 1.99f)
        {
            brick = new PWBrick(price, "USD", "Kilanerin Games", "Buy 3000 Jewels");
        }
        
        PWBase.SetAppKey("c41847bfc51c8a9c72a390848d7f8454");
        PWBase.SetSecretKey("4d176d842330b80a08b6b500529d3d46");
        brick.ShowPaymentForm();
    }
}
