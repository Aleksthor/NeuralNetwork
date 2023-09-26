using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : MonoBehaviour
{
    [SerializeField] Vector3 velocity = new Vector3();
    [SerializeField] public Vector3 position = new Vector3();
    [SerializeField] Vector3 acceleration = new Vector3();
    [SerializeField] GameObject prey_prefab;

    float mass = 25;
    float drag_coefficient = 0.25f;
    float friction_coefficient = 1f;

    public float max_velocity = 25;

    public NeuralNetwork brain;

    [SerializeField] float energy_left = 0;
    [SerializeField] float energy_to_create_offspring = 700;

    private void Start()
    {
        position = transform.position;
        energy_left = 500;
        if (brain == null)
        {
            brain = new NeuralNetwork();
            brain.Setup(16, new List<int>() { 4, 16 }, 2);
        }
    }

    void Update()
    {
        energy_left -= Time.deltaTime * 5;

        if (energy_left < 0)
        {
            SimulationController.instance.RemovePrey(gameObject);
            Destroy(gameObject);
        }
        if (energy_left > energy_to_create_offspring && SimulationController.instance.CanSpawnPrey())
        {
            energy_left -= 350;
            GameObject go = Instantiate(prey_prefab, transform.position, Quaternion.identity);
            go.GetComponent<Prey>().brain = brain.Copy();
            go.GetComponent<Prey>().brain.GeneticAlgorithm();

            SimulationController.instance.AddPrey(go);
        }


        List<float> inputs = new List<float>();



        RaycastHit forward_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out forward_hit, 20))
        {
            inputs.Add(Map(forward_hit.distance, 0, 20, -1f, 1f));
            switch (forward_hit.collider.tag)
            {
                case "Food":
                    inputs.Add(0);
                    break;
                case "Predator":
                    inputs.Add(1);
                    break;
                case "Prey":
                    inputs.Add(0.5f);
                    break;
                default:
                    inputs.Add(-0.5f);
                    break;
            }
        }
        else
        {
            inputs.Add(1f);
            inputs.Add(-1f);
        }

        RaycastHit forward_right_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection((Vector3.right + Vector3.forward).normalized), out forward_right_hit, 20))
        {
            inputs.Add(Map(forward_right_hit.distance, 0, 20, -1f, 1f));
            switch (forward_right_hit.collider.tag)
            {
                case "Food":
                    inputs.Add(0);
                    break;
                case "Predator":
                    inputs.Add(1);
                    break;
                case "Prey":
                    inputs.Add(0.5f);
                    break;
                default:
                    inputs.Add(-0.5f);
                    break;
            }
        }
        else
        {
            inputs.Add(1f);
            inputs.Add(-1f);
        }

        RaycastHit forward_left_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection((-Vector3.right + Vector3.forward).normalized), out forward_left_hit, 20))
        {
            inputs.Add(Map(forward_left_hit.distance, 0, 20, -1f, 1f));
            switch (forward_left_hit.collider.tag)
            {
                case "Food":
                    inputs.Add(0);
                    break;
                case "Predator":
                    inputs.Add(1);
                    break;
                case "Prey":
                    inputs.Add(0.5f);
                    break;
                default:
                    inputs.Add(-0.5f);
                    break;
            }
        }
        else
        {
            inputs.Add(1f);
            inputs.Add(-1f);
        }

        RaycastHit right_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out right_hit, 20))
        {
            inputs.Add(Map(right_hit.distance, 0, 20, -1f, 1f));
            switch (right_hit.collider.tag)
            {
                case "Food":
                    inputs.Add(0);
                    break;
                case "Predator":
                    inputs.Add(1);
                    break;
                case "Prey":
                    inputs.Add(0.5f);
                    break;
                default:
                    inputs.Add(-0.5f);
                    break;
            }
        }
        else
        {
            inputs.Add(1f);
            inputs.Add(-1f);
        }

        RaycastHit left_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.right), out left_hit, 20))
        {
            inputs.Add(Map(left_hit.distance, 0, 20, -1f, 1f));
            switch (left_hit.collider.tag)
            {
                case "Food":
                    inputs.Add(0);
                    break;
                case "Predator":
                    inputs.Add(1);
                    break;
                case "Prey":
                    inputs.Add(0.5f);
                    break;
                default:
                    inputs.Add(-0.5f);
                    break;
            }
        }
        else
        {
            inputs.Add(1f);
            inputs.Add(-1f);
        }

        RaycastHit backwards_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.forward), out backwards_hit, 20))
        {
            inputs.Add(Map(backwards_hit.distance, 0, 20, -1f, 1f));
            switch (backwards_hit.collider.tag)
            {
                case "Food":
                    inputs.Add(0);
                    break;
                case "Predator":
                    inputs.Add(1);
                    break;
                case "Prey":
                    inputs.Add(0.5f);
                    break;
                default:
                    inputs.Add(-0.5f);
                    break;
            }
        }
        else
        {
            inputs.Add(1f);
            inputs.Add(-1f);
        }

        RaycastHit backwards_left_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection((-Vector3.right - Vector3.forward).normalized), out backwards_left_hit, 20))
        {
            inputs.Add(Map(backwards_left_hit.distance, 0, 20, -1f, 1f));
            switch (backwards_left_hit.collider.tag)
            {
                case "Food":
                    inputs.Add(0);
                    break;
                case "Predator":
                    inputs.Add(1);
                    break;
                case "Prey":
                    inputs.Add(0.5f);
                    break;
                default:
                    inputs.Add(-0.5f);
                    break;
            }
        }
        else
        {
            inputs.Add(1f);
            inputs.Add(-1f);
        }

        RaycastHit backwards_right_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection((Vector3.right - Vector3.forward).normalized), out backwards_right_hit, 20))
        {
            inputs.Add(Map(backwards_right_hit.distance, 0, 20, -1f, 1f));
            switch (backwards_right_hit.collider.tag)
            {
                case "Food":
                    inputs.Add(0);
                    break;
                case "Predator":
                    inputs.Add(1);
                    break;
                case "Prey":
                    inputs.Add(0.5f);
                    break;
                default:
                    inputs.Add(-0.5f);
                    break;
            }
        }
        else
        {
            inputs.Add(1f);
            inputs.Add(-1f);
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


        Move();

        if (velocity.magnitude > 1.5f)
        {
            transform.forward = velocity.normalized;
        }

    }


    public void AddForwardInput(float input)
    {
        Vector3 target = position;
        target += transform.forward * input;
        Vector3 desired_vel = (target - position).normalized * max_velocity;
        Vector3 steering = desired_vel - velocity;
        AddForce(steering * 50 * input);
    }
    public void AddBackwardsInput(float input)
    {
        if (velocity.magnitude < 1)
        {
            return;
        }

        Vector3 target = position;
        target -= transform.forward * input;
        Vector3 desired_vel = (target - position).normalized * max_velocity / 2;
        Vector3 steering = desired_vel - velocity;
        AddForce(steering * 50 * input);
    }
    public void AddRightInput(float input)
    {
        Vector3 target = position;
        float steering_force = 50;
        target += transform.right * velocity.sqrMagnitude * input;

        Vector3 desired_vel = (target - position).normalized * max_velocity / 2;
        Vector3 steering = desired_vel - velocity;
        AddForce(steering * steering_force * input);
    }
    public void AddLeftInput(float input)
    {
        Vector3 target = position;
        float steering_force = 50;
        target -= transform.right * velocity.sqrMagnitude * input;

        Vector3 desired_vel = (target - position).normalized * max_velocity / 2;
        Vector3 steering = desired_vel - velocity;
        AddForce(steering * steering_force * input);
    }

    public void Move()
    {
        //AddFriction();
        velocity += acceleration * Time.deltaTime;
        energy_left -= velocity.magnitude * Time.deltaTime * 0.5f;
        position += velocity * Time.deltaTime;
        acceleration = Vector3.zero;

        if (position.x < -250f)
        {
            position.x = 249f;
        }
        if (position.x > 250f)
        {
            position.x = -249f;
        }
        if (position.z < -250f)
        {
            position.z = 249f;
        }
        if (position.z > 250f)
        {
            position.z = -249f;
        }

        transform.position = position;
    }

    public void AddForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    public void AddDrag()
    {
        Vector3 force = velocity;
        force.Normalize();
        force *= -0.5f;

        float speed = velocity.magnitude;

        force *= speed * drag_coefficient * speed;
        AddForce(force);
    }
    public void AddFriction()
    {
        Vector3 force = velocity;
        force.Normalize();
        force *= -1;

        float normal = mass;

        force *= friction_coefficient * normal;
        AddForce(force);
    }

    public static float Map(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Food")
        {
            SimulationController.instance.RemoveFood(other.gameObject);
            Destroy(other.gameObject);
            energy_left += 350;
        }
    }
}
