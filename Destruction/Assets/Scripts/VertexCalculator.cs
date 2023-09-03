using UnityEngine;

public class VertexCalculator : MonoBehaviour
{
    private void Update()
    {
        // Check if the "P" key is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            CalculateVertices();
        }
    }

    private void CalculateVertices()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        int totalVertices = 0;

        foreach (MeshFilter meshFilter in meshFilters)
        {
            Mesh mesh = meshFilter.mesh;
            totalVertices += mesh.vertexCount;
        }

        Debug.Log("Total Vertices in Child Meshes: " + totalVertices);
    }
}
