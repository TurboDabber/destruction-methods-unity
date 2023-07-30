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

    private void Start()
    {
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
        }
    }

    private Vector3 GetClickPosition()
    {
        StartCoroutine(CountSeconds());
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return ray.GetPoint(100f);
    }

    IEnumerator CountSeconds()
    {
        //I am reducing the time left on the counter by 1 second each time.
        yield return new WaitForSeconds(cooldown);
        cooldownEnd = true;

    }
}
