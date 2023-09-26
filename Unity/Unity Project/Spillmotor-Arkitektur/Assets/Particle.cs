using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public enum ParticleColor
{
    Green,
    Red,
    White,
    Blue
}

public class Particle : MonoBehaviour
{
    public ParticleColor color;
    public Vector3 position = new Vector3();
    public Vector3 velocity = new Vector3();

    [SerializeField] Material green;
    [SerializeField] Material red;
    [SerializeField] Material white;
    [SerializeField] Material blue;
    [SerializeField] MeshRenderer mesh;

    public int id = 0;

    private void Start()
    {
        position = transform.position;
    }



    public void SetColor(ParticleColor _color)
    {
        color = _color;
        switch(color)
        {
            case ParticleColor.Green:
                mesh.material = green;
                break;
            case ParticleColor.Red:
                mesh.material = red;
                break;
            case ParticleColor.White:
                mesh.material = white;
                break;
            case ParticleColor.Blue:
                mesh.material = blue;
                break;
        }
    }



}
