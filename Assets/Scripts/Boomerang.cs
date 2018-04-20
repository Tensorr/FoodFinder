using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour {
    private bool _initialized = false;
    private Transform hex;

    //public float Angle2Hex0 = 0f;
    //public float Angle2Hex1 = 0f;
    //public float Distance2Hex = 0f;
    //public float BoomHead = 0f;
    //public Vector2 DVector2 = Vector2.up;

    private NeuralNetwork net;
    private Rigidbody2D rBody;
    private Material[] mats;

    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        mats = new Material[transform.childCount];
        for(int i = 0; i < mats.Length; i++)
            mats[i] = transform.GetChild(i).GetComponent<Renderer>().material;
    }

    void FixedUpdate ()
    {
        if (_initialized)
        {
            float distance = Vector2.Distance(transform.position, hex.position); //distance to the HEX

            //Distance2Hex = distance;


            float[] inputs = new float[1];

            inputs[0] = BearingToHex()/180; //-1 to +1

            float[] output = net.FeedForward(inputs);

            rBody.velocity = 2.5f * transform.up;
            rBody.angularVelocity = 500f * output[0];

            //net.AddFitness((1f-Mathf.Abs(inputs[0])));   //the smaller the angle to the HEX the Fitter
            net.AddFitness((1f - distance));  //the closer to the HEX the Fitter

            if (distance > 20f) distance = 20f; //max out distance to 20 
            foreach (Material t in mats)
                t.color = new Color((1f - (distance / 20f)), 0, distance / 20f); //close is red, far is blue

           // Manager.RemTimeSlider.value -= Time.deltaTime;
        }
    }

    public void Init(NeuralNetwork net, Transform hex)
    {
        this.hex = hex;
        this.net = net;
        _initialized = true;
    }

    /// <summary>
    /// Returns the deg to turn towards the HEX <br/>
    /// CCW is + , CW is -
    /// </summary>
    /// <returns>Degrees heading towards HEX</returns>
    private float BearingToHex()
    {
        return BearingToPoint(hex.position);
    }

    /// <summary>
    /// Returns the deg to turn towards the Vector3 parameter <br/>
    /// CCW is + , CW is -
    /// </summary>
    /// <returns>Degrees heading towards pointVector3</returns>
    private float BearingToPoint(Vector3 pointVector3)
    {
        float heading = (transform.eulerAngles.z + 90f) % 360f; // Current boomerang heading ccw from x+ axis 
        if (heading < 0f) heading += 360f;     //turn acute negative angles into positive obtuse angles 

        Vector2 deltaVector = (pointVector3 - transform.position);
        var ang2Hex = Mathf.Atan2(deltaVector.y, deltaVector.x) * Mathf.Rad2Deg; //angle of Hex ccw from X+ axis 
        if (ang2Hex < 0f) ang2Hex += 360f;     //turn acute negative angles into positive obtuse angles

        //DVector2 = deltaVector;
        //Angle2Hex0 = ang2Hex;

        ang2Hex -= heading;
        if (ang2Hex >= 180f) // do we need to turn CW-or CCW+ ?
            ang2Hex = (360 - ang2Hex) * -1f;

        //BoomHead = heading;
        //Angle2Hex1 = ang2Hex;

        return ang2Hex;
    }

}