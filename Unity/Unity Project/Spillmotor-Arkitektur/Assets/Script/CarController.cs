using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

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
        int b = 0;
        int n = 0;
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
            main_camera.transform.position = cars[n].transform.position + new Vector3(0, 45, 0);
        }
        else
        {
            main_camera.transform.position = new Vector3(-25, 175, -30);
        }

        if (all_dead)
        {

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
                Debug.Log("Average fitness this generation (" + generation + "): " + (total/cars_per_generation));
            }
            if ((total / cars_per_generation) > 4000)
            {
                fitness_mode = 1;

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
}
