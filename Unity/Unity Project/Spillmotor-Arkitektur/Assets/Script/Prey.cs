using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : MonoBehaviour
{
    [SerializeField] GameObject prey_prefab;



    public float max_velocity = 25;
    public float birthing_speed = 6f;
    public float birthing_clock = 0f;

    public NeuralNetwork brain;
    public bool dead = false;

    Rigidbody rb;

    private void Start()
    {
        if (brain == null)
        {
            brain = new NeuralNetwork();
            brain.Setup(8, new List<int>() { 16 }, 2);
        }
        birthing_speed = Random.Range(4f, 10f);
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        birthing_clock += Time.deltaTime;

        if (birthing_clock > birthing_speed)
        {
            birthing_clock = 0f;
            if (SimulationController.instance.object_pool.CanSpawnPrey())
            {
                GameObject go = SimulationController.instance.object_pool.InstantiatePrey(transform.position + new Vector3(Random.Range(-1,1),0, Random.Range(-1, 1)),this);
                SimulationController.instance.prey_list.Add(go);
            }
            birthing_speed = Random.Range(4f, 10f);
        }


        List<float> inputs = new List<float>();



        RaycastHit forward_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out forward_hit, 20))
        {
            inputs.Add(Map(forward_hit.distance, 0, 20, -1f, 1f));

        }
        else
        {
            inputs.Add(1f);
        }

        RaycastHit forward_right_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection((Vector3.right + Vector3.forward).normalized), out forward_right_hit, 20))
        {
            inputs.Add(Map(forward_right_hit.distance, 0, 20, -1f, 1f));

        }
        else
        {
            inputs.Add(1f);
        }

        RaycastHit forward_left_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection((-Vector3.right + Vector3.forward).normalized), out forward_left_hit, 20))
        {
            inputs.Add(Map(forward_left_hit.distance, 0, 20, -1f, 1f));
        }
        else
        {
            inputs.Add(1f);
        }

        RaycastHit right_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out right_hit, 20))
        {
            inputs.Add(Map(right_hit.distance, 0, 20, -1f, 1f));
        }
        else
        {
            inputs.Add(1f);
        }

        RaycastHit left_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.right), out left_hit, 20))
        {
            inputs.Add(Map(left_hit.distance, 0, 20, -1f, 1f));
        }
        else
        {
            inputs.Add(1f);
        }

        RaycastHit backwards_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.forward), out backwards_hit, 20))
        {
            inputs.Add(Map(backwards_hit.distance, 0, 20, -1f, 1f));
        }
        else
        {
            inputs.Add(1f);
        }

        RaycastHit backwards_left_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection((-Vector3.right - Vector3.forward).normalized), out backwards_left_hit, 20))
        {
            inputs.Add(Map(backwards_left_hit.distance, 0, 20, -1f, 1f));
        }
        else
        {
            inputs.Add(1f);
        }

        RaycastHit backwards_right_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection((Vector3.right - Vector3.forward).normalized), out backwards_right_hit, 20))
        {
            inputs.Add(Map(backwards_right_hit.distance, 0, 20, -1f, 1f));
        }
        else
        {
            inputs.Add(1f);
        }

        List<float> outputs = brain.FeedForward(inputs);

        if (outputs[0] >= 0.5f)
        {
            AddForwardInput(Map(outputs[0], 0.5f, 1f, 0f, 1f));
        }
        else
        {
            AddBackwardsInput(Map(outputs[0], 0.5f, 0f, 0f, 1f));
        }
        if (outputs[1] >= 0.5f)
        {
            AddRightInput(Map(outputs[1], 0.5f, 1f, 0f, 1f));
        }
        else
        {
            AddLeftInput(Map(outputs[1], 0.5f, 0f, 0f, 1f));
        }

        if (transform.position.x < -250f)
        {
            rb.MovePosition(new Vector3(250f, transform.position.y, transform.position.z));
        }
        if (transform.position.x > 250f)
        {
            rb.MovePosition(new Vector3(-250f, transform.position.y, transform.position.z));
        }
        if (transform.position.z < -250f)
        {
            rb.MovePosition(new Vector3(transform.position.x, transform.position.y, 250f));
        }
        if (transform.position.z > 250f)
        {
            rb.MovePosition(new Vector3(transform.position.x, transform.position.y, -250f));
        }
        if (rb.velocity.magnitude > 0.01f)
        {
            transform.forward = rb.velocity.normalized;
        }
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 45);

    }

    public void AddForwardInput(float input)
    {
        rb.AddForce(transform.forward * 50 * input);
    }
    public void AddBackwardsInput(float input)
    {
        if (rb.velocity.magnitude < 1)
        {
            return;
        }
        rb.AddForce(-transform.forward * 50 * input);
    }
    public void AddRightInput(float input)
    {
        rb.AddForce(transform.right * 30 * input);
    }
    public void AddLeftInput(float input)
    {
        rb.AddForce(transform.right * 30 * input * -1f);
    }

    public static float Map(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Food")
        {
        }
    }
}


