using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

    public GameObject boomerPrefab;
    public GameObject FoodObj;
    public Text GenerationText;
    public Slider RemTimeSlider;

    // these allow for tweaking from Unity frontend
    public int PopulationSize = 4;
    public float TrainTime = 25f;
    [Space(10)]
    public int[] Layers = new int[] { 1, 10, 10, 1 };
    // --

    private List<Boomerang> boomerangList = null;
    private List<NeuralNetwork> boomerBrainz;
    private bool _leftMouseDown = false;
    private int _generationNumber = 0;  
    private bool _isTraning = false;
    private GameObject EneFolder;

    void TrainTimer()
    {
        _isTraning = false;
        RemTimeSlider.value = RemTimeSlider.maxValue; //reset timer
    }

    /// <summary>
    /// Called on Init.
    /// </summary>
    void Start()
    {
        RemTimeSlider.maxValue = TrainTime;
        RemTimeSlider.value = RemTimeSlider.maxValue;
        EneFolder = new GameObject("Foodz");
    }

    /// <summary>
    /// Called every Tick
    /// </summary>
    void Update ()
    {
        RemTimeSlider.value -= Time.deltaTime;
        if (!_isTraning)                // if not training 
        {
            if (_generationNumber == 0) //if we're starting reset the NNs
            {
                InitBoomerangNeuralNetworks();
            }                           //otherwise breed!
            else
            {
                boomerBrainz.Sort(); //sort by fit worst to best

                for (int badHalf = 0; badHalf < PopulationSize / 2; badHalf++)
                {
                    var goodHalf = badHalf + (PopulationSize/2);
                    //was this the other way around on purpose? yeah a fit of 1 is perfect -1 is horrible
                    //copy the good half over the bad half and mutate it 
                    boomerBrainz[badHalf] = new NeuralNetwork(boomerBrainz[goodHalf]); 
                    boomerBrainz[badHalf].Mutate();
                    //swapped. keep the good half
                    boomerBrainz[goodHalf] = new NeuralNetwork(boomerBrainz[goodHalf]); //todo: matrix reset vs this ? why would it be better?
                    if (_generationNumber < 5) boomerBrainz[goodHalf].Mutate(); //increase early mutations
                    //reset all their fitnesses to 0f
                    boomerBrainz[badHalf].Fitness=0f;
                    boomerBrainz[goodHalf].Fitness=0f; 
                }
            }
           
            _generationNumber++;
            GenerationText.text = "Gen: " +_generationNumber.ToString();

            _isTraning = true;

            Invoke("TrainTimer",TrainTime);  //train for trainTime sec
            CreateBoomerangBodies();
        }

        if (Input.GetMouseButtonDown(0))
        {
            _leftMouseDown = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _leftMouseDown = false;
        }

	    if (_leftMouseDown != true) return;
	    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	    FoodObj.transform.position = mousePosition;
    }

    /// <summary>
    /// Destroys the current Boomerangs, if any
    /// Creates _populationSize new ones
    /// </summary>
    private void CreateBoomerangBodies()
    {
        // if we have any, kill em!
        if (boomerangList != null)
        {
            foreach (var t in boomerangList)
            {
                GameObject.Destroy(t.gameObject);
            }
        }
        // Rise my babies!
        boomerangList = new List<Boomerang>();
        for (int i = 0; i < PopulationSize; i++)
        {
            var boomer = ((GameObject)Instantiate(boomerPrefab)).GetComponent<Boomerang>();
            boomer.Init(boomerBrainz[i],FoodObj.transform);
            boomerangList.Add(boomer);
        }

    }

    /// <summary>
    /// Initializes the Boomerangs' "brains" <br/>
    ///
    /// </summary>
    private void InitBoomerangNeuralNetworks()
    {
        //population must be even
        if (PopulationSize % 2 != 0) PopulationSize++;

        boomerBrainz = new List<NeuralNetwork>();       

        for (int i = 0; i < PopulationSize; i++)
        {
            //var boomerBrain = new NeuralNetwork(1, 10, 4, 1); //1 input and 1 output
            var boomerBrain = new NeuralNetwork(Layers); // this allows for tweaking from Unity frontend
            boomerBrain.Mutate();
            boomerBrainz.Add(boomerBrain);
        }
    }

    /// <summary>
    /// Creates an object from the prefabs/ folder
    /// </summary>
    /// <param name="thing"></param>
    /// <returns></returns>
    public static GameObject CreateEntity(string thing)
    {
        return Instantiate(Resources.Load<GameObject>("prefabs/" + thing));
    }


}

