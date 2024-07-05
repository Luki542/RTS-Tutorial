using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    const string hpCanvas = "HpCanvas";
    Slider slider;
    Unit unit;
    Transform cameraTransform;

    [SerializeField]
    Vector3 offset;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        unit = GetComponentInParent<Unit>();
        var canvas = GameObject.FindGameObjectWithTag(hpCanvas);
        if (canvas) transform.SetParent(canvas.transform);
        cameraTransform = Camera.main.transform;
        
    }

    private void Update()
    {
        if(!unit)
        {
            Destroy(gameObject);
            return;
        }

        slider.value = unit.healthPercent;
        transform.position = unit.transform.position + offset;
        transform.LookAt(cameraTransform);

        var rotation = transform.localEulerAngles;
        rotation.y = 180;
        transform.localEulerAngles = rotation;
    }
}
