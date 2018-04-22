
using System.Collections.Generic;
using System;

/// <summary>
/// Neural Network C# (Unsupervised)
/// </summary>
public class NeuralNetwork : IComparable<NeuralNetwork>
{
    private int[] layers; //layers
    private float[][] neurons; //neuron matix
    private float[][][] weights; //weight matrix

    public float Fitness { get; set; }

    /// <summary>
    /// Initilizes and neural network with random weights <br/>
    /// Each parameter is the number of neurons in that layer <br/>   
    /// Format: (input, L1, L2 ... Ln, output)
    /// </summary>
    /// <param name="layers"> Layers to the neural network<br/> 
    /// Format: (input, L1, L2 ... Ln, output)
    /// </param>
    public NeuralNetwork(params int[] layers)
    {   
        if (layers==null) layers= new int[] { 1, 4, 4, 1 }; //if no layers make std 1,4,4,1 brain.
        
        //deep copy of layers of this network 
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        //generate matrix
        InitNeurons();
        InitWeights();
    }

    /// <summary>
    /// Deep copy constructor 
    /// </summary>
    /// <param name="copyNetwork">Network to deep copy</param>
    public NeuralNetwork(NeuralNetwork copyNetwork)
    {
        this.layers = new int[copyNetwork.layers.Length];
        for (int i = 0; i < copyNetwork.layers.Length; i++)
        {
            this.layers[i] = copyNetwork.layers[i];
        }

        InitNeurons();
        InitWeights();
        CopyWeights(copyNetwork.weights);
    }
  
    /// <summary>
    /// Copy the network
    /// </summary>
    /// <param name="copyWeights"></param>
    private void CopyWeights(float[][][] copyWeights)
    {
        //todo: try to optimize this to use Array.Copy it is 1000x faster for larger NN.
        
        Array.Copy(copyWeights,weights,weights.Length);

        //for (int i = 0; i < weights.Length; i++)
        //{
        //    for (int j = 0; j < weights[i].Length; j++)
        //    {
        //        for (int k = 0; k < weights[i][j].Length; k++)
        //        {
        //            weights[i][j][k] = copyWeights[i][j][k];
        //        }
        //    }
        //}
    }

    /// <summary>
    /// Create neuron matrix
    /// </summary>
    private void InitNeurons()
    {
        //Neuron Initilization
        List<float[]> neuronsList = new List<float[]>();

        for (int i = 0; i < layers.Length; i++) //run through all layers
        {
            neuronsList.Add(new float[layers[i]]); //add layer to neuron list
        }

        neurons = neuronsList.ToArray(); //convert list to array
    }

    /// <summary>
    /// Create weights matrix.
    /// </summary>
    private void InitWeights()
    {

        var weightsList = new List<float[][]>(); //weights list which will later will converted into a weights 3D array

        //iterate over all neurons that have a weight connection
        for (int i = 1; i < layers.Length; i++)
        {
            var layerWeightsList = new List<float[]>(); //layer weight list for this current layer (will be converted to 2D array)
            int neuronsInPreviousLayer = layers[i - 1]; 

            //iterate over all neurons in this current layer
            for (int j = 0; j < neurons[i].Length; j++)
            {
                var neuronWeights = new float[neuronsInPreviousLayer]; //neruons weights

                //iterate over all neurons in the previous layer and set the weights randomly between 0.5f and -0.5
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    //give random weights to neuron weights
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f,0.5f);
                }

                layerWeightsList.Add(neuronWeights); //add neuron weights of this current layer to layer weights
            }

            weightsList.Add(layerWeightsList.ToArray()); //add this layers weights converted into 2D array into weights list
        }

        weights = weightsList.ToArray(); //convert to 3D array
    }

    /// <summary>
    /// Feed forward this neural network with a given input array
    /// </summary>
    /// <param name="inputs">Inputs to network</param>
    /// <returns></returns>
    public float[] FeedForward(float[] inputs)
    {
        //Add inputs to the neuron matrix
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        //iterate over all neurons and compute feedforward values 
        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0f;
                for (int k = 0; k < neurons[i-1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k]; //sum off all weights connections of this neuron weight their values in previous layer
                }
                neurons[i][j] = (float)Math.Tanh(value); //Hyperbolic tangent activation
            }
        }
        return neurons[neurons.Length-1]; //return output layer
    }

    /// <summary>
    /// Mutate neural network weights
    /// </summary>
    public void Mutate()
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    float weight = weights[i][j][k];

                    //mutate weight value 
                    float randomNumber = UnityEngine.Random.Range(0f,100f);

                    if (randomNumber <= 2f)
                    { //if 1
                      //flip sign of weight
                        weight *= -1f;
                    }
                    else if (randomNumber <= 4f)
                    { //if 2
                      //pick random weight between -1 and 1
                        weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                    else if (randomNumber <= 6f)
                    { //if 3
                      //randomly increase by 0% to 100%
                        float factor = UnityEngine.Random.Range(0f, 1f) + 1f;
                        weight *= factor;
                    }
                    else if (randomNumber <= 8f)
                    { //if 4
                      //randomly decrease by 0% to 100%
                        float factor = UnityEngine.Random.Range(0f, 1f);
                        weight *= factor;
                    }

                    weights[i][j][k] = weight;
                }
            }
        }
    }

    public void AddFitness(float fit)
    {
        Fitness += fit;
    }

    public void SetFitness(float fit)
    {
        Fitness = fit;
    }

    /// <summary>
    /// Compare two neural networks and sort based on fitness
    /// </summary>
    /// <param name="other">Network to be compared to</param>
    /// <returns></returns>
    public int CompareTo(NeuralNetwork other)
    {
        if (other == null) return 1;
        if (Fitness > other.Fitness) return 1;
        if (Fitness < other.Fitness) return -1;
        return 0; // they can only be equal
    }
}
