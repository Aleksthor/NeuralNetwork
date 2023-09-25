using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    // The object we spawn in the world
    [SerializeField] GameObject car_prefab;
    [SerializeField] GameObject main_camera;

    // Logic variables
    [SerializeField] Vector3 spawn_position = new Vector3(0f, 0f, -25f);
    [SerializeField] int cars_per_generation = 20;
    [SerializeField] int chosen_parents = 2;
    [SerializeField] int generation = 0;
    [SerializeField] List<GameObject> cars = new List<GameObject>();
    [SerializeField] bool supervised_learning = true;

    int fitness_mode = 0;
    int b = 0;
    int n = 0;

    [SerializeField] TextMeshProUGUI current_generation;
    [SerializeField] TextMeshProUGUI generation_size;
    [SerializeField] TextMeshProUGUI mutation_pool_size;
    [SerializeField] TextMeshProUGUI average_fitness;

    [SerializeField] TextMeshProUGUI best_fitness;
    [SerializeField] TextMeshProUGUI average_speed;
    [SerializeField] TextMeshProUGUI number_of_laps;
    [SerializeField] Button start_over;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < cars_per_generation; i++)
        {
            cars.Add(Instantiate(car_prefab, spawn_position, Quaternion.identity));
            cars[i].GetComponent<PhysicsCar>().supervised_learning = supervised_learning;
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
        bool all_dead = true;
        for (int i = 0; i < cars.Count; i++)
        {
            PhysicsCar car = cars[i].GetComponent<PhysicsCar>();
            if (!car.dead)
            {
                all_dead = false;
                break;
            }
        }

        for (int i = 0; i < cars.Count; i++)
        {
            PhysicsCar car = cars[i].GetComponent<PhysicsCar>();
            if (car.fitness > b)
            {
                b = (int)car.fitness;
                n = i;
            }
        }



        if (cars.Count > n)
        {
            if (cars[n].GetComponent<PhysicsCar>().dead)
            {
                int b2 = 0;
                int n2 = 0;
                for (int i = 0; i < cars.Count; i++)
                {
                    PhysicsCar car = cars[i].GetComponent<PhysicsCar>();
                    if (car.fitness > b2 && !car.dead)
                    {
                        b2 = (int)car.fitness;
                        n2 = i;
                    }
                }
                n = n2;
            }


            if (cars[n].GetComponent<PhysicsCar>().current_checkpoint > 0 || cars[n].GetComponent<PhysicsCar>().current_lap > 0)
            {
                main_camera.transform.position = cars[n].transform.position;
                main_camera.transform.position += new Vector3(0, 30, 0);
                main_camera.transform.position -= cars[n].transform.forward * 30;

                main_camera.transform.LookAt(cars[n].transform, new Vector3(0, 1, 0));
            }
            else
            {
                main_camera.transform.position = new Vector3(-95, 30, -75);
                main_camera.transform.eulerAngles = new Vector3(45,0,0);
            }

        }
        else
        {
            main_camera.transform.position = new Vector3(-95, 30, -75);
            main_camera.transform.eulerAngles = new Vector3(45, 0, 0);
        }

        current_generation.text = generation.ToString();
        generation_size.text = cars_per_generation.ToString();
        mutation_pool_size.text = chosen_parents.ToString();

        if (all_dead)
        {
            b = 0;
            n = 0;
            SpawnNewGeneration();
            generation++;
        }

    }


    void SpawnNewGeneration()
    {
        // Get the best cars into a new List "parents"
        List<GameObject> parents = new List<GameObject>();
        for (int n = 0; n < chosen_parents; n++)
        {
            int best = 0;
            int index = 0;
            int total = 0;
            for (int i = 0; i < cars.Count; i++)
            {
                PhysicsCar car = cars[i].GetComponent<PhysicsCar>();
                total += (int)car.fitness;
                if (car.fitness > best)
                {
                    best = (int)car.fitness;
                    index = i; 
                }
            }
            if (n == 0)
            {
                Debug.Log("The best fitness this generation (" + generation + "): " + cars[index].GetComponent<PhysicsCar>().fitness);
                Debug.Log("Average fitness this generation (" + generation + "): " + (total / cars_per_generation));
                average_fitness.text = (total / cars_per_generation).ToString();
                PhysicsCar car = cars[index].GetComponent<PhysicsCar>();
                best_fitness.text = car.fitness.ToString();
                average_speed.text = car.average_velocity.magnitude.ToString("F2") + " m/s";
                number_of_laps.text = car.current_lap.ToString();

            }

            parents.Add(cars[index]);
            cars.RemoveAt(index);
        }


        // Destroy the "bad" cars
        for (int i = cars.Count - 1; i >= 0; i--)
        { 
            GameObject go = cars[i];
            cars.RemoveAt(i);
            Destroy(go);
        }

        int parent_index = 0;
        // "Mutate" the parents to get a full generation
        for (int i = chosen_parents; i < cars_per_generation; i++)
        {
            Debug.Log("Evolving");
            cars.Add(Instantiate(car_prefab, spawn_position, Quaternion.identity));
            PhysicsCar child = cars[cars.Count - 1].GetComponent<PhysicsCar>();
            child.supervised_learning = supervised_learning;
            child.fitness_mode = fitness_mode;

            PhysicsCar parent = parents[parent_index].GetComponent<PhysicsCar>();
            child.brain = parent.brain.Copy();
            child.brain.GeneticAlgorithm();
            parent_index = (parent_index + 1) % parents.Count;

        }
        for (int i = parents.Count - 1; i >= 0; i--)
        {
            Debug.Log("Prime - Last Gen Copy");
            cars.Add(Instantiate(car_prefab, spawn_position, Quaternion.identity));
            PhysicsCar child = cars[cars.Count - 1].GetComponent<PhysicsCar>();
            child.supervised_learning = supervised_learning;
            child.fitness_mode = fitness_mode;

            child.brain = parents[i].GetComponent<PhysicsCar>().brain.Copy();
            if (i == 0)
            {
                child.can_debug = true;
            }


            GameObject go = parents[i];
            parents.RemoveAt(i);
            Destroy(go);    
        }



        // Set the spawn position again on all cars

        for (int i = 0; i < cars_per_generation; i++)
        {
            cars[i].transform.position = spawn_position;
            PhysicsCar car = cars[i].GetComponent<PhysicsCar>();
            car.position = spawn_position;
            car.dead = false;
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


        for (int i = 0; i < cars_per_generation; i++)
        {
            cars.Add(Instantiate(car_prefab, spawn_position, Quaternion.identity));
            cars[i].GetComponent<PhysicsCar>().supervised_learning = supervised_learning;
        }
    }
}
