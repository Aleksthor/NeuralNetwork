using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : MonoBehaviour
{

    [SerializeField] List<GameObject> trees_To_Spawn;
    [SerializeField] int number_of_trees = 10;
    [SerializeField] float border_radius = 5;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < number_of_trees; i++)
        {
            var distanceFromMiddle = UnityEngine.Random.Range(0, border_radius);
            // distanceFromMiddle = Mathf.Clamp(
            //     distanceFromMiddle,
            //     lastDistance - maxDistanceVariation,
            //     lastDistance + maxDistanceVariation);

            var angle = i * 360f / number_of_trees;
            var angleInRadians = angle * Mathf.Deg2Rad;
            var x = Mathf.Cos(angleInRadians);
            var z = Mathf.Sin(angleInRadians);
            var offset = new Vector3(x, 0f, z).normalized;
            offset *= distanceFromMiddle;
            Vector3 position = transform.position + offset + new Vector3(0, 20, 0);

            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("Map");
            Ray ray = new Ray(position, new Vector3(0, -1, 0));
            if (Physics.Raycast(position, new Vector3(0, -1, 0), out hit, 200f, mask))
            {
                int random = Random.Range(0, trees_To_Spawn.Count);
                GameObject obj = Instantiate(trees_To_Spawn[random], hit.point, Quaternion.identity, gameObject.transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
