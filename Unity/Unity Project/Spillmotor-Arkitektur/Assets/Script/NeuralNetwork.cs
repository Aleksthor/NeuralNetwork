using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Windows;



public enum NetworkTrainingModel
{
    Sigmoid,
    Relu
}

public class NeuralNetwork 
{

    // We want to make a fully connected network

    // Number of nodes in each layer
    int num_nodes_input = 0;
    List<int> num_nodes_hidden = new List<int>();
    int num_nodes_output = 0;

    float learning_rate = 0.1f;

    NetworkTrainingModel training_model = NetworkTrainingModel.Sigmoid;

    // Weights
    Matrix input_hidden_weights;
    List<Matrix> hidden_weights = new List<Matrix>();
    Matrix hidden_output_weights;

    // "Neuron" Layers
    Matrix input_layer = new Matrix();
    List<Matrix> hidden_layers = new List<Matrix>();
    Matrix output_layer = new Matrix();

    // Bias'
    List<Matrix> hidden_biases = new List<Matrix>();
    Matrix output_bias;

    public void Setup(int I, List<int> H, int O)
    {
        num_nodes_hidden = H;
        num_nodes_input = I;
        num_nodes_output = O;

        // Bias'
        hidden_biases = new List<Matrix>();
        for (int i = 0; i < H.Count; i++)
        {
            hidden_biases.Add(new Matrix(H[i], 1));
            hidden_biases[i].Randomize();
        }
        output_bias = new Matrix(O, 1);
        output_bias.Randomize();

        // Weights
        input_hidden_weights = new Matrix(H[0], I);
        input_hidden_weights.Randomize();
        hidden_weights = new List<Matrix>();
        for (int i = 1; i < H.Count; i++)
        {
            hidden_weights.Add(new Matrix(H[i], H[i - 1]));
            hidden_weights[i - 1].Randomize();
        }
        hidden_output_weights = new Matrix(O, H[H.Count - 1]);
        hidden_output_weights.Randomize();


        // Layers
        input_layer = new Matrix(I, 1);

        hidden_layers = new List<Matrix>();
        for (int i = 0; i < H.Count; i++)
        { 
            hidden_layers.Add(new Matrix(H[i], 1));
        }

        output_layer = new Matrix(O,1);
    }

    public List<float> FeedForward(List<float> inputs)
    {

        // MULTI LAYERS

        // Get this List of inputs in to a matrix
        float[] input_array = inputs.ToArray();
        Matrix input_matrix = new Matrix(num_nodes_input, 1);
        for (int i = 0; i < input_matrix.rows; i++)
        {
            input_matrix.mat[i][0] = input_array[i];
        }
        input_layer = input_matrix;


        // Calculate the first hidden layer
        hidden_layers[0] = new Matrix(Matrix.MatrixProduct(input_hidden_weights.mat, input_matrix.mat));
        hidden_layers[0] = hidden_layers[0].Add(hidden_biases[0]);
        switch(training_model)
        {
            case NetworkTrainingModel.Sigmoid:
                hidden_layers[0] = hidden_layers[0].Sigmoid();
                break;
            case NetworkTrainingModel.Relu:
                hidden_layers[0] = hidden_layers[0].Relu();
                break;
            default:
                break;
        }

        // Loop through the rest of the hidden layers
        for (int i = 1; i < hidden_layers.Count; i++)
        {
            hidden_layers[i] = new Matrix(Matrix.MatrixProduct(hidden_weights[i - 1].mat, hidden_layers[i - 1].mat));
            hidden_layers[i] = hidden_layers[i].Add(hidden_biases[i]);
            switch (training_model)
            {
                case NetworkTrainingModel.Sigmoid:
                    hidden_layers[i] = hidden_layers[i].Sigmoid();
                    break;
                case NetworkTrainingModel.Relu:
                    hidden_layers[i] = hidden_layers[i].Relu();
                    break;
                default:
                    break;
            }
        }

        // Generate the output layer from the hidden layer
        output_layer = new Matrix(Matrix.MatrixProduct(hidden_output_weights.mat, hidden_layers[hidden_layers.Count - 1].mat));
        output_layer = output_layer.Add(output_bias);
        switch (training_model)
        {
            case NetworkTrainingModel.Sigmoid:
                output_layer = output_layer.Sigmoid();
                break;
            case NetworkTrainingModel.Relu:
                output_layer = output_layer.Relu();
                break;
            default:
                break;
        }

        // Get the matrix into an array outputs
        List<float> outputs = new List<float>();
        for (int i = 0; i < output_layer.rows; i++)
        {
            outputs.Add(output_layer.mat[i][0]);
        }
        return outputs;



    }

