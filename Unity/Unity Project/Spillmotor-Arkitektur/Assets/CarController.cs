using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // The object we spawn in the world
    [SerializeField] GameObject car_prefab;

    // Logic variables
    Vector3 spawn_position = new Vector3(0f, 0f, -25f);
    [SerializeField] int cars_per_generation = 20;
    [SerializeField] int chosen_parents = 2;
    [SerializeField] int generation = 0;
    [SerializeField] List<GameObject> cars = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < cars_per_generation; i++)
        {
            cars.Add(Instantiate(car_prefab, spawn_position, Quaternion.identity));
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
            for (int i = 0; i < cars.Count; i++)
            {
                PhysicsCar car = cars[i].GetComponent<PhysicsCar>();
                if (car.fitness > best)
                {
                    best = (int)car.fitness;
                    index = i; 
                }
            }

            if (n == 0)
            {
                Debug.Log("The best fitness this generation: " + cars[index].GetComponent<PhysicsCar>().fitness);
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


        // "Mutate" the parents to get a full generation
        for (int i = chosen_parents; i < cars_per_generation; i++)
        {
            cars.Add(Instantiate(car_prefab, spawn_position, Quaternion.identity));
            PhysicsCar child = cars[cars.Count - 1].GetComponent<PhysicsCar>();

            int parent_index = UnityEngine.Random.Range(0, parents.Count);
            PhysicsCar parent = parents[parent_index].GetComponent<PhysicsCar>();

            child.brain = parent.brain;
            child.brain.GeneticAlgorithm();

        }
        for (int i = parents.Count - 1; i >= 0; i--)
        {
            cars.Add(Instantiate(car_prefab, spawn_position, Quaternion.identity));
            PhysicsCar child = cars[cars.Count - 1].GetComponent<PhysicsCar>();
            child.brain = parents[i].GetComponent<PhysicsCar>().brain;

            GameObject go = parents[i];
            parents.RemoveAt(i);
            Destroy(go);    
        }



        // Set the spawn position again on all cars

        for (int i = 0; i < cars_per_generation; i++)
        {
            cars[i].transform.position = spawn_position;
            PhysicsCar car = cars[i].GetComponent<PhysicsCar>();
            car.dead = false;
        }
    }
}
