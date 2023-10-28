using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NetworkVisualizer : MonoBehaviour
{
    [SerializeField] Gradient gradient = new Gradient();    
    NeuralNetwork network = null;
    [SerializeField] GameObject layer;
    [SerializeField] GameObject input_prefab;
    [SerializeField] GameObject hidden_prefab;
    [SerializeField] GameObject output_prefab;
    [SerializeField] Transform parent;


    Transform input_parent;
    List<Transform> hidden_parents = new List<Transform>();
    Transform output_parent;
    public void SetNetwork(NeuralNetwork _network)
    {
        network = _network; 
    }


    // Update is called once per frame
    void Update()
    {
        if (network != null)
        {
            Matrix input_mat = network.InputLayer();
            float[] inputs = new float[input_mat.rows];
            if (inputs.Length == 5)
            {
                inputs = new float[5] { map(input_mat.mat[4][0],-1,1,0,1),
                                        map(input_mat.mat[2][0],-1,1,0,1),
                                        map(input_mat.mat[0][0],-1,1,0,1),
                                        map(input_mat.mat[1][0],-1,1,0,1),
                                        map(input_mat.mat[3][0],-1,1,0,1)};
            }
            else
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    inputs[i] = map(input_mat.mat[i][0], -1, 1, 0, 1);
                }
            }


            List<float[]> hidden = new List<float[]>();

            List<Matrix> hidden_mat = network.HiddenLayers();
            for (int i = 0; i < hidden_mat.Count; i++)
            {
                hidden.Add(new float[hidden_mat[i].rows]);
                for (int j = 0; j < hidden[i].Length; j++)
                {
                    hidden[i][j] = map(hidden_mat[i].mat[j][0],-1,1,0,1);
        }
            }

            Matrix output_mat = network.OutputLayer();
            float[] outputs = new float[output_mat.rows];
            for (int i = 0; i < outputs.Length; i++)
            {
                outputs[i] = map(output_mat.mat[i][0],-1,1,0,1);
        }


            if (input_parent == null)
            {

                input_parent = Instantiate(layer, parent).transform;
                hidden_parents = new List<Transform>();
                for (int i = 0; i < hidden.Count; i++)
                {
                    hidden_parents.Add(Instantiate(layer, parent).transform);
                }
                output_parent = Instantiate(layer, parent).transform;

                for (int i = 0; i < inputs.Length; i++)
                {
                    GameObject go = Instantiate(input_prefab, input_parent);
                    TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
                    text.text = inputs[i].ToString("F1");
                    Image image = go.GetComponentInChildren<Image>();
                    image.color = gradient.Evaluate(inputs[i]);
                }

                for (int n = 0; n < hidden.Count; n++)
                {
                    for (int i = 0; i < hidden[n].Length; i++)
                    {
                        GameObject go = Instantiate(hidden_prefab, hidden_parents[n]);
                        TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
                        text.text = hidden[n][i].ToString("F1");
                        Image image = go.GetComponentInChildren<Image>();
                        image.color = gradient.Evaluate(hidden[n][i]);
                    }
                }

                for (int i = 0; i < outputs.Length; i++)
                {
                    GameObject go = Instantiate(output_prefab, output_parent);
                    TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
                    text.text = outputs[i].ToString("F1");
                    Image image = go.GetComponentInChildren<Image>();
                    image.color = gradient.Evaluate(outputs[i]);
                }
            }
            else
            {

                for (int i = 0; i < inputs.Length; i++)
                {
                    GameObject go = input_parent.GetChild(i).gameObject;
                    TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
                    text.text = inputs[i].ToString("F1");
                    Image image = go.GetComponentInChildren<Image>();
                    image.color = gradient.Evaluate(inputs[i]);
                }

                for (int n = 0; n < hidden.Count; n++)
                {
                    for (int i = 0; i < hidden[n].Length; i++)
                    {
                        GameObject go = hidden_parents[n].GetChild(i).gameObject;
                        TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
                        text.text = hidden[n][i].ToString("F1");
                        Image image = go.GetComponentInChildren<Image>();
                        image.color = gradient.Evaluate(hidden[n][i]);
                    }
                }

                for (int i = 0; i < outputs.Length; i++)
                {
                    GameObject go = output_parent.GetChild(i).gameObject;
                    TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
                    text.text = outputs[i].ToString("F1");
                    Image image = go.GetComponentInChildren<Image>();
                    image.color = gradient.Evaluate(outputs[i]);
                }
            }

        }


    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
