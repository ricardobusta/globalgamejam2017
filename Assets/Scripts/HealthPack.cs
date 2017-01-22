using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour {

    [Range(0, 0.2f)] public float speed = 0.01f;
    [Range(0, 5)] public int hPIncrease = 1;

    private static string PLANET_TAG = "Planet";
    private static string MOON_TAG = "Moon";
    private Vector3 planetPosition;
    private Transform _tr;

	// Use this for initialization
	void Awake () {
        _tr = GetComponent<Transform>();
        planetPosition = GameManager.Instance().shieldRenderer.gameObject.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        _tr.position = Vector3.Lerp(_tr.position, planetPosition, speed);
    }

    void OnTriggerEnter(Collider other)
    {
        // Wasted
        if (other.CompareTag(PLANET_TAG))
        {
            Destroy(gameObject);
        }

        // Increases HP
        else if (other.CompareTag(MOON_TAG))
        {
            if (GameManager.Instance().playerHealth < GameManager.Instance().playerMaxHealth)
                GameManager.Instance().Damage(-hPIncrease);

            GameManager.Instance().AwardPoints(5);
            Destroy(gameObject);
        }
    }
}
