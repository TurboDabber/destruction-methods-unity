using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class ProfilerPlaneDivideManager : MonoBehaviour
{
    public MeshDestroy meshDestroy;
    public AppUsageProfiler profiler;
    // Start is called before the first frame update
    void Awake()
    {

        StartCoroutine(CountSeconds());
    }


    IEnumerator CountSeconds()
    {
        yield return new WaitForSeconds(4f);
        SetUpProfiler();
        yield return new WaitForSeconds(2f);
        meshDestroy.DestroyMesh();

    }
    private void SetUpProfiler()
    {
        profiler.MeasurementStart();
        StartCoroutine(CountMeasurement());
    }

    IEnumerator CountMeasurement()
    {
        //profiler.MeasurementsDisplay("t");
        yield return new WaitForSeconds(1);
        profiler.MeasurementsDisplay("t+1");

        yield return new WaitForSeconds(1);
        profiler.MeasurementsDisplay("t+2");

        yield return new WaitForSeconds(2);
        profiler.MeasurementsDisplay("t+4");

        yield return new WaitForSeconds(4);
        profiler.MeasurementsDisplay("t+8");

        yield return new WaitForSeconds(8);
        profiler.MeasurementsDisplay("t+16");
    }
}
