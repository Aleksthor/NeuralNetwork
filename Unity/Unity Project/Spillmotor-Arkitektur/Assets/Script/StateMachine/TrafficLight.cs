using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT.StateMachine
{

    public enum TrafficColor
    {
        Green,
        Yellow,
        Red
    }
    public class TrafficLight : MonoBehaviour
    {
        float timer = 0f;
        [SerializeField] float green_timer = 10f;
        [SerializeField] float yellow_timer = 3f;
        [SerializeField] float red_timer = 10f;
        TrafficColor currentColor;
        TrafficColor lastColor;
        [SerializeField] Color color;
        [SerializeField] Material off;
        [SerializeField] Material green_color;
        [SerializeField] Material yellow_color;
        [SerializeField] Material red_color;
        [SerializeField] MeshRenderer green;
        [SerializeField] MeshRenderer yellow;
        [SerializeField] MeshRenderer red;

        private void Start()
        {
            currentColor = TrafficColor.Red;
            lastColor = TrafficColor.Yellow;
        }

        private void Update()
        {
            if (timer > 0f)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                ChangeState();   
            }

            
        }


        void ChangeState()
        {
            switch (currentColor)
            {
                case TrafficColor.Green:
                    lastColor = currentColor;
                    currentColor = TrafficColor.Yellow;
                    timer = yellow_timer;
                    yellow.material = yellow_color;
                    green.material = off;
                    break;
                case TrafficColor.Yellow:
                    switch (lastColor)
                    {
                        case TrafficColor.Green:
                            lastColor = currentColor;
                            currentColor = TrafficColor.Red;
                            timer = red_timer;
                            yellow.material = off;
                            red.material = red_color;
                            break;
                        case TrafficColor.Red:
                            lastColor = currentColor;
                            currentColor = TrafficColor.Green;
                            timer = green_timer; 
                            yellow.material = off;
                            green.material = green_color;
                            break;
                    }
                    break;
                case TrafficColor.Red:
                    lastColor = currentColor;
                    currentColor = TrafficColor.Yellow;
                    timer = yellow_timer;
                    yellow.material = yellow_color;
                    red.material = off;
                    break;
            }
        }
    }
}
