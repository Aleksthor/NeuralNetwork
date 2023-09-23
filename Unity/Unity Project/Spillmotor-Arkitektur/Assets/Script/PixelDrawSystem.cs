using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class PixelDrawSystem : MonoBehaviour
{
    public static PixelDrawSystem instance;
    private Grid grid;
    private int width = 5;
    private int height = 5;
    [SerializeField] GameObject Image;
    private GameObject[][] images;
    private RectTransform parent;
    [SerializeField] Button button;
    [SerializeField] TMP_InputField input_field;


    string path = "Assets/Resources/2/2.txt";



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        grid = new Grid(16, 16);

        images = new GameObject[16][];
        for (int i = 0; i < 16; i++)
        {
            images[i] = new GameObject[16];
        }
        parent = GetComponent<RectTransform>();
    }

    private void Start()
    {
        Setup();
        button.onClick.AddListener(Extract);

    }

    private void Update()
    {
        if (DoesMouseHover())
        {
            
            if (Input.GetMouseButton(0))
            {
                float x = Input.mousePosition.x;
                x -= parent.position.x - (8 * width);
                x = Mathf.RoundToInt(x / 5);

                float y = Input.mousePosition.y;
                y -= parent.position.y - (8 * height);
                y = Mathf.RoundToInt(y / 5);

                grid.ChangeColor((int)y, (int)x, new Color(0.9f, 0.9f, 0.9f));
            }


        }

        Draw();
    }

    void Extract()
    {
        var uniquePath = AssetDatabase.GenerateUniqueAssetPath(path);
        WriteToFile(uniquePath);
    }

    private void Setup()
    {
        for (int i = 0; i < grid.Width(); i++)
        {
            for (int j = 0; j < grid.Height(); j++)
            {
                GameObject image = Instantiate(Image, GetComponent<RectTransform>());
                images[i][j] = image;
                RectTransform rect = image.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector3((j * width) + 2.5f, (i * height) + 2.5f, 0);
            }
        }
    }
    private void Draw()
    {
        for (int i = 0; i < grid.Width(); i++)
        {
            for (int j = 0; j < grid.Height(); j++)
            {
                images[i][j].GetComponent<Image>().color = grid.Color(i,j);
            }
        }
    }

    private bool DoesMouseHover()
    {
        Vector3 pos = UnityEngine.Input.mousePosition;
        Vector3 min_pos = parent.position - new Vector3(8 * width, 8 * height, 0);

        if (pos.x > min_pos.x && pos.x < min_pos.x + 16 * width)
        {
            if (pos.y > min_pos.y && pos.y < min_pos.y + 16 * height)
            {
                return true;
            }
        }

        return false;
    }

    public float[] ExtractImage()
    {
        float[] result = new float[16 * 16];
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                result[(i * 16) + j] = grid.Value(i, j);            
            }
        }
        grid.Clear();
        return result;
    }

    public float[] InterpretTextField()
    {
        switch(input_field.text)
        {
            case "0":
                return new float[10] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            case "1":
                return new float[10] { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 };
            case "2":
                return new float[10] { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
            case "3":
                return new float[10] { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 };
            case "4":
                return new float[10] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 };
            case "5":
                return new float[10] { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 };
            case "6":
                return new float[10] { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 };
            case "7":
                return new float[10] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 };
            case "8":
                return new float[10] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 };
            case "9":
                return new float[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 };
            default:
                return new float[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        }
        
    }

    void WriteToFile(string path)
    {
        StreamWriter writer = new StreamWriter(path, false);
        

        float[] values = ExtractImage();
        List<float> values_list = new List<float>(values);
        int size = (int)Mathf.Sqrt(values_list.Count);
        writer.WriteLine("IMAGE");
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                writer.Write(values[(i*size) + j] + ",");
            }
            writer.Write("\n");
        }


        writer.WriteLine("RESULT");
        float[] targets = InterpretTextField();
        List<float> targets_list = new List<float>(targets);
        for (int i = 0; i < targets_list.Count; i++)
        {
            writer.Write(targets_list[i] + ",");
        }


        writer.Close();

    }


    public List<float[]> ReadFromFile(string path, int image_size)
    {
        List<float[]> result = new List<float[]>();
        result.Add(new float[image_size * image_size]);
        StreamReader reader = new StreamReader(path);

        string header = reader.ReadLine();
        if (header == "IMAGE")
        {
            for (int i = 0; i < image_size; i++)
            {
                string[] values = reader.ReadLine().Split(new char[] { ',' });
                for (int j = 0; j < image_size; j++)
                {

                    float value = float.Parse(values[j]);

                    result[0][(i * image_size) + j] = value;
                }

            }
        }
        result.Add(new float[10]);
        string target = reader.ReadLine();
        if (target == "RESULT")
        {
            string[] values = reader.ReadLine().Split(new char[] { ',' });
            for (int i = 0; i < 10; i++)
            {
                float value = float.Parse(values[i]);

                result[1][i] = value;
            }
        }

        return result;
        
    }


}



