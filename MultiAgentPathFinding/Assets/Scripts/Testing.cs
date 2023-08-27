using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private PathVisuals pathVisual;
    private Pathfinding pathfinding;
    private AgentManager agentMgr;
    private int AgentIdx=0;
    private bool placing = true;

    private bool started = false;

    private List<GameObject> targets;
    private List<GameObject> obstacles;

    // Start is called before the first frame update
    private void Start()
    {
        pathfinding = new Pathfinding(20, 10);
        agentMgr = new AgentManager();
        targets = new List<GameObject>();
        obstacles = new List<GameObject>();
    }
    
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(placing)
            {
                agentMgr.AddAgent(GetMouseWorldPos(), pathfinding.GetGrid());
                placing = false;
            }
            else
            {
                agentMgr.SetGoal(GetMouseWorldPos());
                placing = true;
            GameObject target = new GameObject("Target",typeof(Target));
            target.transform.position = GetMouseWorldPos();
            targets.Add(target);
            }
        }
        if(Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = GetMouseWorldPos();
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            pathfinding.GetNode(x, y).SetIsWalkable(false);
            GameObject obstacle = new GameObject("Obstacle", typeof(Obstacle));
            obstacle.transform.position = GetMouseWorldPos();
            obstacles.Add(obstacle);
        }
        if (Input.GetKeyDown("space"))
        {
            agentMgr.FindPaths();
            started = true;
        }
        
        if(agentMgr != null)
        if (!agentMgr.IsMoving() && started)
        {
            foreach (var target in targets)
            {
                Destroy(target);
            }
            foreach (var obstacle in obstacles)
            {
                Destroy(obstacle);
            }
            targets.Clear();
            obstacles.Clear();
            pathfinding.ResetObstacles();
            started = false;
        }
    }
    private Vector3 GetMouseWorldPos()
    {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vec.z = 0f;
        return vec;
    }

    
}
