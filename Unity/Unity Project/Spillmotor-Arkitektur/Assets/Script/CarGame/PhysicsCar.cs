using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhysicsCar : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] Vector3 velocity = new Vector3();
    [SerializeField] public Vector3 position = new Vector3();
    Vector3 acceleration = new Vector3();


    [Header("Components")]
    [SerializeField] MeshRenderer mesh_renderer;


    float mass = 25;
    float drag_coefficient = 0.25f;
    float friction_coefficient = 1f;
    float max_velocity = 45;

    [Header("GameLogic")]
    public int current_checkpoint = 0;
    public int current_lap = 0;
    float lap_time = 0f;
    float best_lap_time = 0f;
    [SerializeField] public float time_lived = 0f;
    [SerializeField] public float time_since_last_checkpoint = 0f;
    [SerializeField] public float fitness = 0f;
    [SerializeField] public bool dead = false;
    [SerializeField] public bool isPlayer = false;
    [SerializeField] bool isTraining = true;
    [SerializeField] TextMeshProUGUI number;
    Color color = Color.black;

    float sector_one = 0f;
    float sector_two = 0f;
    float sector_three = 0f;
    float best_sector_one = 0f;
    float best_sector_two = 0f;
    float best_sector_three = 0f;
    float best_lap_sector_one = 0f;
    float best_lap_sector_two = 0f;
    float best_lap_sector_three = 0f;
    float pre_round = 0f;

    public NeuralNetwork brain;
    public bool done = false;

    public float view_distance = 40f;
    public int fitness_mode = 0;
    [SerializeField] int index = 0;
    float show_lap_time = 0f;

    private void Start()
    {
        position = transform.position;
        if (color == Color.black)
        {
            color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
        mesh_renderer.materials[0].color = color;
        if (brain == null)
        {
            brain = new NeuralNetwork();
            brain.Setup(5, new List<int>() { 32, 32 }, 2);
        }
        best_sector_one = float.MaxValue;
        best_sector_two = float.MaxValue;
        best_sector_three = float.MaxValue;
        pre_round = 0f;
    }
    void Update()
    {
        if (pre_round < 0.5f)
        {
            pre_round += Time.deltaTime;
            return;
        }

        if (dead)
        {
            return;
            
        }
        time_lived += Time.deltaTime;
        time_since_last_checkpoint += Time.deltaTime;
        lap_time += Time.deltaTime;

        if (show_lap_time >= 0)
        {
            show_lap_time -= Time.deltaTime;
            if (show_lap_time < 0)
            {
                sector_one = 0;
                sector_two = 0;
                sector_three = 0;
            }
        }
        if (time_lived < 2f)
        {
            number.gameObject.SetActive(false);
        }
        else
        {
            number.gameObject.SetActive(true);
        }

        if (time_since_last_checkpoint > 10f)
        {
            dead = true;
        }

        if (time_lived > 7f && current_checkpoint == 0 && current_lap == 0)
        {
            dead = true;
            return;
        }
        float lap_time_score;

        if (current_lap > 0 && fitness_mode == 0)
        {
            switch (fitness_mode)
            {
                case 0:
                    lap_time_score = best_lap_time == 0 ? 0 : 100f / best_lap_time;
                    fitness = (300 * (current_checkpoint + (current_lap * 15))) + lap_time_score;
                    break;
                case 1:
                    lap_time_score = best_lap_time == 0 ? 0 : 100f / best_lap_time;
                    fitness = lap_time_score;
                    break;
                default:
                    break;
            }
            done = true;
            dead = true;
            return;
        }
        if (current_lap > 2)
        {
            switch (fitness_mode)
            {
                case 0:
                    lap_time_score = best_lap_time == 0 ? 0 : 100f / best_lap_time;
                    fitness = (300 * (current_checkpoint + (current_lap * 15))) + lap_time_score;
                    break;
                case 1:
                    lap_time_score = best_lap_time == 0 ? 0 : 100f / best_lap_time;
                    fitness = lap_time_score;
                    break;
                default:
                    break;
            }
            done = true;
            dead = true;
            return;
        }



        if (!isPlayer)
        {
            List<float> inputs = new List<float>();

            int layerMask = 1 << 8;


            RaycastHit forward_hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position - new Vector3(0, 0.5f, 0), transform.TransformDirection(Vector3.forward), out forward_hit, view_distance, layerMask))
            {
                inputs.Add(Map(forward_hit.distance, 0, view_distance, -1f, 1f));
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * forward_hit.distance, Color.red);
            }
            else
            {
                inputs.Add(1f);
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * view_distance, Color.green);
            }

            RaycastHit forward_right_hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position - new Vector3(0, 0.5f, 0), transform.TransformDirection((Vector3.right + Vector3.forward).normalized), out forward_right_hit, view_distance, layerMask))
            {
                inputs.Add(Map(forward_right_hit.distance, 0, view_distance, -1f, 1f));
                Debug.DrawRay(transform.position, transform.TransformDirection((Vector3.right + Vector3.forward).normalized) * forward_right_hit.distance, Color.red);
            }
            else
            {
                inputs.Add(1f);
                Debug.DrawRay(transform.position, transform.TransformDirection((Vector3.right + Vector3.forward).normalized) * view_distance, Color.green);
            }

            RaycastHit forward_left_hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position - new Vector3(0, 0.5f, 0), transform.TransformDirection((-Vector3.right + Vector3.forward).normalized), out forward_left_hit, view_distance, layerMask))
            {
                inputs.Add(Map(forward_left_hit.distance, 0, view_distance, -1f, 1f));
                Debug.DrawRay(transform.position, transform.TransformDirection((-Vector3.right + Vector3.forward).normalized) * forward_left_hit.distance, Color.red);
            }
            else
            {
                inputs.Add(1f);
                Debug.DrawRay(transform.position, transform.TransformDirection((-Vector3.right + Vector3.forward).normalized) * view_distance, Color.green);
            }

            RaycastHit right_hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position - new Vector3(0, 0.5f, 0), transform.TransformDirection(Vector3.right), out right_hit, view_distance, layerMask))
            {
                inputs.Add(Map(right_hit.distance, 0, view_distance, -1f, 1f));
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * right_hit.distance, Color.red);
            }
            else
            {
                inputs.Add(1f);
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * view_distance, Color.green);
            }

            RaycastHit left_hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position - new Vector3(0, 0.5f, 0), transform.TransformDirection(-Vector3.right), out left_hit, view_distance))
            {
                inputs.Add(Map(left_hit.distance, 0, view_distance, -1f, 1f));
                Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.right) * left_hit.distance, Color.red);
            }
            else
            {
                inputs.Add(1f);
                Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.right) * view_distance, Color.green);
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
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                AddForwardInput(1f);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                AddBackwardsInput(1f);
            }
            else
            {
                AddBackwardsInput(0.1f);
            }
            if (Input.GetKey(KeyCode.D))
            {
                AddRightInput(0.8f);
            }
            if (Input.GetKey(KeyCode.A))
            {
                AddLeftInput(0.8f);
            }
        }

        

     
        Move();

        if (velocity.magnitude > 1.5f)
        {
            transform.forward = velocity.normalized;
        }
        switch (fitness_mode)
        {
            case 0:
                lap_time_score = best_lap_time == 0 ? 0 : 100f / best_lap_time;
                fitness = (300 * (current_checkpoint + (current_lap * 15))) + lap_time_score;
                break;
            case 1:
                lap_time_score = best_lap_time == 0 ? 0 : 100f / best_lap_time;
                fitness = lap_time_score;
                break;
            default:
                break;
        }



    }
    public static float Map(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
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
        position += velocity * Time.deltaTime;
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
        if (dead)
        {
            return;
        }

        if (other.tag != "Checkpoint" && !isPlayer && isTraining)
        {
            time_since_last_checkpoint = 0f;
            dead = true;
            return;
        }

        if (other.tag == "Checkpoint")
        {
            RoadCollider road_collider = other.GetComponent<RoadCollider>();

            if (road_collider.index == current_checkpoint - 1)
            {
                dead = true;
                return;
            }
            if (road_collider.index == 18 && current_checkpoint == 0)
            {
                dead = true;
                return;
            }

            if (road_collider.index == current_checkpoint)
            {
                time_since_last_checkpoint = 0f;
                // Sector one
                if (road_collider.index == 5)
                {
                    sector_one = lap_time;
                    if (sector_one < best_sector_one)
                    {
                        best_sector_one = sector_one;
                    }
                }
                // Sector two
                if (road_collider.index == 13)
                {
                    sector_two = lap_time - sector_one;
                    if (sector_two < best_sector_two)
                    {
                        best_sector_two = sector_two;
                    }
                }
                // Sector three
                if (road_collider.index == 18)
                {
                    sector_three = lap_time - sector_one - sector_two;
                    if (sector_three < best_sector_three)
                    {
                        best_sector_three = sector_three;
                    }
                    if (current_lap == 0) { best_lap_time = lap_time; }
                    else
                    {
                        if (lap_time < best_lap_time) 
                        { 
                            best_lap_time = lap_time;
                            best_lap_sector_one = sector_one;
                            best_lap_sector_two = sector_two;
                            best_lap_sector_three = sector_three;
                        }
                    }
                    lap_time = 0f;
                    show_lap_time = 5f;
                    float lap_time_score;
                    switch (fitness_mode)
                    {
                        case 0:
                            lap_time_score = best_lap_time == 0 ? 0 : 100f / best_lap_time;
                            fitness = (300 * (current_checkpoint + (current_lap * 15))) + lap_time_score;
                            break;
                        case 1:
                            lap_time_score = best_lap_time == 0 ? 0 : 100f / best_lap_time;
                            fitness = lap_time_score;
                            break;
                        default:
                            break;
                    }


                    current_checkpoint = 0;
                    road_collider.AddTime(this, time_lived, current_lap);
                    current_lap++;
                }
                else
                {
                    current_checkpoint++;
                    road_collider.AddTime(this, time_lived, current_lap);
                }

                

            }
            
        }


    }

    public float GetSectorOne()
    {
        return sector_one;
    }
    public float GetSectorTwo()
    {
        return sector_two;
    }
    public float GetSectorThree()
    {
        return sector_three;
    }

    public float GetBestSectorOne()
    {
        return best_sector_one;
    }
    public float GetBestSectorTwo()
    {
        return best_sector_two;
    }
    public float GetBestSectorThree()
    {
        return best_sector_three;
    }
    public float GetBestLapSectorOne()
    {
        return best_lap_sector_one;
    }
    public float GetBestLapSectorTwo()
    {
        return best_lap_sector_two;
    }
    public float GetBestLapSectorThree()
    {
        return best_lap_sector_three;
    }

    public float GetTimeLived()
    {
        return lap_time;
    }


    public void ResetCar()
    {
        pre_round = 0f;
        current_checkpoint = 0;
        current_lap = 0;
        max_velocity = 45;
        lap_time = 0f;
        best_lap_time = 0f;
        time_lived = 0f;
        time_since_last_checkpoint = 0f;
        fitness = 0f;
        dead = false;
        velocity = Vector3.zero;
        acceleration = Vector3.zero;
        sector_one = 0f;
        sector_two = 0f;
        sector_three = 0f;
        done = false;
    }

    public float GetBestLapTime()
    {
        return best_lap_time;
    }
    public void SetIndex(int _index)
    {
        index = _index;
    }
    public Color GetColor()
    {
        return color;
    }
    public int GetIndex()
    {
        return index;
    }
}
