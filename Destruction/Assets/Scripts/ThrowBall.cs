using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBall : MonoBehaviour
{
    public GameObject ball;
    private Camera cam;
    public float shootSpeed=20f;
    bool cooldownEnd = true;
    public float cooldown = 0.4f;
    public AppUsageProfiler profiler;
    private void Start()
    {
        profiler = GetComponent<AppUsageProfiler>();
        cam = Camera.main;
    }

    void FixedUpdate()
    {
        if (Input.GetButton("Fire1") && cooldownEnd)
        {
            cooldownEnd = false;
            // Calculate the click position in world space
            Vector3 clickPosition = GetClickPosition();
            GameObject newBall = Instantiate(ball, transform.position, Quaternion.identity);
            Vector3 shootDirection = (clickPosition - transform.position).normalized;
            newBall.GetComponent<Rigidbody>().velocity = shootDirection * shootSpeed;
            SetUpProfiler();
        }
    }

    private Vector3 GetClickPosition()
    {
        StartCoroutine(CountCooldown());
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return ray.GetPoint(100f);
    }

    private void SetUpProfiler() {
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

    IEnumerator CountCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        cooldownEnd = true;
    }
}
