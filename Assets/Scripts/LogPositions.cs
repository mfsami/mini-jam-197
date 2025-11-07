using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Apple.ReplayKit;
public class LogPositions : MonoBehaviour
{
    List<Vector3> positions = new List<Vector3>();
    public Transform player;
    public Transform ghostPrefab;
    private Transform activeGhost;
    public bool recording = true;
    public Transform originPoint;

    
    public float windowSeconds = 20f; // recording window seconds
    private int replayIndex = -1;
    private int maxFrames;

    void Awake()
    {
        maxFrames = Mathf.CeilToInt(windowSeconds / Time.fixedDeltaTime);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // spawn ghost
            activeGhost = Instantiate(ghostPrefab, originPoint.position, Quaternion.identity);
            recording = false;
            
            player.position = originPoint.position;

            // start from the oldest (play forward in time)
            replayIndex = positions.Count - 1;
        }
    }

    void FixedUpdate()
    {
        if (recording)
        {
            // record newest at the END 
            positions.Insert(0, player.position);

            // cap to last 20s
            if (positions.Count > maxFrames)
                positions.RemoveAt(0);
        }
        else
        {
            // step one frame per physics tick
            if (replayIndex >= 0)
            {
                activeGhost.position = positions[replayIndex];
                replayIndex--;
            }
            else
            {
                // done replaying: clear and resume recording fresh
                positions.Clear();
                recording = true;
            }
        }
    }

}
