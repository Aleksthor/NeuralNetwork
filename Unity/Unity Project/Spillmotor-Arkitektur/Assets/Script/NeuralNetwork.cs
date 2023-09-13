using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class NeuralNetwork 
{

    // We want to make a fully connected network

    // Number of nodes in each layer
    int num_nodes_input = 0;
    int num_nodes_hidden = 0;
    int num_nodes_output = 0;

    float learning_rate = 0.1f;

    // Weights
    Matrix input_hidden_weights;
    List<Matrix> hidden_weights = new List<Matrix>();
    Matrix hidden_output_weights;

    // "Neuron" Layers
    Matrix input_layer;
    List<Matrix> hidden_layers = new List<Matrix>();
    Matrix output_layer;

    // Bias'
    List<Matrix> hidden_biases = new List<Matrix>();
    Matrix output_bias;

    public void Setup(int I, int H, int L, int O)
    {
        num_nodes_hidden = H;
        num_nodes_input = I;
        num_nodes_output = O;

        // Bias'
        hidden_biases = new List<Matrix>();
        for (int i = 0; i < L; i++)
        {
            hidden_biases.Add(new Matrix(H, 1));
            hidden_biases[i].Randomize();
        }
        output_bias = new Matrix(O, 1);
        output_bias.Randomize();

        // Weights
        input_hidden_weights = new Matrix(H, I);
        input_hidden_weights.Randomize();
        hidden_weights = new List<Matrix>();
        for (int i = 1; i < L; i++)
        {
            hidden_weights.Add(new Matrix(H, H));
            hidden_weights[i - 1].Randomize();
        }
        hidden_output_weights = new Matrix(O, H);
        hidden_output_weights.Randomize();


        // Layers
        hidden_layers = new List<Matrix>();
        hidden_layers.Add(new Matrix(H, 1));
        for (int i = 1; i < L; i++)
        {
            hidden_layers.Add(new Matrix(H, 1));
        }

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
        hidden_layers[0] = hidden_layers[0].Sigmoid();

        // Loop through the rest of the hidden layers
        for (int i = 1; i < hidden_layers.Count; i++)
        {
            hidden_layers[i] = new Matrix(Matrix.MatrixProduct(hidden_weights[i - 1].mat, hidden_layers[i - 1].mat));
            hidden_layers[i] = hidden_layers[i].Add(hidden_biases[i]);
            hidden_layers[i] = hidden_layers[i].Sigmoid();
        }

        // Generate the output layer from the hidden layer
        output_layer = new Matrix(Matrix.MatrixProduct(hidden_output_weights.mat, hidden_layers[hidden_layers.Count - 1].mat));
        output_layer = output_layer.Add(output_bias);
        output_layer = output_layer.Sigmoid();

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
        for (int i = hidden_weights.Count - 1; i >= 0; i--)
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
        Matrix output_gradient = output_matrix.SigmoidPrime();
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
            Matrix hiddens_gradient = hidden_layers[i].SigmoidPrime();
            hiddens_gradient = hiddens_gradient.Multiply(hidden_errors[i]);
            hiddens_gradient = hiddens_gradient.Multiply(learning_rate);

            Matrix hiddens_T = hidden_layers[i - 1].Transpose();
            Matrix hidden_weights_delta = new Matrix(Matrix.MatrixProduct(hiddens_gradient.mat, hiddens_T.mat));
            hidden_weights[i - 1] = hidden_weights[i - 1].Add(hidden_weights_delta);
            hidden_biases[i] = hidden_biases[i].Add(hiddens_gradient);
        }
        Matrix hidden_gradient = hidden_layers[0].SigmoidPrime();
        hidden_gradient = hidden_gradient.Multiply(hidden_errors[0]);
        hidden_gradient = hidden_gradient.Multiply(learning_rate);

        Matrix input_T = input_layer.Transpose();
        Matrix input_hidden_weight_delta = new Matrix(Matrix.MatrixProduct(hidden_gradient.mat, input_T.mat));
        input_hidden_weights = input_hidden_weights.Add(input_hidden_weight_delta);
        hidden_biases[0] = hidden_biases[0].Add(hidden_gradient);


    }

    public int GetInputs()
    {
        return num_nodes_input;
    }
    public int GetHidden()
    {
        return num_nodes_hidden;
    }
    public int GetOutput()
    {
        return num_nodes_output;
    }

}
