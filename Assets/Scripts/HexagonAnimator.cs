using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonAnimator : MonoBehaviour {
    bool _increasingSize = true;
    Material mat;

	// Use this for initialization
	void Start () {
        mat = GetComponent<Renderer>().material;
        mat.color = Color.yellow;
    }
	
	// Update is called once per frame
	void Update () {
        float delta = Time.deltaTime;
        Vector3 angles = transform.eulerAngles;
        angles.z += delta * 50f;
        transform.eulerAngles = angles;    //rotate the HEX

        // make the HEX "breathe"
        Vector3 localScale = transform.localScale;
        switch (_increasingSize)
        {
            case true:
                localScale += new Vector3(delta, delta ,0f);
                if (localScale.x >= 2f)
                {
                    _increasingSize = false;
                }
                break;
            case false:
                localScale -= new Vector3(delta, delta, 0f);
                if (localScale.x <=1f)
                {
                    _increasingSize = true;
                }
                break;
        }

        transform.localScale = localScale;

        
    }
}
