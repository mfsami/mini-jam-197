using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] PlayerController playerPrefab;
    [SerializeField] public Transform[] spawnPoints;     
    [SerializeField] CameraManager cameraManager;

    public int levelIndex = 0;
    PlayerController currentPlayer;


    private void Start()
    {
        // spawn at level 0 when game starts
        SpawnAt(levelIndex);

        // ensure cam 0 is active
        
    }
    
    public bool IsLastLevel() => levelIndex >= spawnPoints.Length - 1;


    public void AdvanceLevel()
    {
        levelIndex = Mathf.Min(levelIndex + 1, spawnPoints.Length - 1);
        if (currentPlayer != null) Destroy(currentPlayer.gameObject);
        SpawnAt(levelIndex);
        
    }

    void SpawnAt(int idx)
    {
        
        currentPlayer = Instantiate(playerPrefab, spawnPoints[idx].position, Quaternion.identity);
    }
}
