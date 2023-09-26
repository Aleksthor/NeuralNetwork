using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimulationController : MonoBehaviour
{
    public static SimulationController instance;
    [SerializeField] GameObject prey_prefab;
    [SerializeField] GameObject predator_prefab;
    [SerializeField] GameObject food_prefab;

    List<GameObject> prey_list = new List<GameObject>();
    List<GameObject> predator_list = new List<GameObject>();
    List<GameObject> food_list = new List<GameObject>();

    [SerializeField] int prey_start_size = 50;
    [SerializeField] int predator_start_size = 50;
    [SerializeField] int food_start_size = 100;

    [SerializeField] int prey_max_size = 800;
    [SerializeField] int predator_max_size = 250;
    [SerializeField] int food_max_size = 1250;

    [SerializeField] TextMeshProUGUI prey_text;
    [SerializeField] TextMeshProUGUI predator_text;
    [SerializeField] TextMeshProUGUI food_text;

    int food_spawn_frequency = 2;
    int frame = 0;


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
        SpawnPopulation();
    }

    // Update is called once per frame
    void Update()
    {
        frame = (frame + 1) % food_spawn_frequency;

        if (frame == 0 && CanSpawnFood())
        {
            Vector3 pos = new Vector3(Random.Range(-250f, 250f), 0, Random.Range(-250f, 250f));
            GameObject go = Instantiate(food_prefab, pos, Quaternion.identity);
            AddFood(go);
        }

    }


    void SpawnPopulation()
    {
        for (int i = 0; i < prey_start_size; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-250f, 250f), 0, Random.Range(-250f, 250f));
            GameObject go = Instantiate(prey_prefab, pos, Quaternion.identity);
            prey_list.Add(go);
        }
        for (int i = 0; i < predator_start_size; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-250f, 250f), 0, Random.Range(-250f, 250f));
            GameObject go = Instantiate(predator_prefab, pos, Quaternion.identity);
            predator_list.Add(go);
        }
        for (int i = 0; i < food_start_size; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-250f, 250f), 0, Random.Range(-250f, 250f));
            GameObject go = Instantiate(food_prefab, pos, Quaternion.identity);
            food_list.Add(go);
        }
    }


    public void RemovePrey(GameObject go)
    {
        Destroy(go);
        for (int i = prey_list.Count - 1; i >= 0; i--)
        {
            if (prey_list[i] == null)
            {
                prey_list.RemoveAt(i);
            }
        }
    }
    public void AddPrey(GameObject go)
    {
        prey_list.Add(go);
    }

    public bool CanSpawnPrey()
    {
        return prey_list.Count < prey_max_size;
    }



    public void RemovePredator(GameObject go)
    {
        Destroy(go);
        for (int i = predator_list.Count - 1; i >= 0; i--)
        {
            if (predator_list[i] == null)
            {
                predator_list.RemoveAt(i);
            }
        }
    }
    public void AddPredator(GameObject go)
    {
        predator_list.Add(go);
    }

    public bool CanSpawnPredator()
    {
        return predator_list.Count < predator_max_size;
    }


    public void RemoveFood(GameObject go)
    {
        Destroy(go);
        for (int i = food_list.Count - 1; i >= 0; i--)
        {
            if (food_list[i] == null)
            {
                food_list.RemoveAt(i);
            }
        }
    }
    public void AddFood(GameObject go)
    {
        food_list.Add(go);
    }

    public bool CanSpawnFood()
    {
        return food_list.Count < food_max_size;
    }

}
