using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayManager : MonoBehaviour
{
    [SerializeField] Duck beaver;
    [SerializeField] List<Terrain> terrainsList;
   [SerializeField] int initialGrassCount = 5;
   [SerializeField] int horizontalSize;
   [SerializeField] int backViewDistance = -4;
   [SerializeField] int forwardViewDistance = 15;
   [SerializeField, Range(0,1)] float treeProbability;
   
   Dictionary<int,Terrain> activeTerrainDict = new Dictionary<int, Terrain>(20);
    
    [SerializeField] int travelDistance;
    
    public UnityEvent<int, int> OnUpdateTerrainLimit;
   private void Start()
   {
    // create initial Grass
    for (int zPos = backViewDistance; zPos < initialGrassCount; zPos++)
    {
        var terrain = Instantiate(terrainsList[0]);

        terrain.transform.localPosition = new Vector3(0,0,zPos);

        if(terrain is Grass grass)
            grass.SetTreePercentage(zPos < -1 ? 1 : 0);

        terrain.Generate(horizontalSize);
        
        activeTerrainDict[zPos] = terrain;
    }

    // 
    for (int zPos = initialGrassCount; zPos < forwardViewDistance; zPos++)
    {
        SpawnRandomTerrain(zPos);
    }

   }

    private Terrain SpawnRandomTerrain(int zPos)
   {
        Terrain comparatorTerrain = null;
        int randomIndex;
        for (int z = -1; z >= -3; z--)
        {
            var checkPos = zPos + z;
            System.Type comparatorType = comparatorTerrain.GetType();
            System.Type checkType = activeTerrainDict[checkPos].GetType();

            if (comparatorTerrain == null)
            {
                comparatorTerrain = activeTerrainDict[checkPos];
                continue;
            }
            else if (comparatorType != checkType)
            {
                randomIndex = Random.Range(0,terrainsList.Count);
                return SpawnTerrain(terrainsList[randomIndex],zPos);

            }
            else
            {
                continue;
            }
        }

        var CandidateTerrain = new List<Terrain>(terrainsList);
        for (int i = 0; i < CandidateTerrain.Count; i++)
        {
            System.Type comparatorType = comparatorTerrain.GetType();
            System.Type checkType = activeTerrainDict[i].GetType();
            if(comparatorTerrain.GetType() == CandidateTerrain[i].GetType())
            {
                CandidateTerrain.Remove(CandidateTerrain[i]);
                break;
            }
        }

         randomIndex = Random.Range(0,CandidateTerrain.Count);
        return SpawnTerrain(CandidateTerrain[randomIndex],zPos);
   }

   public Terrain SpawnTerrain(Terrain terrain, int zPos)
   {
       terrain = Instantiate(terrain);
         terrain.transform.position = new Vector3(0,0,zPos);
        terrain.Generate(horizontalSize);
        activeTerrainDict[zPos] = terrain;
        return terrain; 
   }


   public void UpdateTravelDistance(Vector3 targetPosition)
   {
    if(targetPosition.z > travelDistance)
    {
        travelDistance = Mathf.CeilToInt(targetPosition.z);
        UpdateTerrain();
    }
   }

   public void UpdateTerrain()
   {
        var destroyPos = travelDistance - 1 + backViewDistance;
        Destroy(activeTerrainDict[destroyPos].gameObject);
        activeTerrainDict.Remove(destroyPos);

        var spawnPosition = travelDistance - 1 + forwardViewDistance;
        SpawnRandomTerrain(spawnPosition);

        OnUpdateTerrainLimit.Invoke(horizontalSize,travelDistance + backViewDistance);
   }
}
