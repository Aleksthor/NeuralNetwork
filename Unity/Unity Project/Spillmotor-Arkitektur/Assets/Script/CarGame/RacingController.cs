using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingController : MonoBehaviour
{
    [SerializeField] List<GameObject> ai_cars = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < ai_cars.Count; i++)
        {
            ai_cars[i].GetComponent<PhysicsCar>().brain = new NeuralNetwork();
            ai_cars[i].GetComponent<PhysicsCar>().brain.Setup(5, new List<int>() { 32, 32 }, 2);
            ai_cars[i].GetComponent<PhysicsCar>().brain.ReadFromFile("Assets/SavedBrains/56sec.txt");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
