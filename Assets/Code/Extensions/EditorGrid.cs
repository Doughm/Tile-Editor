using UnityEngine;
using System.Collections;

public class EditorGrid : MonoBehaviour
{
    public float width = 1.0f;
    public float height = 1.0f;
    public Color color = Color.white;
    public bool enableGrid = true;
    private float gridLength = 1000000.0f;


    void OnDrawGizmos()
    {
        if (enableGrid == true)
        {
            Vector3 pos = Camera.current.transform.position;
            Gizmos.color = color;

            for (float y = pos.y - 800.0f; y < pos.y + 800.0f; y += height)
            {
                Gizmos.DrawLine(new Vector3(-gridLength,( Mathf.Floor(y / height) * height) + 0.5f, 0.0f), new Vector3(gridLength, (Mathf.Floor(y / height) * height) + 0.5f, 0.0f));
            }
            for (float x = pos.x - 1200.0f; x < pos.x + 1200.0f; x += width)
            {
                Gizmos.DrawLine(new Vector3((Mathf.Floor(x / width) * width) + 0.5f, -gridLength, 0.0f), new Vector3((Mathf.Floor(x / width) * height) + 0.5f, gridLength, 0.0f));
            }
        }
    }
}
