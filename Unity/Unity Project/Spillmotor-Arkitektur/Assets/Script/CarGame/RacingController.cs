using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RacingController : MonoBehaviour
{
    [SerializeField] List<GameObject> ai_cars = new List<GameObject>();
    [SerializeField] GameObject car_ui;
    [SerializeField] PhysicsCar player;

    // All the colors for current car ui
    [SerializeField] Color neutral;
    [SerializeField] Color bad_sector;
    [SerializeField] Color decent_sector;
    [SerializeField] Color good_sector;
    [SerializeField] Color best_sector;

    // Best sectors so far
    float best_sector_one = float.MaxValue;
    float best_sector_two = float.MaxValue;
    float best_sector_three = float.MaxValue;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < ai_cars.Count; i++)
        {
            ai_cars[i].GetComponent<PhysicsCar>().brain = new NeuralNetwork();
            ai_cars[i].GetComponent<PhysicsCar>().brain.Setup(5, new List<int>() { 32, 32 }, 2);
            ai_cars[i].transform.Find("Canvas").Find("Number").GetComponent<TextMeshProUGUI>().text = ai_cars[i].GetComponent<PhysicsCar>().GetIndex().ToString();
            if (i < 3)
            {
                ai_cars[i].GetComponent<PhysicsCar>().brain.ReadFromFile("Assets/SavedBrains/Expert" + (i+1).ToString() +".txt");
                ai_cars[i].GetComponent<PhysicsCar>().view_distance = 40;
            }
            else
            {
                ai_cars[i].GetComponent<PhysicsCar>().brain.ReadFromFile("Assets/SavedBrains/56sec.txt");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        TextMeshProUGUI timer = car_ui.transform.Find("Timer").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI name = car_ui.transform.Find("Car").GetComponent<TextMeshProUGUI>();
        Image sector_one = car_ui.transform.Find("Sector1").GetComponentInChildren<Image>();
        Image sector_two = car_ui.transform.Find("Sector2").GetComponentInChildren<Image>();
        Image sector_three = car_ui.transform.Find("Sector3").GetComponentInChildren<Image>();
        name.text = "Player_" + player.GetIndex();
        // Lap Time
        int floored_lap_time = (int)player.GetTimeLived();
        float decimals = player.GetTimeLived() - floored_lap_time;
        string dec = decimals.ToString("F3");
        dec = dec.Remove(0, 1);
        timer.text = TimeSpan.FromSeconds(floored_lap_time).ToString().Remove(0, 3) + dec;

        // Sector Colors
        // 1
        float s1 = player.GetSectorOne();
        float b1 = player.GetBestSectorOne();
        if (s1 <= best_sector_one && s1 != 0f)
        {
            best_sector_one = s1;
            sector_one.color = best_sector;
        }
        else if (s1 == b1 && s1 != 0f)
        {
            sector_one.color = good_sector;
        }
        else if (s1 <= b1 + 1f && s1 != 0f)
        {
            sector_one.color = decent_sector;
        }
        else if (s1 > b1 && s1 != 0f)
        {
            sector_one.color = bad_sector;
        }
        else if (s1 == 0f)
        {
            sector_one.color = neutral;
        }
        // Sector Colors
        // 2
        float s2 = player.GetSectorTwo();
        float b2 = player.GetBestSectorTwo();
        if (s2 <= best_sector_two && s2 != 0f)
        {
            best_sector_two = s2;
            sector_two.color = best_sector;
        }
        else if (s2 == b2 && s2 != 0f)
        {
            sector_two.color = good_sector;
        }
        else if (s2 <= b2 + 1f && s2 != 0f)
        {
            sector_two.color = decent_sector;
        }
        else if (s2 > b2 && s2 != 0f)
        {
            sector_two.color = bad_sector;
        }
        else if (s2 == 0f)
        {
            sector_two.color = neutral;
        }
        // Sector Colors
        // 3
        float s3 = player.GetSectorThree();
        float b3 = player.GetBestSectorThree();
        if (s3 <= best_sector_three && s3 != 0f)
        {
            best_sector_three = s3;
            sector_three.color = best_sector;
        }
        else if (s3 == b3 && s3 != 0f)
        {
            sector_three.color = good_sector;
        }
        else if (s3 <= b3 + 1f && s3 != 0f)
        {
            sector_three.color = decent_sector;
        }
        else if (s3 > b3 && s3 != 0f)
        {
            sector_three.color = bad_sector;
        }
        else if (s3 == 0f)
        {
            sector_three.color = neutral;
        }
    }
}
