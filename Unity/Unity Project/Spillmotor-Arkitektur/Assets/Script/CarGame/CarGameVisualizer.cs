using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarGameVisualizer : MonoBehaviour
{
    public static CarGameVisualizer instance;
    [SerializeField] List<RoadCollider> colliders = new List<RoadCollider>();
    [SerializeField] List<GameObject> ui_elements = new List<GameObject>();
    [SerializeField] TextMeshProUGUI lap;
    [SerializeField] bool isRacingMode = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Update()
    {
        bool no_times = true;
        int highest_lap = 0;
        for (int i = colliders.Count - 1; i >= 0; i--)
        {
            if (i == colliders.Count - 1)
            {
                if (colliders[i].HasTimes(0))
                {
                    highest_lap = colliders[i].GetLap() + 1;
                }
            }

            if (colliders[i].HasTimes(0))
            {
                no_times = false; 
            }
            if (colliders[i].GetLap() > highest_lap)
            {
                highest_lap = colliders[i].GetLap();
            }
            
        }

        lap.text = "LAP <b>" + (highest_lap + 1).ToString() + "</b>/3";
        if (no_times)
        {
            for (int i = 0; i < ui_elements.Count; i++)
            {
                TextMeshProUGUI position = ui_elements[i].transform.Find("Position").GetComponent<TextMeshProUGUI>();
                Image color = ui_elements[i].transform.Find("Color").GetComponent<Image>();
                TextMeshProUGUI car = ui_elements[i].transform.Find("Car").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI timeinterval = ui_elements[i].transform.Find("TimeInterval").GetComponent<TextMeshProUGUI>();
                position.text = (i + 1).ToString();
                color.color = Color.white;
                car.text = "Car_??";
                timeinterval.text = "Interval";
            }
            return;
        }


        List<PhysicsCar> twenty_best = new List<PhysicsCar>();
        List<float> intervals = new List<float>();
        int cars_left = 20;
        for (int i = colliders.Count - 1; i >= 0; i--)
        {
            if (colliders[i].HasTimes(highest_lap) && colliders[i].GetLap() == highest_lap)
            {
                List<PhysicsCar> cars = colliders[i].GetCars(highest_lap);
                List<float> times = colliders[i].GetTimes(highest_lap);
                for (int j = 0; j < cars.Count; j++)
                {
                    if (cars_left > 0 && !twenty_best.Contains(cars[j]))
                    {
                        twenty_best.Add(cars[j]);
                        intervals.Add(times[j] - times[0]);
                        cars_left--;
                    }
                    else
                    {
                        
                    }
                   
                }
                if (cars_left == 0)
                {
                    break;
                }

            }
        }
        if (cars_left > 0)
        {
            for (int i = colliders.Count - 1; i >= 0; i--)
            {
                if (colliders[i].HasTimes(highest_lap - 1) && colliders[i].GetLap() < highest_lap)
                {
                    List<PhysicsCar> cars = colliders[i].GetCars(highest_lap - 1);
                    List<float> times = colliders[i].GetTimes(highest_lap - 1);
                    for (int j = 0; j < cars.Count; j++)
                    {
                        if (cars_left > 0 && !twenty_best.Contains(cars[j]))
                        {
                            twenty_best.Add(cars[j]);
                            intervals.Add(times[j] - times[0]);
                            cars_left--;
                        }
                        else
                        {
                            
                        }

                    }
                    if (cars_left == 0)
                    {
                        break;
                    }
                }
            }
        }


        for (int i = 0; i < twenty_best.Count; i++)
        {
            if (!isRacingMode)
            {
                TextMeshProUGUI position = ui_elements[i].transform.Find("Position").GetComponent<TextMeshProUGUI>();
                Image color = ui_elements[i].transform.Find("Color").GetComponent<Image>();
                TextMeshProUGUI car = ui_elements[i].transform.Find("Car").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI timeinterval = ui_elements[i].transform.Find("TimeInterval").GetComponent<TextMeshProUGUI>();
                position.text = (i + 1).ToString();
                color.color = twenty_best[i].GetColor();
                car.text = "Car_" + twenty_best[i].GetIndex().ToString();
                timeinterval.text = intervals[i] == 0f ? "Interval" : "+" + intervals[i].ToString("F3");
                bool is_not_dead = (twenty_best[i].current_checkpoint == 0 && twenty_best[i].current_lap > 0);
                timeinterval.text = twenty_best[i].dead && !is_not_dead ? "DNF" : timeinterval.text;
            }
            else
            {
                TextMeshProUGUI position = ui_elements[i].transform.Find("Position").GetComponent<TextMeshProUGUI>();
                Image color = ui_elements[i].transform.Find("Color").GetComponent<Image>();
                TextMeshProUGUI car = ui_elements[i].transform.Find("Car").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI timeinterval = ui_elements[i].transform.Find("TimeInterval").GetComponent<TextMeshProUGUI>();
                position.text = (i + 1).ToString();
                color.color = twenty_best[i].GetColor();
                if (twenty_best[i].GetComponent<PhysicsCar>().isPlayer)
                {
                    car.text = "Player_" + twenty_best[i].GetIndex().ToString();
                }
                else
                {
                    car.text = "AI_" + twenty_best[i].GetIndex().ToString();
                }
                timeinterval.text = intervals[i] == 0f ? "Interval" : "+" + intervals[i].ToString("F3");
                bool is_not_dead = (twenty_best[i].current_checkpoint == 0 && twenty_best[i].current_lap > 0);
                timeinterval.text = twenty_best[i].dead && !is_not_dead ? "DNF" : timeinterval.text;

            }

        }


        

    }
    public PhysicsCar BestCar()
    {
        bool no_times = true;
        int highest_lap = 0;
        for (int i = colliders.Count - 1; i >= 0; i--)
        {
            if (i == colliders.Count - 1)
            {
                if (colliders[i].HasTimes(0))
                {
                    highest_lap = colliders[i].GetLap() + 1;
                }
            }

            if (colliders[i].HasTimes(0))
            {
                no_times = false;
            }
            if (colliders[i].GetLap() > highest_lap)
            {
                highest_lap = colliders[i].GetLap();
            }

        }
        if (no_times)
        {
            return null;
        }

        for (int i = colliders.Count - 1; i >= 0; i--)
        {
            if (colliders[i].HasTimes(highest_lap) && colliders[i].GetLap() == highest_lap)
            {
                List<PhysicsCar> cars = colliders[i].GetCars(highest_lap);
                List<float> times = colliders[i].GetTimes(highest_lap);
                for (int j = 0; j < cars.Count; j++)
                {
                    if (!cars[j].dead)
                        return cars[j];

                }
            }
        }

        return null;
    }
}
