using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapPrompter : MonoBehaviour
{
    private float promptTimer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        promptTimer += Time.deltaTime * 2;
        this.GetComponent<CanvasGroup>().alpha = (1 + Mathf.Sin(promptTimer)) / 2;
    }
}
