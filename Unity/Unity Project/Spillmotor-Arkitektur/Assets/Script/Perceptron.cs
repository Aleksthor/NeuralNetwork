using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Perceptron 
{
    // Weights
    [SerializeField] List<float> weights = new List<float>();


    // Setup the neuron's weights
    public void Setup()
    {
        weights = new List<float>();
        for (int i = 0; i < 2; i++)
        {
            weights.Add(Random.Range(0.0f, 1.0f));
        }
    }

    // Activation of the neuron, trying to calculate the importance of the inputs and weighte
    public float Activate(List<float> inputs)
    {
        if (inputs.Count != weights.Count)
        {
            Debug.Log("inputs or weights not setup correctly");
            return 0;
        }

        float sum = 0.0f;
        for (int i = 0; i < weights.Count; i++)
        {
            sum += inputs[i] * weights[i];
        }
        return NegPos(sum);
    }


    float Sigmoid(float sum)
    {
        float power = -sum;
        float nevner = 1 + Mathf.Exp(power);
        return 1f / nevner;
    }

    float Relu(float sum)
    {
        if (sum <= 0.0f)
        {
            return 0.0f;
        }

        float power = -sum;
        float nevner = 1 + Mathf.Exp(power);
        return 1f / nevner;
    }

    float NegPos(float sum)
    {
        if (sum > 0f)
        {
            return 1;
        }
        else
        {
            return -1;
        }
      
    }

    //Training Function
    public void Training(List<float> inputs, float guess, float target)
    {
        float error = target - guess;


        // Tune the weights
        for (int i = 0; i < weights.Count; ++i)
        {
            weights[i] += error * inputs[i] * 0.1f;
        }
    }
}
