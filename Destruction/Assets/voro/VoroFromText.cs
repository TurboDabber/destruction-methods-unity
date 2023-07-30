using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ProBuilder;
using static UnityEngine.ParticleSystem;


public class VoroFromText : MonoBehaviour
{
    public int particles = 10;
    // Start is called before the first frame update
    List<Rigidbody> rb = new();
    public Vector3 bounds;
    private bool draw = true;
    public bool standartColor = false;

    void Start()
    {

        string programPath = Application.dataPath + "/voro/voro.exe"; // Replace with the actual path to program.exe
        string arguments = $"{bounds.x} {bounds.y} {bounds.z} {particles}";

        ExecuteRandomCells(programPath, arguments);


        string fileContent = File.ReadAllText(Application.dataPath.Replace('/','\\')+"\\result.txt");
        fileContent = fileContent.Replace("\n", "");

        string[] cellSections = fileContent.Split(new[] { "-c" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string cellSection in cellSections)
        {
            GameObject cellObject = new GameObject("Cell");
            cellObject.transform.SetParent(transform);
            cellObject.transform.position = transform.position;

            Mesh cellMesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> trianglesIndices = new List<int>();
            string[] cellParams = cellSection.Split(new[] { ">>>" }, StringSplitOptions.RemoveEmptyEntries);
            string[] cellCenter = cellParams[0].Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            double.TryParse(cellCenter[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double xCell);
            double.TryParse(cellCenter[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double yCell);
            double.TryParse(cellCenter[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double zCell);
            cellObject.transform.position += new Vector3((float)xCell, (float)yCell, (float)zCell);
            string[] triangles = cellParams[1].Split(new[] { "t:" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string triangle in triangles)
            {
                string[] vertexCoordinates = triangle.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < vertexCoordinates.Length; i++)
                {
                    string[] coordinates = vertexCoordinates[i].Split(',');

                    if (double.TryParse(coordinates[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double xDouble) &&
                        double.TryParse(coordinates[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double yDouble) &&
                        double.TryParse(coordinates[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double zDouble))
                    {
                        float x = (float)xDouble - (float)xCell;
                        float y = (float)yDouble - (float)yCell;
                        float z = (float)zDouble - (float)zCell ;

                        vertices.Add(new Vector3(x, y, z));
                        trianglesIndices.Add(vertices.Count - 1);
                    }
                }
            }

            cellMesh.SetVertices(vertices);
            cellMesh.SetTriangles(trianglesIndices, 0);
            cellMesh.RecalculateNormals();
            var mesh = cellObject.AddComponent<MeshFilter>();
            mesh.mesh = cellMesh;

            MeshRenderer meshRenderer = cellObject.AddComponent<MeshRenderer>();
            Material material = new Material(Shader.Find("Standard"));
            if(!standartColor)
                material.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

            meshRenderer.material = material;

            Rigidbody rigidbody = cellObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true; // Adjust according to your needs
            if (rigidbody != null)
                rb.Add(rigidbody);
            // Add MeshCollider component
            MeshCollider meshCollider = cellObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = cellMesh;
            meshCollider.convex = true; // Ensure convex is set for accurate collision detection
        }
        StartCoroutine(CountSeconds());
    }

    IEnumerator CountSeconds()
    {
        //I am reducing the time left on the counter by 1 second each time.
        yield return new WaitForSeconds(3f);
        draw = false;
        foreach (var r in rb)
        {
            r.isKinematic = false;
        }

    }

    static int ExecuteRandomCells(string programPath, string arguments)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = programPath,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        try
        {
            using (Process process = new Process())
            {
                process.StartInfo = psi;
                process.Start();
                process.WaitForExit();
                // You can read the output of the external program if needed
                string output = process.StandardOutput.ReadToEnd();
                UnityEngine.Debug.Log(output);
                return 1;
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log("Error executing the program: " + ex.Message);
        }
        return 0;
    }

    private void OnDrawGizmos()
    {
        if (draw)
        {
            // Set the Gizmos color to red
            Gizmos.color = Color.red;

            // Calculate the size of the shape in each dimension

            // Calculate the center of the shape
            Vector3 center = transform.position;

            // Draw the shape using Gizmos
            Gizmos.DrawWireCube(center, new Vector3(bounds.x, bounds.y, bounds.z));
        }
    }

}
