using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    public static CarManager instance; //Singleton

    [Header("Car Stats")]
    [SerializeField] GameObject car_Parent;
    [SerializeField] GameObject car_Prefab;
    [SerializeField] List<GameObject> carList = new List<GameObject>();

    [SerializeField] int carsToSpawn = 20;
    [SerializeField] int despawnTime = 20;
    public Vector3 carSpawnPosition;

    [Header("AI stats")]
    [SerializeField] float timeAlive;
    [SerializeField] float highestTimeAlive;
    [SerializeField] int generation = 1;
    [SerializeField] bool training = true;
    [SerializeField] int parentsAmount = 2;


    //--------------------


    private void Awake()
    {
        //Singleton Functionality
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        carSpawnPosition = car_Parent.transform.position;
    }
    private void Start()
    {
        AddCar(carsToSpawn, carSpawnPosition);
    }
    private void Update()
    {
        timeAlive += Time.deltaTime;

        CheckIfAllCarsHaveCrashed();

    }


    //--------------------


    #region Add Cars
    void AddCar()
    {
        carList.Add(Instantiate(car_Prefab) as GameObject);
        carList[carList.Count - 1].transform.SetParent(car_Parent.transform);

        carList[carList.Count - 1].GetComponent<Car>().training = training;
        carList[carList.Count - 1].GetComponent<Car>().despawnTime = despawnTime;
    }
    void AddCar(Vector3 position)
    {
        carList.Add(Instantiate(car_Prefab) as GameObject);
        carList[carList.Count - 1].transform.SetParent(car_Parent.transform);
        carList[carList.Count - 1].transform.position.Set(position.x, position.y, position.z);

        carList[carList.Count - 1].GetComponent<Car>().training = training;
        carList[carList.Count - 1].GetComponent<Car>().despawnTime = despawnTime;
    }
    void AddCar(int amount, Vector3 position)
    {
        for (int i = 0; i < amount; i++)
        {
            carList.Add(Instantiate(car_Prefab) as GameObject);
            carList[carList.Count - 1].transform.SetParent(car_Parent.transform);
            carList[carList.Count - 1].transform.position.Set(position.x, position.y, position.z);

            carList[carList.Count - 1].GetComponent<Car>().training = training;
            carList[carList.Count - 1].GetComponent<Car>().despawnTime = despawnTime;
        }
    }
    #endregion


    //--------------------


    void CheckIfAllCarsHaveCrashed()
    {
        //Check if any car have crashed
        bool allCarsHasCrashed = true;
        for (int i = 0; i < carList.Count; i++)
        {
            Car car = carList[i].GetComponent<Car>();
            if (car.isAlive)
            {
                allCarsHasCrashed = false;
                break;
            }
        }

        //If all cars have crashed, start new session with new Generation
        if (allCarsHasCrashed)
        {
            MakeNewGeneration();
            generation++;
        }
    }
    void MakeNewGeneration()
    {
        timeAlive = 0;

        //Make a new List of the best performing cars
        List<GameObject> newParents = new List<GameObject>();
        for (int n = 0; n < parentsAmount; n++)
        {
            float bestTimeLastingCar = 0;
            int parentIndex = 0;

            //Get the car with the highest checkPoint amount
            for (int i = 0; i < carList.Count; i++)
            {
                //Car car = carList[i].GetComponent<Car>();

                if (carList[i].GetComponent<Car>().timeAlive > bestTimeLastingCar)
                {
                    bestTimeLastingCar = carList[i].GetComponent<Car>().timeAlive;
                    parentIndex = i;
                }
            }

            highestTimeAlive = bestTimeLastingCar;

            newParents.Add(carList[parentIndex]);
            carList.RemoveAt(parentIndex);
        }

        //Destroy the bad performing cars
        for (int i = carList.Count - 1; i >= 0; i--)
        {
            GameObject carToDestroy = carList[i];

            carList.RemoveAt(i);
            Destroy(carToDestroy);
        }

        //Mutate the best cars into a new generation
        int newParent_index = 0;

        for (int i = parentsAmount; i < carsToSpawn; i++)
        {
            AddCar(carSpawnPosition);

            Car child = carList[carList.Count - 1].GetComponent<Car>();
            child.training = training;

            Car parent = newParents[newParent_index].GetComponent<Car>();
            child.brain = parent.brain.Copy();
            child.brain.GeneticAlgorithm();
            newParent_index = (newParent_index + 1) % newParents.Count;
        }

        for (int i = newParents.Count - 1; i >= 0; i--)
        {
            AddCar(carSpawnPosition);

            Car child = carList[carList.Count - 1].GetComponent<Car>();
            child.training = training;

            child.brain = newParents[i].GetComponent<Car>().brain.Copy();
            if (i == 0)
            {
                child.isReadyForDebug = true;
            }

            GameObject parentToDestroy = newParents[i];
            newParents.RemoveAt(i);
            Destroy(parentToDestroy);
        }

        //Set new spawn position of the new generation
        //for (int i = 0; i < carsToSpawn; i++)
        //{
        //    carList[i].transform.position = carSpawnPosition;
        //    Car car = carList[i].GetComponent<Car>();
        //    car.carPosition = carSpawnPosition;
        //    car.isAlive = true;
        //}
    }
}
