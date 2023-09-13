using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        button.onClick.AddListener(NetworkController.instance.RetrieveImageArray);
    }

    private void Update()
    {
        if (DoesMouseHover())
        {
            
            if (UnityEngine.Input.GetMouseButton(0))
            {
                float x = UnityEngine.Input.mousePosition.x;
                x -= parent.position.x - (8 * width);
                x = Mathf.RoundToInt(x / 5);

                float y = UnityEngine.Input.mousePosition.y;
                y -= parent.position.y - (8 * height);
                y = Mathf.RoundToInt(y / 5);

                grid.ChangeColor((int)y, (int)x, new Color(0.9f, 0.9f, 0.9f));
            }


        }

        Draw();
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

}



