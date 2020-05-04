using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SocialPlatforms;
using UnityEngine.AI;

[System.Serializable]
public class AIObjects
{
    //-------->
    //declare variables

    public string AIGroupName {get { return m_aiGroupName; } }
    public GameObject ojbectPrefab { get { return m_prefab; } }
    public int maxAI { get { return m_maxAI; } }
    public int spawnRate { get { return m_spawnRate; } }
    public int spawnAmount { get { return m_maxSpawnAmount; } }
    public bool randomizeStats { get { return m_randomizeStats; } }
    public bool enableSpawner { get { return m_enableSpawner; } }


    [Header("AI Group Stats")]
    [SerializeField]
    private string m_aiGroupName;
    [SerializeField]
    private GameObject m_prefab;
    [SerializeField]
    [Range(0f, 1000f)]
    private int m_maxAI;
    [SerializeField]
    [Range(0f, 200f)]
    private int m_spawnRate;
    [SerializeField]
    [Range(0f, 100f)]
    private int m_maxSpawnAmount;
    [SerializeField]
    private bool m_randomizeStats;
    [SerializeField]
    private bool m_enableSpawner;

    public AIObjects(string Name, GameObject Prefab, int MaxAI, int SpawnRate, int SpawnAmount, bool RandomizeStats)
    {
        this.m_aiGroupName = Name;
        this.m_prefab = Prefab;
        this.m_maxAI = MaxAI;
        this.m_spawnRate = SpawnRate;
        this.m_maxSpawnAmount = SpawnAmount;
        this.m_randomizeStats = RandomizeStats;
    }



}


public class AISpawner : MonoBehaviour
{


    //-------->
    //declare variables

    public List<Transform> Waypoints = new List<Transform>();


    public float spawnTimer { get { return m_SpawnTimer; } }
    public Vector3 spawnArea { get { return m_SpawnArea; } }
    // serliaze the private variables
    [Header("Global Stats")]
    [Range(0f, 600f)]
    [SerializeField]
    private float m_SpawnTimer;
    [SerializeField]
    private Color m_SpawnColor = new Color(1.000f, 0.000f, 0.300f); // use the color for the gizmo
    [SerializeField]
    private Vector3 m_SpawnArea = new Vector3(200f, 10f, 200f);

    // create array from new class
    [Header("AI Groups Settings")]
    public AIObjects[] AIObject = new AIObjects[5];



    // Start is called before the first frame update
    void Start()
    {
        GetWaypoints();
        RandomiseGroups();
        CreateAIGroups();
        InvokeRepeating("SpawnNPC", 0.5f, spawnTimer);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnNPC()
    {
        // loop through all of the AI groups
        for (int i = 0; i < AIObject.Count(); i++)
        {
            // check to make sure spawner is enabled
            if (AIObject[i].enableSpawner && AIObject[i].ojbectPrefab != null)
            {
                // make sure that AI group does not have max NPCs
                GameObject tempGroup = GameObject.Find(AIObject[i].AIGroupName);
                if (tempGroup.GetComponentInChildren<Transform>().childCount < AIObject[i].maxAI)
                {
                    // spawn random number of NPCs from 0 to Max Spawn Amount
                    for (int y = 0; y < Random.Range(0, AIObject[i].spawnAmount); i++)
                    {
                        // get random rotation
                        Quaternion randomRotation = Quaternion.Euler(Random.Range(-20, 20), Random.Range(0, 360), 0);
                        // create spawned game object
                        GameObject tempSpawn;
                        tempSpawn = Instantiate(AIObject[i].ojbectPrefab, RandomPosition(), randomRotation);
                        // put spawned NPC as child of group
                        tempSpawn.transform.parent = tempGroup.transform;
                        // Add the AIMove script and class to the new NPC
                        tempSpawn.AddComponent<AIMove>();
                    }
                }
            }
        }
    }


    // public method for Random Position within the Spawn Area
    public Vector3 RandomPosition()
    {
        // get a random position within our Spawn Area
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnArea.x, spawnArea.x),
            Random.Range(-spawnArea.y, spawnArea.y),
            Random.Range(-spawnArea.z, spawnArea.z)
            );
        randomPosition = transform.TransformPoint(randomPosition * .5f);
        return randomPosition;
    }

    // public method for getting a Random Waypoint
    public Vector3 RandomWaypoint()
    {
        int randomWP = Random.Range(0, (Waypoints.Count - 1));
        Vector3 randomWaypoint = Waypoints[randomWP].transform.position;
        return randomWaypoint;
    }



    // method for putting random values in the AI Group Settings
    void RandomiseGroups()
    {
        //randomise
        for (int i = 0; i < AIObject.Count(); i++)
        {
            if (AIObject[i].randomizeStats == true)
            {
                AIObject[i] = new AIObjects(AIObject[i].AIGroupName, AIObject[i].ojbectPrefab, Random.Range(1, 30), Random.Range(1, 20), Random.Range(1, 10), AIObject[i].randomizeStats);

            }
        }
    }

    // method for creating the empty worldoject groups
    void CreateAIGroups()
    {
        for (int i = 0; i < AIObject.Count(); i++)
        {
            // Empty Game Object to keep our AI in
            GameObject m_AIGroupSpawn;

            // create a new game object
            m_AIGroupSpawn = new GameObject(AIObject[i].AIGroupName);
            m_AIGroupSpawn.transform.parent = this.gameObject.transform;
        }
    }

    void GetWaypoints()
    {
        // list using standard lib
        // look through nested children
        Transform[] wpList = transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < wpList.Length; i++)
        {
            if (wpList[i].tag == "waypoint")
            {
                //add to the list
                Waypoints.Add(wpList[i]);
            }
        }
    }


    // show the gizmos in color
    void OnDrawGizmoSelected()
    {
        Gizmos.color = m_SpawnColor;
        Gizmos.DrawCube(transform.position, spawnArea);
    }
}
