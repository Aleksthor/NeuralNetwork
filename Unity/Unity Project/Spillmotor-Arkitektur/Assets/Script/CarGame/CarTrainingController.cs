using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CarTrainingController : MonoBehaviour
{

    // Singleton
    public static CarTrainingController instance;
    // List of camera position for cinematic view
    [SerializeField] List<Vector3> camera_positions = new List<Vector3>();

    // Prefabs
    [SerializeField] GameObject car_prefab;
    [SerializeField] GameObject main_camera;

    // Logic variables
    // Spawn Position on the track
    [SerializeField] Vector3 spawn_position = new Vector3(0f, 0f, -25f);
    // How many cars per generation
    [Range(1, 200)]
    [SerializeField] int cars_per_generation = 20;
    // How many are selected to create offspring
    [Range(1, 50)]
    [SerializeField] int chosen_parents = 2;
    // What is the current generation
    [SerializeField] int generation = 0;
    // The list of the all the cars
    [SerializeField] List<GameObject> cars = new List<GameObject>();
    // A UI Element where we show the best car so far
    [SerializeField] GameObject best_car_ui;
    [SerializeField] string path_to_save_brain = "/Assets/";
    // A list of all the colliders on the track
    [SerializeField] List<RoadCollider> colliders = new List<RoadCollider>();
    // The current car UI element
    [SerializeField] GameObject current_car_ui;
    // All the colors for current car ui
    [SerializeField] Color neutral;
    [SerializeField] Color bad_sector;
    [SerializeField] Color decent_sector;
    [SerializeField] Color good_sector;
    [SerializeField] Color best_sector;


    // Current fitness mode - will determine what the cars will focus on
    int fitness_mode = 0;
    // Best sectors so far
    float best_sector_one = float.MaxValue;
    float best_sector_two = float.MaxValue;
    float best_sector_three = float.MaxValue;

    // Cinematic mode switch
    private bool cinematic_mode = false;

    // Camera Controls
    [Range(1f,30f)]
    [SerializeField] float camera_up_distance = 20f;
    [Range(1f, 30f)]
    [SerializeField] float camera_back_distance = 20f;

    // Elements on the UI
    [SerializeField] TextMeshProUGUI current_generation;
    [SerializeField] TextMeshProUGUI generation_size;
    [SerializeField] TextMeshProUGUI mutation_pool_size;
    [SerializeField] TextMeshProUGUI average_fitness;
    [SerializeField] TextMeshProUGUI best_fitness;
    [SerializeField] TextMeshProUGUI lap_time;
    [SerializeField] TextMeshProUGUI number_of_laps;
    [SerializeField] Button start_over;
    [SerializeField] Button save_best_brain;
    [SerializeField] Button open_visualizer;
    [SerializeField] GameObject visualizer;

    // The visulizerobject we send brains to
    NetworkVisualizer network_visulizer = null;

    // Give new cars a unique id from 1-100
    int unique_id = 1;
    // Save the best lap time
    float best_lap_time = float.MaxValue;
    // Save the name of the fastest car
    string best_car_name;
    // Save the color of the fastest car
    Color best_car_color;
    // Save the best brain so we can print it to a file
    NeuralNetwork best_brain = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        save_best_brain.onClick.AddListener(SaveBrainToFile);
        open_visualizer.onClick.AddListener(ToggleVisualizer);
        network_visulizer = visualizer.GetComponent<NetworkVisualizer>();

        for (int i = 0; i < cars_per_generation; i++)
        {
            cars.Add(Instantiate(car_prefab, spawn_position, Quaternion.Euler(0f, 45f, 0f)));
            cars[i].transform.Find("Canvas").Find("Number").GetComponent<TextMeshProUGUI>().text = unique_id.ToString();
            cars[i].GetComponent<PhysicsCar>().SetIndex(unique_id);
            unique_id++;
        }

        if (main_camera == null)
        {
            main_camera = GameObject.Find("Main Camera");
        }

        start_over.onClick.AddListener(StartOver);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            cinematic_mode = !cinematic_mode;
        }


        bool all_dead = true;
        int done = 0;
        for (int i = 0; i < cars.Count; i++)
        {
            PhysicsCar car = cars[i].GetComponent<PhysicsCar>();
            if (car.done)
            {
                done++;
            }
            if (!car.dead)
            {
                all_dead = false;
                break;
            }
        }
        PhysicsCar follow_car = CarGameVisualizer.instance.BestCar();



        if (follow_car != null)
        {
            if (cinematic_mode)
            {
                main_camera.transform.position = camera_positions[follow_car.current_checkpoint];
            }
            else
            {
                Vector3 prefered_pos = follow_car.transform.position -
                                        (follow_car.transform.forward * camera_back_distance) +
                                        (follow_car.transform.up * camera_up_distance);

                main_camera.transform.position = prefered_pos;
            }
            main_camera.transform.LookAt(follow_car.transform, new Vector3(0, 1, 0));
            network_visulizer.SetNetwork(follow_car.GetComponent<PhysicsCar>().brain);


            // Current Car UI
            TextMeshProUGUI timer = current_car_ui.transform.Find("Timer").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI name = current_car_ui.transform.Find("Car").GetComponent<TextMeshProUGUI>();
            Image sector_one = current_car_ui.transform.Find("Sector1").GetComponentInChildren<Image>();
            Image sector_two = current_car_ui.transform.Find("Sector2").GetComponentInChildren<Image>();
            Image sector_three = current_car_ui.transform.Find("Sector3").GetComponentInChildren<Image>();
            PhysicsCar car = follow_car.GetComponent<PhysicsCar>();
            name.text = "Car_" + car.GetIndex();
            // Lap Time
            int floored_lap_time = (int)car.GetTimeLived();
            float decimals = car.GetTimeLived() - floored_lap_time;
            string dec = decimals.ToString("F3");
            dec = dec.Remove(0, 1);
            timer.text = TimeSpan.FromSeconds(floored_lap_time).ToString().Remove(0, 3) + dec;

            // Sector Colors
            // 1
            float s1 = car.GetSectorOne();
            float b1 = car.GetBestSectorOne();
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
            float s2 = car.GetSectorTwo();
            float b2 = car.GetBestSectorTwo();
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
            float s3 = car.GetSectorThree();
            float b3 = car.GetBestSectorThree();
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
        else
        {
            Vector3 prefered_pos = spawn_position -
                        (new Vector3(1,0,1).normalized * camera_back_distance) +
                        (new Vector3(0, 1, 0).normalized * camera_up_distance);
            main_camera.transform.position = prefered_pos;
            main_camera.transform.LookAt(spawn_position, new Vector3(0, 1, 0));


            // Current Car UI
            TextMeshProUGUI timer = current_car_ui.transform.Find("Timer").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI name = current_car_ui.transform.Find("Car").GetComponent<TextMeshProUGUI>();
            Image sector_one = current_car_ui.transform.Find("Sector1").GetComponentInChildren<Image>();
            Image sector_two = current_car_ui.transform.Find("Sector2").GetComponentInChildren<Image>();
            Image sector_three = current_car_ui.transform.Find("Sector3").GetComponentInChildren<Image>();
            name.text = "Car_??";
            // Lap Time
            int floored_lap_time = 0;
            float decimals = 0f - floored_lap_time;
            string dec = decimals.ToString("F3");
            dec = dec.Remove(0, 1);
            timer.text = TimeSpan.FromSeconds(floored_lap_time).ToString().Remove(0, 3) + dec;

            // Sector Colors
            sector_one.color = neutral;
            sector_two.color = neutral;
            sector_three.color = neutral;
        }



        current_generation.text = generation.ToString();
        generation_size.text = cars_per_generation.ToString();
        mutation_pool_size.text = chosen_parents.ToString();

        if (all_dead || done > 25)
        {
            SpawnNewGeneration();
            generation++;
            main_camera.transform.position = spawn_position + new Vector3(-20, 30, -20);
            main_camera.transform.LookAt(spawn_position, new Vector3(0, 1, 0));
        }

    }


    void SpawnNewGeneration()
    {

        // Get the best cars into a new List "parents"
        List<GameObject> parents = new List<GameObject>();
        for (int n = 0; n < chosen_parents; n++)
        {
            float best = 0;
            int index = 0;
            float total = 0;      
            for (int i = 0; i < cars.Count; i++)
            {
                PhysicsCar car = cars[i].GetComponent<PhysicsCar>();
                total += car.fitness;
                if (car.fitness > best)
                {
                    best = car.fitness;
                    index = i; 
                }

            }
            PhysicsCar best_car = cars[index].GetComponent<PhysicsCar>();
            if (best_brain == null)
            {
                best_brain = best_car.brain;
            }

            if (n == 0)
            {
                average_fitness.text = (total / cars_per_generation).ToString();
                PhysicsCar car = best_car;
                best_fitness.text = car.fitness.ToString();
                int floored_lap_time = (int)car.GetBestLapTime();
                float decimals = car.GetBestLapTime() - floored_lap_time;
                string dec = decimals.ToString("F3");
                dec = dec.Remove(0,1);
                lap_time.text = TimeSpan.FromSeconds(floored_lap_time).ToString().Remove(0, 3) + dec;
                number_of_laps.text = car.current_lap.ToString();
            }
            if ((total / cars_per_generation) > 2000f)
            {
                fitness_mode = 1;
            }
            float this_best_lap_time = best_car.GetBestLapTime();
            if (best_lap_time > this_best_lap_time && this_best_lap_time != 0f)
            {
                best_lap_time = this_best_lap_time;
                best_car_name = "Car_" + best_car.GetIndex().ToString();
                best_car_color = best_car.GetColor();
                best_brain = best_car.brain;
                // Get all components
                TextMeshProUGUI position = best_car_ui.transform.Find("Position").GetComponent<TextMeshProUGUI>();
                Image color = best_car_ui.transform.Find("Color").GetComponent<Image>();
                TextMeshProUGUI name = best_car_ui.transform.Find("Car").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI timeinterval = best_car_ui.transform.Find("TimeInterval").GetComponent<TextMeshProUGUI>();

                // Set the data
                position.text = "1";
                color.color = best_car_color;
                name.text = best_car_name + " (Gen " + generation.ToString() + ")";
                int floored_lap_time = (int)best_lap_time;
                float decimals = best_lap_time - floored_lap_time;
                string dec = decimals.ToString("F3");
                dec = dec.Remove(0, 1);
                timeinterval.text = TimeSpan.FromSeconds(floored_lap_time).ToString().Remove(0,3) + dec;

                TextMeshProUGUI s1 = best_car_ui.transform.parent.Find("Sectors").Find("Sector1").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI s2 = best_car_ui.transform.parent.Find("Sectors").Find("Sector2").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI s3 = best_car_ui.transform.parent.Find("Sectors").Find("Sector3").GetComponent<TextMeshProUGUI>();

                float s1_time = best_car.GetBestLapSectorOne();
                Debug.Log("Sector1 time " + s1_time);
                int floored_s1_time = (int)s1_time;
                decimals = s1_time - floored_s1_time;
                dec = decimals.ToString("F3");
                dec = dec.Remove(0, 1);
                s1.text = TimeSpan.FromSeconds(floored_s1_time).ToString().Remove(0, 6) + dec;

                float s2_time = best_car.GetBestLapSectorTwo();
                int floored_s2_time = (int)s2_time;
                decimals = s2_time - floored_s2_time;
                dec = decimals.ToString("F3");
                dec = dec.Remove(0, 1);
                s2.text = TimeSpan.FromSeconds(floored_s2_time).ToString().Remove(0, 6) + dec;

                float s3_time = best_car.GetBestLapSectorThree();
                int floored_s3_time = (int)s3_time;
                decimals = s3_time - floored_s3_time;
                dec = decimals.ToString("F3");
                dec = dec.Remove(0, 1);
                s3.text = TimeSpan.FromSeconds(floored_s3_time).ToString().Remove(0, 6) + dec;
            }

            parents.Add(cars[index]);
            cars.RemoveAt(index);
        }



        int parent_index = 0;
        // "Mutate" the parents to get a full generation
        unique_id = 1;
        for (int i = 0; i < cars.Count; i++)
        { 
            PhysicsCar child = cars[i].GetComponent<PhysicsCar>();
            child.fitness_mode = fitness_mode;
            bool insert = false;
            while (!insert)
            {
                insert = true;
                for (int j = parents.Count - 1; j >= 0; j--)
                {
                    if (parents[j].GetComponent<PhysicsCar>().GetIndex() == unique_id)
                    {
                        insert = false;
                    }
                }
                if (!insert)
                {
                    unique_id++;
                }
            }
            child.SetIndex(unique_id);
            unique_id++;
            PhysicsCar parent = parents[parent_index].GetComponent<PhysicsCar>();
            child.brain = parent.brain.Copy();
            child.brain.GeneticAlgorithm();
            parent_index = (parent_index + 1) % parents.Count;

        }
        for (int i = parents.Count - 1; i >= 0; i--)
        {
            PhysicsCar child = parents[i].GetComponent<PhysicsCar>();
            child.fitness_mode = fitness_mode;
            cars.Add(parents[i]);
            parents.RemoveAt(i); 
        }



        // Set the spawn position again on all cars
        for (int i = 0; i < cars_per_generation; i++)
        {
            cars[i].transform.position = spawn_position;
            PhysicsCar car = cars[i].GetComponent<PhysicsCar>();
            cars[i].transform.Find("Canvas").Find("Number").GetComponent<TextMeshProUGUI>().text = car.GetIndex().ToString();
            car.ResetCar();
            car.position = spawn_position;
            car.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
        }

        for (int i = 0; i < colliders.Count; i++)
        {
            colliders[i].ResetCollider();
        }
    }

    public void StartOver()
    {
        for (int i = cars.Count - 1; i >= 0; i--)
        {
            GameObject go = cars[i];
            cars.RemoveAt(i);
            Destroy(go);
        }
        fitness_mode = 0;

        for (int i = 0; i < cars_per_generation; i++)
        {
            cars.Add(Instantiate(car_prefab, spawn_position, Quaternion.Euler(0f, 45f, 0f)));
        }
    }

    void SaveBrainToFile()
    {
        if (best_brain != null)
        {
            string path = AssetDatabase.GenerateUniqueAssetPath(instance.path_to_save_brain);
            best_brain.WriteToFile(path);
        }
    }

    void ToggleVisualizer()
    {
        visualizer.SetActive(!visualizer.activeSelf);
    }
}
