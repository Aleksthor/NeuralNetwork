using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class SimulationController : MonoBehaviour
{
    public static SimulationController instance;
    [SerializeField] GameObject prey_prefab;
    [SerializeField] GameObject predator_prefab;

    public List<GameObject> prey_list = new List<GameObject>();
    public List<GameObject> predator_list = new List<GameObject>();

    [SerializeField] int prey_start_size = 50;
    [SerializeField] int predator_start_size = 50;

    [SerializeField] int prey_max_size = 800;
    [SerializeField] int predator_max_size = 250;


    [SerializeField] int prey_size = 0;
    [SerializeField] int predator_size = 0;

    public ObjectPool object_pool = new ObjectPool();


    [SerializeField] TextMeshProUGUI prey_text;
    [SerializeField] TextMeshProUGUI predator_text;

    float time_lived = 0f;


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
        time_lived += Time.deltaTime;

        if (time_lived > 30f)
        {

            time_lived = 0f;

        }

        for (int i = prey_list.Count - 1; i >= 0; i--)
        {
            if (prey_list[i].GetComponent<Prey>().dead)
            {
                object_pool.DestroyPrey(prey_list[i]);
                prey_list.RemoveAt(i);
            }
        }
        for (int i = predator_list.Count - 1; i >= 0; i--)
        {
            if (predator_list[i].GetComponent<Predator>().dead)
            {
                object_pool.DestroyPredator(predator_list[i]);
                predator_list.RemoveAt(i);
            }
        }

        prey_size = prey_list.Count;
        predator_size = predator_list.Count;
    }


    void SpawnPopulation()
    {
        for (int i = 0; i < prey_max_size; i++)
        {
            GameObject go = Instantiate(prey_prefab, Vector3.zero, Quaternion.identity);
            object_pool.DestroyPrey(go);
        }
        for (int i = 0; i < predator_max_size; i++)
        {
            GameObject go = Instantiate(predator_prefab, Vector3.zero, Quaternion.identity);
            object_pool.DestroyPredator(go);
        }


        for (int i = 0; i < prey_start_size; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-250f, 250f), 0, Random.Range(-250f, 250f));
            GameObject go = object_pool.InstantiatePrey(pos);
            prey_list.Add(go);
        }
        for (int i = 0; i < predator_start_size; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-250f, 250f), 0, Random.Range(-250f, 250f));
            GameObject go = object_pool.InstantiatePredator(pos);
            predator_list.Add(go);
        }
    }

}

[System.Serializable]
public class ObjectPool
{
    Vector3 object_pool_pos = new Vector3(0,-20,0);
    public List<GameObject> disabled_prey = new List<GameObject>();
    public List<GameObject> disabled_predator = new List<GameObject>();

    public void DestroyPrey(GameObject obj)
    {
        disabled_prey.Add(obj);
        obj.transform.position = object_pool_pos;   
        obj.SetActive(false);
    }
    public void DestroyPredator(GameObject obj)
    {
        disabled_predator.Add(obj);
        obj.transform.position = object_pool_pos;
        obj.SetActive(false);
    }

    public bool CanSpawnPrey()
    {
        return disabled_prey.Count > 0;
    }
    public bool CanSpawnPredator()
    {
        return disabled_predator.Count > 0;
    }

    public GameObject InstantiatePrey(Vector3 pos, Prey prey)
    {
        GameObject go = disabled_prey[disabled_prey.Count - 1];
        go.SetActive(true);
        disabled_prey.RemoveAt(disabled_prey.Count - 1);
        go.transform.position = pos;
        go.GetComponent<Prey>().dead = false;
        go.GetComponent<Prey>().brain = prey.brain.Copy();
        go.GetComponent<Prey>().brain.GeneticAlgorithm();
        return go;
    }
    public GameObject InstantiatePredator(Vector3 pos, Predator predator)
    {
        GameObject go = disabled_predator[disabled_predator.Count - 1];
        go.SetActive(true);
        disabled_predator.RemoveAt(disabled_predator.Count - 1);
        go.transform.position = pos;
        go.GetComponent<Predator>().energy = 0;
        go.GetComponent<Predator>().age = 0;
        go.GetComponent<Predator>().time_without_food = 0;
        go.GetComponent<Predator>().dead = false;
        go.GetComponent<Predator>().brain = predator.brain.Copy();
        go.GetComponent<Predator>().brain.GeneticAlgorithm();
        return go;
    }


    public GameObject InstantiatePrey(Vector3 pos)
    {
        GameObject go = disabled_prey[disabled_prey.Count - 1];
        go.SetActive(true);
        disabled_prey.RemoveAt(disabled_prey.Count - 1);
        go.transform.position = pos;
        return go;
    }
    public GameObject InstantiatePredator(Vector3 pos)
    {
        GameObject go = disabled_predator[disabled_predator.Count - 1];
        go.SetActive(true);
        disabled_predator.RemoveAt(disabled_predator.Count - 1);
        go.transform.position = pos;
        return go;
    }
}