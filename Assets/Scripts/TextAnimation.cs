using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAnimation : MonoBehaviour
{
    private Text text;
    private float tForAnim; 

    void Start()
    {
        text = GetComponent<Text>();
    }

    void OnEnable() {
        tForAnim = 4.0f; //1 sec offset to start text from 3
    }

    void Update()
    {
        if (tForAnim >= 1) {
            float t = Mathf.Repeat((4 - tForAnim), 1.0f);
            text.fontSize = Mathf.RoundToInt(Mathf.SmoothStep(100.0f, 150.0f, t));
            tForAnim -= Time.deltaTime;
            int tRounded = Mathf.FloorToInt(tForAnim);
            text.text = tRounded.ToString();
        }        
    }
}