    // This function does supervised learning
    public void BackPropagate(List<float> output, List<float> target_outputs)
    {

        // Get this List of inputs in to a matrix
        float[] output_array = output.ToArray();
        Matrix output_matrix = new Matrix(output.Count, 1);
        for (int i = 0; i < output_matrix.rows; i++)
        {
            output_matrix.mat[i][0] = output_array[i];
        }

        // Calculate the output errors
        Matrix output_errors = new Matrix(output.Count, 1);
        for (int i = 0; i < target_outputs.Count && i < output.Count; i++)
        {
            output_errors.mat[i][0] = target_outputs[i] - output[i];
        }

        // Calculate the hidden errors
        Matrix transposed_hidden_output_weights = hidden_output_weights.Transpose();
        List<Matrix> transposed_hidden_weights_temp = new List<Matrix>();
        List<Matrix> transposed_hidden_weights = new List<Matrix>();
        for (int i = hidden_weights.Count - 1; i >= 0; i--)
        {
            transposed_hidden_weights_temp.Add(hidden_weights[i].Transpose());
        }
        for (int i = 0; i < hidden_weights.Count; i++)
        {
            transposed_hidden_weights.Add(transposed_hidden_weights_temp[i]);
        }


        List<Matrix> hidden_errors_temp = new List<Matrix>();
        List<Matrix> hidden_errors = new List<Matrix>();
        hidden_errors_temp.Add(new Matrix(Matrix.MatrixProduct(transposed_hidden_output_weights.mat, output_errors.mat)));
        for (int i = 0; i < transposed_hidden_weights.Count; i++)
        {
            hidden_errors_temp.Add(new Matrix(Matrix.MatrixProduct(transposed_hidden_weights[i].mat, hidden_errors_temp[i].mat)));
        }
        for (int i = hidden_errors_temp.Count - 1; i >= 0; i--)
        {
            hidden_errors.Add(hidden_errors_temp[i]);
        }

        // Changing all the weights in the network based on this delta function
        // Wdelta = lr * Layer2Error * Layer2' * Layer1^T , where layer 1 is earlier in the network than layer 2

        // OUTPUT LAYER
        // Calculate the step direction and size of the "correction" - then add that corrections
        Matrix output_gradient = new Matrix();
        switch (training_model)
        {
            case NetworkTrainingModel.Sigmoid:
                output_gradient = output_matrix.SigmoidPrime();
                break;
            case NetworkTrainingModel.Relu:
                output_gradient = output_matrix.ReluPrime();
                break;
            default:
                break;
        }
        output_gradient = output_gradient.Multiply(output_errors);
        output_gradient = output_gradient.Multiply(learning_rate);

        Matrix hidden_T = hidden_layers[hidden_layers.Count - 1].Transpose();
        Matrix hidden_output_weight_delta = new Matrix(Matrix.MatrixProduct(output_gradient.mat, hidden_T.mat));
        hidden_output_weights = hidden_output_weights.Add(hidden_output_weight_delta);
        output_bias = output_bias.Add(output_gradient);



        // HIDDEN LAYER
        // Calculate the hidden gradient and add the correction
        for (int i = hidden_layers.Count - 1; i >= 1; i--)
        {
            Matrix hiddens_gradient = new Matrix();
            switch (training_model)
            {
                case NetworkTrainingModel.Sigmoid:
                    hiddens_gradient = hidden_layers[i].SigmoidPrime();
                    break;
                case NetworkTrainingModel.Relu:
                    hiddens_gradient = hidden_layers[i].ReluPrime();
                    break;
                default:
                    break;
            }
            hiddens_gradient = hiddens_gradient.Multiply(hidden_errors[i]);
            hiddens_gradient = hiddens_gradient.Multiply(learning_rate);

            Matrix hiddens_T = hidden_layers[i - 1].Transpose();
            Matrix hidden_weights_delta = new Matrix(Matrix.MatrixProduct(hiddens_gradient.mat, hiddens_T.mat));
            hidden_weights[i - 1] = hidden_weights[i - 1].Add(hidden_weights_delta);
            hidden_biases[i] = hidden_biases[i].Add(hiddens_gradient);
        }
        Matrix hidden_gradient = new Matrix();
        switch (training_model)
        {
            case NetworkTrainingModel.Sigmoid:
                hidden_gradient = hidden_layers[0].SigmoidPrime();
                break;
            case NetworkTrainingModel.Relu:
                hidden_gradient = hidden_layers[0].ReluPrime();
                break;
            default:
                break;
        }
        hidden_gradient = hidden_gradient.Multiply(hidden_errors[0]);
        hidden_gradient = hidden_gradient.Multiply(learning_rate);

        Matrix input_T = input_layer.Transpose();
        Matrix input_hidden_weight_delta = new Matrix(Matrix.MatrixProduct(hidden_gradient.mat, input_T.mat));
        input_hidden_weights = input_hidden_weights.Add(input_hidden_weight_delta);
        hidden_biases[0] = hidden_biases[0].Add(hidden_gradient);


    }

