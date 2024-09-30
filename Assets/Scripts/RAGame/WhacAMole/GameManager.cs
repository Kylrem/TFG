using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject molePrefab;
    public float initialSpawnInterval = 2f;
    public float initialMoleLifetime = 1.5f;

    //Son variables que varían a medida que el juego avanza, aunque muchas no están en uso porque sus funciones estaban mal, actualizaré la versión correcta
    private float spawnInterval;
    private float moleLifetime;
    private PlayerStats playerStats;
    private Vector3[] spawnPoints;
    private List<GameObject> activeMoles = new List<GameObject>();

    private int molesHit = 0;
    private int molesMissed = 0;
    private int totalMoles = 0;

    private const float minSpawnInterval = 0.5f;
    private const float maxSpawnInterval = 2f;
    private const float minMoleLifetime = 0.5f;
    private const float maxMoleLifetime = 2f;

    private void Start()
    {
        playerStats = PlayerStats.LoadStats();
        spawnInterval = initialSpawnInterval;
        moleLifetime = initialMoleLifetime;
        CalculateSpawnPoints();
        playerStats.StartNewAttempt();
        StartCoroutine(SpawnMolesRoutine());
    }

    // Divide el plano en un 3x3 para generar los spawnPoints de los "topos"
    private void CalculateSpawnPoints()
    {
        Vector3 planeSize = GetComponent<Renderer>().bounds.size;

        spawnPoints = new Vector3[9];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                spawnPoints[i * 3 + j] = new Vector3(
                    transform.position.x + (i - 1) * planeSize.x / 3f,
                    transform.position.y + 0.1f, // Provisional para este prefab
                    transform.position.z + (j - 1) * planeSize.z / 3f
                );
            }
        }
    }

    // Genera topos en el intervalos si no se han llenado todos los espacios de la mesa
    private IEnumerator SpawnMolesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (activeMoles.Count < spawnPoints.Length)
            {
                GenerateRandomMole();
            }
        }
    }

    //Genera un "topo" en una posición aleatoria disponible 
    private void GenerateRandomMole()
    {
        List<int> availableIndices = new List<int>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            bool isOccupied = false;

            foreach (GameObject mole in activeMoles)
            {
                if (mole == null) continue;

                if (Vector3.Distance(mole.transform.position, spawnPoints[i]) < 0.1f)
                {
                    isOccupied = true;
                    break;
                }
            }

            if (!isOccupied)
            {
                availableIndices.Add(i);
            }
        }

        if (availableIndices.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableIndices.Count);
            Vector3 spawnPosition = spawnPoints[availableIndices[randomIndex]];
            GameObject mole = Instantiate(molePrefab, spawnPosition, Quaternion.identity);
            activeMoles.Add(mole);

            totalMoles++;

            MoleManager moleScript = mole.GetComponent<MoleManager>();
            moleScript.Initialize(moleLifetime, () => MoleWhacked(mole));
        }
    }

    // Destruye los "topos" al ser impactados (redundante) y actualiza las estadisticas (hay que refinar la parte de las estadisticas)
    private void MoleWhacked(GameObject mole)
    {
        if (mole == null) return; 

        activeMoles.Remove(mole);
        Destroy(mole);
        molesHit++;

        playerStats.UpdateCurrentAttempt(molesHit, molesMissed, totalMoles, spawnInterval, moleLifetime);
    }

    // Guarda las estadisticas al acabar la sesión
    private void OnApplicationQuit()
    {
        playerStats.FinalizeCurrentAttempt();
        playerStats.SaveStats();
    }
}
