using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class NetworkController : MonoBehaviour
{
    public static NetworkController instance;



    // Neural Network
    NeuralNetwork neuralNetwork;




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
        neuralNetwork.Setup(2, new List<int>() {6,3,6}, 1);

        neuralNetwork.ResizeLayer(1,1);
        neuralNetwork.AddHiddenLayer(3);
        // Example of using single layered network

        for (int i = 0; i < 20000; i++)
        {
            int random = UnityEngine.Random.Range(0, inputs.Count);       
            List<float> output = neuralNetwork.FeedForward(inputs[random]);
            neuralNetwork.BackPropagate(output, targets[random]);
            
        }
        Debug.Log("[0,1] -> " + neuralNetwork.FeedForward(new List<float> { 0, 1 })[0]);
        Debug.Log("[1,0] -> " + neuralNetwork.FeedForward(new List<float> { 1, 0 })[0]);
        Debug.Log("[1,1] -> " + neuralNetwork.FeedForward(new List<float> { 1, 1 })[0]);
        Debug.Log("[0,0] -> " + neuralNetwork.FeedForward(new List<float> { 0, 0 })[0]);



    }

    // Update is called once per frame
    void Update()
    {


    }


    public void RetrieveImageArray()
    {
        float[] image = PixelDrawSystem.instance.ExtractImage();
        float[] number = PixelDrawSystem.instance.InterpretTextField();
    }

    public int PrintGuess(List<float> list)
    {
        float best = 0;
        int index = 0;
        for(int i = 0; i < list.Count; i++)
        {
            if (list[i] > best)
            {
                best = list[i];
                index = i;
            }
        }

        Debug.Log("The guess was " + index);
        return index;
    }
    public int PrintTarget(List<float> list)
    {
        float best = 0;
        int index = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] > best)
            {
                best = list[i];
                index = i;
            }
        }
        Debug.Log("The target was " + index);
        return index;

    }

}