    public void ResizeLayer(int nodes_to_add, int layer_index)
    {
        Debug.Log("Resizing layer " + layer_index + "; This layer has " + hidden_layers[layer_index].rows + " amount of rows");

        // Bias'
        if (layer_index < hidden_biases.Count)
        {
            hidden_biases[layer_index].AddNodes(nodes_to_add,0);
        }

        // Fix input weights if hidden is first layer
        if (layer_index == 0)
        {
            input_hidden_weights.AddNodes(nodes_to_add, 0);
        }

        // Fix hidden weights 
        if (layer_index < hidden_weights.Count)
        {

            if (layer_index == 0)
            {
                input_hidden_weights.AddNodes(nodes_to_add, 0);
            }
            else if (layer_index == hidden_layers.Count)
            {
                hidden_output_weights.AddNodes(0, nodes_to_add);
            }
            else
            {
                hidden_weights[layer_index - 1].AddNodes(nodes_to_add, 0);
            }

            hidden_weights[layer_index].AddNodes(0, nodes_to_add);
        }




        // Fix hidden layer 
        if (layer_index < hidden_layers.Count)
        {
            hidden_layers[layer_index].AddNodes(nodes_to_add, 0);
        }


        PrintStructure();

    }

    public void AddHiddenLayer(int size)
    {
        Debug.Log("Adding a layer with size " + size);
        hidden_biases.Add(new Matrix(size, 1));
        hidden_biases[hidden_biases.Count - 1].Randomize();

        hidden_layers.Add(new Matrix(size, 1));
        hidden_layers[hidden_layers.Count - 1].Randomize();

        hidden_weights.Add(new Matrix(size, hidden_layers[hidden_layers.Count - 2].rows));
        hidden_weights[hidden_weights.Count -1].Randomize();

        hidden_output_weights = new Matrix(output_layer.rows, size);
        hidden_output_weights.Randomize();

        PrintStructure();
    }

    public void PrintStructure()
    {
        string text = "Input nodes : " + input_layer.rows + "\n";

        for (int i = 0; i < hidden_layers.Count; i++)
        {
            text += "Hidden Layer " + i + " : " + hidden_layers[i].rows + "\n";
        }
        text += "Output Layer : " + output_layer.rows + "\n";

        Debug.Log(text);
    }



    public void GeneticAlgorithm()
    {
        int random_mutation_chance = UnityEngine.Random.Range(20, 101);
        for (int n = 0; n < hidden_layers.Count; n++) 
        {
            for (int i = 0; i < hidden_layers[n].rows; i++)
            {
                int check = UnityEngine.Random.Range(0, 101);
                if (check > random_mutation_chance)
                {
                    hidden_layers[n].Randomize(i, 0);
                }
            }
        }

        for (int n = 0; n < hidden_weights.Count; n++)
        {
            for (int i = 0; i < hidden_weights[n].rows; i++)
            {

                for (int j = 0; j < hidden_weights[n].cols; j++)
                {
                    int check = UnityEngine.Random.Range(0, 101);
                    if (check > random_mutation_chance)
                    {
                        hidden_weights[n].Randomize(i, j);
                    }
                }
            }
        }

        for (int i = 0; i < input_hidden_weights.rows; i++)
        {

            for (int j = 0; j < input_hidden_weights.cols; j++)
            {
                int check = UnityEngine.Random.Range(0, 101);
                if (check > random_mutation_chance)
                {
                    input_hidden_weights.Randomize(i, j);
                }
            }
        }
        for (int i = 0; i < hidden_output_weights.rows; i++)
        {

            for (int j = 0; j < hidden_output_weights.cols; j++)
            {
                int check = UnityEngine.Random.Range(0, 101);
                if (check > random_mutation_chance)
                {
                    hidden_output_weights.Randomize(i, j);
                }
            }
        }

    }



    public int GetInputs()
    {
        return num_nodes_input;
    }
    public List<int> GetHidden()
    {
        return num_nodes_hidden;
    }
    public int GetOutput()
    {
        return num_nodes_output;
    }
    public NetworkTrainingModel GetTrainingModel()
    {
        return training_model;
    }
    public void SetTrainingModel(NetworkTrainingModel model)
    {
        training_model = model;
    }

}
