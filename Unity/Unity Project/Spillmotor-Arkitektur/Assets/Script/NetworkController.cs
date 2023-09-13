using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkController : MonoBehaviour
{
    public static NetworkController instance;


    // Neural Network
    NeuralNetwork neuralNetwork;
    NeuralNetwork neuralNetwork2;
    [SerializeField] int inputLayers = 5;
    [SerializeField] int hiddenLayers = 6;
    [SerializeField] int outputLayers = 3;

    [SerializeField] int numHiddenLayers = 3;


    // Test Data Set
    List<List<float>> inputs = new List<List<float>>
    {
        new List<float>{0,1},
        new List<float>{1,0},
        new List<float>{1,1},
        new List<float>{0,0}
    };

    List<List<float>> targets = new List<List<float>>
    {
        new List<float>{1},
        new List<float>{1},
        new List<float>{0},
        new List<float>{0}

    };

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {


        // NEURAL NETWORKS DECLARATION
        neuralNetwork = new NeuralNetwork();
        neuralNetwork.Setup(2, 6, 3, 1);

        neuralNetwork2 = new NeuralNetwork();
        neuralNetwork2.Setup(16*16, 12, 2, 10);

        // Example of using single layered network

        for (int i = 0; i < 50000; i++)
        {
            int random = Random.Range(0, inputs.Count);       
            List<float> output = neuralNetwork.FeedForward(inputs[random]);
            neuralNetwork.BackPropagate(output, targets[random]);
            
        }
        Debug.Log("[0,1] -> " + neuralNetwork.FeedForward(new List<float> { 0, 1 })[0]);
        Debug.Log("[1,0] -> " + neuralNetwork.FeedForward(new List<float> { 1, 0 })[0]);
        Debug.Log("[1,1] -> " + neuralNetwork.FeedForward(new List<float> { 1, 1 })[0]);
        Debug.Log("[0,0] -> " + neuralNetwork.FeedForward(new List<float> { 0, 0 })[0]);


        // Example of using multi layered network
        //input = new List<float> { 1, 0};
        //List<float> multiLayerOutput = neuralNetwork2.FeedForward(input);
        //neuralNetwork2.BackPropagate(output, output); - CANT DO THIS YET



    }

    // Update is called once per frame
    void Update()
    {

    }


    public void RetrieveImageArray()
    {
        float[] image = PixelDrawSystem.instance.ExtractImage();
        float[] number = PixelDrawSystem.instance.InterpretTextField();

        List<float> input = new List<float>(image);
        List<float> target = new List<float>(number);

        List<float> output = neuralNetwork2.FeedForward(input);

        int guess = 0;
        float best = 0f;

        for (int i = 0; i < output.Count; i++)
        {
            if (output[i] > best)
            {
                best = output[i];
                guess = i;
            }
        }
        Debug.Log(output[0] + ", " + output[1] + ", " + output[2] + ", " + output[3] + ", " + output[4] + ", " + output[5] + ", " + output[6] + ", " + output[7] + ", " + output[8] + ", " + output[9]);
        Debug.Log("Machine guesses: " + guess);

        neuralNetwork2.BackPropagate(output,target);

    }
}



