using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Profiling;

public class AppUsageProfiler : MonoBehaviour
{
    //frames
    private int frameCount;
    private float totalTime;
    private float minFPS = float.MaxValue;

    //ram
    private long maxRamUsage = 0;
    [SerializeField] bool autostart=false;
    bool measure = false;

    private void Start()
    {
        if(autostart) 
        {
            StartCoroutine(Autostart()); 
        }
    }
    private void Update()
    {
        AlternativeStart();
        if (measure)
        {
            //frames
            frameCount++;
            totalTime += Time.unscaledDeltaTime;
            float currentFPS = 1f / Time.unscaledDeltaTime;
            if (currentFPS < minFPS && totalTime > 0.5f)
                minFPS = currentFPS;
            //ram

            long currentRAMUsage = Profiler.GetTotalAllocatedMemoryLong();
            maxRamUsage = (long)Mathf.Max(maxRamUsage, currentRAMUsage);
        }
    }

    IEnumerator Autostart()
    {
        yield return new WaitForSeconds(1f);
        MeasurementStart();
        StartCoroutine(CountMeasurement());
    }

    void AlternativeStart()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            MeasurementStart();
            StartCoroutine(CountMeasurement());
        }
    }

    IEnumerator CountMeasurement()
    {
        //profiler.MeasurementsDisplay("t");
        yield return new WaitForSeconds(1);
        MeasurementsDisplay("t+1");

        yield return new WaitForSeconds(1);
        MeasurementsDisplay("t+2");

        yield return new WaitForSeconds(2);
        MeasurementsDisplay("t+4");

        yield return new WaitForSeconds(4);
        MeasurementsDisplay("t+8");

        yield return new WaitForSeconds(8);
        MeasurementsDisplay("t+16");
    }

    public void MeasurementStart()
    {
        measure = true;
        maxRamUsage = Profiler.GetTotalAllocatedMemoryLong();
    }

    public void MeasurementsDisplay(string Msg)
    {
        float averageFPS = frameCount / totalTime;

        Debug.Log($"{Msg}\nMeasurement: {Msg}\nMinimal FPS: {minFPS}\nAverage FPS: {averageFPS}\nMax RAM Usage (KB): {maxRamUsage/ 1024}");

    }
}

