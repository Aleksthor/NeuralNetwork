using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ParticleController : MonoBehaviour
{
    public static ParticleController instance;


    [SerializeField] GameObject particle_prefab;
    List<GameObject> red_particles;
    List<GameObject> green_particles;
    List<GameObject> white_particles;
    List<GameObject> blue_particles;
    [SerializeField] Button try_again;

    [Header("Red Rules")]
    [SerializeField] float red_red = -0.1f;
    [SerializeField] float red_green = -0.34f;
    [SerializeField] float red_white = 0f;
    [SerializeField] float red_blue = 0f;
    [Header("Green Rules")]
    [SerializeField] float green_red = -0.17f;
    [SerializeField] float green_green = -0.32f;
    [SerializeField] float green_white = 0.34f;
    [SerializeField] float green_blue = 0f;
    [Header("White Rules")]
    [SerializeField] float white_red = 0f;
    [SerializeField] float white_green = -0.2f;
    [SerializeField] float white_white = 0.15f;
    [SerializeField] float white_blue = 0f;
    [Header("Blue Rules")]
    [SerializeField] float blue_red = 0f;
    [SerializeField] float blue_green = 0f;
    [SerializeField] float blue_white = 0f;
    [SerializeField] float blue_blue = 0f;


    public float simulation_speed = 5f;


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

        try_again.onClick.AddListener(Respawn);
        red_particles = SpawnParticle(ParticleColor.Red, 200);
        green_particles = SpawnParticle(ParticleColor.Green, 200);
        white_particles = SpawnParticle(ParticleColor.White, 200);
        blue_particles = SpawnParticle(ParticleColor.Blue, 5);


    }

    private void Update()
    {    
        red_red = Mathf.Clamp(red_red, -5f, 5f);
        red_green = Mathf.Clamp(red_green, -5f, 5f);
        red_white = Mathf.Clamp(red_white, -5f, 5f);
        red_blue = Mathf.Clamp(red_blue, -5f, 5f);

        green_red = Mathf.Clamp(green_red, -5f, 5f);
        green_green = Mathf.Clamp(green_green, -5f, 5f);
        green_white = Mathf.Clamp(green_white, -5f, 5f);
        green_blue = Mathf.Clamp(green_blue, -5f, 5f);

        white_red = Mathf.Clamp(white_red, -5f, 5f);
        white_green = Mathf.Clamp(white_green, -5f, 5f);
        white_white = Mathf.Clamp(white_white, -5f, 5f);
        white_blue = Mathf.Clamp(white_blue, -5f, 5f);

        blue_red = Mathf.Clamp(blue_red, -5f, 5f);
        blue_green = Mathf.Clamp(blue_green, -5f, 5f);
        blue_white = Mathf.Clamp(blue_white, -5f, 5f);
        blue_blue = Mathf.Clamp(blue_blue, -5f, 5f);



        if (red_red != 0f)
            Rule(red_particles, red_particles, red_red);
        if (red_green != 0f)
            Rule(red_particles, green_particles, red_green);
        if (red_white != 0f)
            Rule(red_particles, white_particles, red_white);
        if (red_blue != 0f)
            Rule(red_particles, blue_particles, red_blue);
        if (green_red != 0f)
            Rule(green_particles, red_particles, green_red);
        if (green_green != 0f)
            Rule(green_particles, green_particles, green_green);
        if (green_white != 0f)
            Rule(green_particles, white_particles, green_white);
        if (green_blue != 0f)
            Rule(green_particles, blue_particles, green_blue);
        if (white_red != 0f)
            Rule(white_particles, red_particles, white_red);
        if (white_green != 0f)
            Rule(white_particles, green_particles, white_green);
        if (white_white != 0f)
            Rule(white_particles, white_particles, white_white);
        if (white_blue != 0f)
            Rule(white_particles, blue_particles, white_blue);
        if (blue_red != 0f)
            Rule(blue_particles, red_particles, blue_red);
        if (blue_green != 0f)
            Rule(blue_particles, green_particles, blue_green);
        if (blue_white != 0f)
            Rule(blue_particles, white_particles, blue_white);
        if (blue_blue != 0f)
            Rule(blue_particles, blue_particles, blue_blue);
    }

    void Rule(List<GameObject> particles1, List<GameObject> particles2, float g)
    {
        for (int i = 0; i < particles1.Count; i++)
        {
            float fx = 0;
            float fy = 0;
            Particle a = particles1[i].GetComponent<Particle>();
            for (int j = 0; j < particles2.Count; j++)
            {
                Particle b = particles2[j].GetComponent<Particle>();
                float dx = a.position.x - b.position.x;
                float dy = a.position.z - b.position.z;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                if (d > 0 && d < 80)
                {
                    float F = (g * 1) / d;
                    fx += F * dx;
                    fy += F * dy;
                }
            }

            a.velocity.x = (a.velocity.x + fx) * 0.5f * Time.deltaTime * simulation_speed;
            a.velocity.z = (a.velocity.z + fy) * 0.5f * Time.deltaTime * simulation_speed;

            if (a.position.x + a.velocity.x <= -250) { a.velocity.x = 250 + +a.velocity.x; }
            else if (a.position.x + a.velocity.x >= 250) { a.position.x = -249 + a.velocity.x; }
            else if (a.position.z + a.velocity.z <= -250) { a.position.z = 249 + a.velocity.z; }
            else if (a.position.z + a.velocity.z >= 250) { a.position.z = -249 + a.velocity.z; }
            else a.position += a.velocity;

            a.transform.position = a.position;
        }
    }



    public List<GameObject> SpawnParticle(ParticleColor color, int number)
    {
        List<GameObject> result = new List<GameObject>();
        for (int i = 0; i < number; i++)
        {
            GameObject go = Instantiate(particle_prefab, new Vector3(UnityEngine.Random.Range(-250, 250),0, UnityEngine.Random.Range(-250, 250)), Quaternion.identity);
            go.GetComponent<Particle>().SetColor(color);
            go.GetComponent<Particle>().id = i;
            result.Add(go);
        }

        return result;
    }


    public List<GameObject> GetParticles(ParticleColor color)
    {
        switch (color)
        {
            case ParticleColor.Green:
                return green_particles;
            case ParticleColor.Red:
                return red_particles;
            case ParticleColor.White:
                return white_particles;
            case ParticleColor.Blue:
                return blue_particles;
        }
        return new List<GameObject>();
    }

  

    public void Respawn()
    {
        for (int i = red_particles.Count - 1; i >= 0; i--)
        {
            GameObject go = red_particles[i];
            red_particles.RemoveAt(i);
            Destroy(go);
        }
        for (int i = green_particles.Count - 1; i >= 0; i--)
        {
            GameObject go = green_particles[i];
            green_particles.RemoveAt(i);
            Destroy(go);
        }
        for (int i = white_particles.Count - 1; i >= 0; i--)
        {
            GameObject go = white_particles[i];
            white_particles.RemoveAt(i);
            Destroy(go);
        }
        for (int i = blue_particles.Count - 1; i >= 0; i--)
        {
            GameObject go = blue_particles[i];
            blue_particles.RemoveAt(i);
            Destroy(go);
        }


        red_particles = SpawnParticle(ParticleColor.Red, 100);
        green_particles = SpawnParticle(ParticleColor.Green, 100);
        white_particles = SpawnParticle(ParticleColor.White, 100);
        blue_particles = SpawnParticle(ParticleColor.Blue, 100);
    }
}

