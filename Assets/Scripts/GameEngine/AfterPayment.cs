using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterPayment : MonoBehaviour
{
    public static AfterPayment Instance;
    public bool isPaid;
    public float payAmount;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaid)
        {
            isPaid = false;

            if (payAmount == 0.09f)
            {
                PlayerPrefs.SetInt("crystalsNum", PlayerPrefs.GetInt("crystalsNum") + 100);
                PlayerPrefs.Save();
            }
            else if (payAmount == 0.19f)
            {
                PlayerPrefs.SetInt("crystalsNum", PlayerPrefs.GetInt("crystalsNum") + 300);
                PlayerPrefs.Save();
            }
            else if (payAmount == 0.49f)
            {
                PlayerPrefs.SetInt("crystalsNum", PlayerPrefs.GetInt("crystalsNum") + 800);
                PlayerPrefs.Save();
            }
            else if (payAmount == 0.99f)
            {
                PlayerPrefs.SetInt("crystalsNum", PlayerPrefs.GetInt("crystalsNum") + 1200);
                PlayerPrefs.Save();
            }
            else if (payAmount == 1.49f)
            {
                PlayerPrefs.SetInt("crystalsNum", PlayerPrefs.GetInt("crystalsNum") + 2000);
                PlayerPrefs.Save();
            }
            else if (payAmount == 1.99f)
            {
                PlayerPrefs.SetInt("crystalsNum", PlayerPrefs.GetInt("crystalsNum") + 3000);
                PlayerPrefs.Save();
            }
        }
    }
}
