using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PhysicsCar : MonoBehaviour
{
    [SerializeField] Vector3 velocity = new Vector3();
    [SerializeField] public Vector3 position = new Vector3();
    [SerializeField] Vector3 acceleration = new Vector3();
    Vector3 total_velocity = new Vector3();

    float mass = 25;
    float drag_coefficient = 0.25f;
    float friction_coefficient = 1f;
    float learning_innaccuracy = 0.8f;

    public int current_checkpoint = 0;
    public int current_lap = 0;
    public float time_since_last_checkpoint = 0f;
    public float max_velocity = 25;

    [SerializeField] public float time_lived = 0f;
    [SerializeField] public float fitness = 0f;
    [SerializeField] public bool dead = false;

    public NeuralNetwork brain;

    public bool constant_learning = true;
    List<float[]> all_outputs = new List<float[]>();
    List<float[]> all_targets = new List<float[]>();


    private void Start()
    {
        position = transform.position;

        if (brain == null)
        {
            brain = new NeuralNetwork();
            brain.Setup(5, new List<int>() { 12, 12, 12, 12 }, 4);
        }
        
    }
    void Update()
    {
        if (dead)
        {
            return;
        }
        time_lived += Time.deltaTime;
        time_since_last_checkpoint += Time.deltaTime;   

        if (time_since_last_checkpoint > 15)
        {
            dead = true;
        }

        List<float> inputs = new List<float>();

        int layerMask = 1 << 8;


        RaycastHit forward_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out forward_hit, 20, layerMask))
        {
            inputs.Add(forward_hit.distance);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * forward_hit.distance, Color.red);
        }
        else
        {
            inputs.Add(20);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 20, Color.green);
        }

        RaycastHit forward_right_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection((Vector3.right + Vector3.forward).normalized), out forward_right_hit, 20, layerMask))
        {
            inputs.Add(forward_right_hit.distance);
            Debug.DrawRay(transform.position, transform.TransformDirection((Vector3.right + Vector3.forward).normalized) * forward_right_hit.distance, Color.red);
        }
        else
        {
            inputs.Add(20);
            Debug.DrawRay(transform.position, transform.TransformDirection((Vector3.right + Vector3.forward).normalized) * 20, Color.green);
        }

        RaycastHit forward_left_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection((-Vector3.right + Vector3.forward).normalized), out forward_left_hit, 20, layerMask))
        {
            inputs.Add(forward_left_hit.distance);
            Debug.DrawRay(transform.position, transform.TransformDirection((-Vector3.right + Vector3.forward).normalized) * forward_left_hit.distance, Color.red);
        }
        else
        {
            inputs.Add(20);
            Debug.DrawRay(transform.position, transform.TransformDirection((-Vector3.right + Vector3.forward).normalized) * 20, Color.green);
        }

        RaycastHit right_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out right_hit, 20, layerMask))
        {
            inputs.Add(right_hit.distance);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * right_hit.distance, Color.red);
        }
        else
        {
            inputs.Add(20);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * 20, Color.green);
        }

        RaycastHit left_hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.right), out left_hit, 20))
        {
            inputs.Add(left_hit.distance);
            Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.right) * left_hit.distance, Color.red);
        }
        else
        {
            inputs.Add(20);
            Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.right) * 20, Color.green);
        }


        List<float> outputs = brain.FeedForward(inputs);

        AddForwardInput(outputs[0]); 

        AddBackwardsInput(outputs[1]);

        AddRightInput(outputs[2]);

        AddLeftInput(outputs[3]);


        if (constant_learning)
        {
            List<float> targets = new List<float>();

            float forward_safety = inputs[0] / 20f / learning_innaccuracy;
            float forward_right_safety = inputs[1] / 20f / learning_innaccuracy;
            float forward_left_safety = inputs[2] / 20f / learning_innaccuracy;
            float right_safety = inputs[3] / 20f / learning_innaccuracy;
            float left_safety = inputs[4] / 20f / learning_innaccuracy;


            float forward_target = (forward_safety / 2f) + (forward_right_safety / 4f) + (forward_left_safety / 4);
            float back_target = 1 - forward_target;
            float right_target = (right_safety * 0.5f) + (forward_right_safety * 0.5f);
            float left_target = (left_safety * 0.5f) + (forward_left_safety * 0.5f);


            targets.Add(forward_target);
            targets.Add(back_target);
            targets.Add(right_target);
            targets.Add(left_target);

            brain.BackPropagate(outputs, targets);
        }
        else
        {
            List<float> targets = new List<float>();

            float forward_safety = inputs[0] / 20f / learning_innaccuracy;
            float forward_right_safety = inputs[1] / 20f / learning_innaccuracy;
            float forward_left_safety = inputs[2] / 20f / learning_innaccuracy;
            float right_safety = inputs[3] / 20f / learning_innaccuracy;
            float left_safety = inputs[4] / 20f / learning_innaccuracy;


            float forward_target = (forward_safety / 2f) + (forward_right_safety / 4f) + (forward_left_safety / 4);
            float back_target = 1 - forward_target;
            float right_target = (right_safety * 0.5f) + (forward_right_safety * 0.5f);
            float left_target = (left_safety * 0.5f) + (forward_left_safety * 0.5f);


            targets.Add(forward_target);
            targets.Add(back_target);
            targets.Add(right_target);
            targets.Add(left_target);

            all_outputs.Add(outputs.ToArray());
            all_targets.Add(targets.ToArray());
        }

      



        Move();

        if (velocity.magnitude > 1.5f)
        {
            transform.forward = velocity.normalized;
        }


        Vector3 average_velocity = total_velocity / time_lived;
        fitness = average_velocity.magnitude + (total_velocity.magnitude) + (300 * (current_checkpoint + (current_lap * 15)));


    }

    public void Learn()
    {
        for (int i = 0; i < all_outputs.Count && i < all_targets.Count; i++)
        {
            brain.BackPropagate(all_outputs[i].ToList(), all_targets[i].ToList());
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
;

        position += velocity * Time.deltaTime;
        total_velocity += velocity * Time.deltaTime;
        acceleration = Vector3.zero;

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

    public Vector3 RotateVector(Vector3 vector, float angle)
    {
        return new Vector3(Mathf.Cos(angle) * vector.x - Mathf.Sin(angle) * vector.z, vector.y, Mathf.Sin(angle) * vector.x + Mathf.Cos(angle) * vector.z);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag != "Checkpoint")
        {
            dead = true;
            return;
        }

        if (other.tag == "Checkpoint")
        {
            ColliderIndex index = other.GetComponent<ColliderIndex>();
            time_since_last_checkpoint = 0;

            if (index.index == current_checkpoint - 1)
            {
                dead = true;
                return;
            }
            if (index.index == 15 && current_checkpoint == 1)
            {
                dead = true;
                return;
            }
            if (index.index == 15 && current_checkpoint == 0)
            {
                dead = true;
                return;
            }

            if (index.index == current_checkpoint)
            {
                if (index.index == 15)
                {
                    current_checkpoint = 0;
                    current_lap++;
                }
                else
                {
                    current_checkpoint++;
                }
            }
            
        }


    }
}
