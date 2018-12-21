using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {


    const string HP_CANVAS = "HpCanvas";

    [SerializeField]
    Vector3 offset;

    Slider slider;
    Unit unit;
    Transform cameraTransform;

	// Use this for initialization
	private void Awake () {
        slider = GetComponent<Slider>();
        unit = GetComponentInParent<Unit>();
        var canvas = GameObject.FindGameObjectWithTag(HP_CANVAS);
        if (canvas) transform.SetParent(canvas.transform);
        cameraTransform = Camera.main.transform;
	}
	


	// Update is called once per frame
	private void Update () {
		if(!unit)
        {
            Destroy(gameObject);
            return;
        }

        slider.value = unit.HealtPercent;

        transform.position = unit.transform.position + offset;
        transform.LookAt(cameraTransform);//na co ma patrzec
        var rotation = transform.localEulerAngles;
        rotation.y = 180;
        transform.localEulerAngles = rotation;

    }
}
