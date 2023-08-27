using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite sprite;
    private Vector3 location;
    private Vector3 endLoc;
    public Pathfinding pathfinding;
    public List<Vector3> pathList;
    private int moveIdx=1;
    private bool isMoving = false;
    private float moveTime = 0.2f;

    public float moveSpeed = 3f;

    private float elapsedTime = 0f;

    private AgentManager mgr;

    public void SetPathFinding()
    {
        pathfinding = new Pathfinding(10, 10);
    }

    private void OnMouseDown()
    {
        Debug.Log("Clicked");
    }
    public void SetPathFinding(Grid<PathNode> pathGrid)
    {
        pathfinding = new Pathfinding(pathGrid);
    }

    public List<Vector3> FindPath(Vector3 avoidLoc)
    {
        pathList = pathfinding.FindPath(location, endLoc, avoidLoc);
        return pathList;
    }

    public List<Vector3> FindPath(List<Vector3> avoidLocs)
    {
        pathList = pathfinding.FindPath(location, endLoc, avoidLocs);
        return pathList;
    }

    public void SetLoc(Vector3 start)
    {
       
        location = start;

        gameObject.transform.position = location;
    }

    public void SetMgr(AgentManager agtmgr)
    {
        mgr = agtmgr;
    }
    public void SetGoal(Vector3 goal)
    {
        pathfinding.GetGrid().GetXY(goal, out int x, out int y);
        if(pathfinding.GetNode(x,y).isWalkable)
        {
            endLoc = goal;
        }
       
    }
    public List<Vector3> FindPath()
    {
        pathList=pathfinding.FindPath(location, endLoc);

        return pathList;
    }

    public void SetMoving(bool moving)
    {
        isMoving = moving;
    }

    void Start()
    {
        gameObject.AddComponent<SpriteRenderer>();
        pathList = new List<Vector3>();
        sprite = Resources.Load<Sprite>("Robot");

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 4000;

    }

    // Update is called once per frame
    void Update()
    {
        if (pathList != null && isMoving)
        {
            if (moveIdx >= pathList.Count)
            {
                isMoving = false;
                mgr.agentObj.Remove(gameObject);
                Destroy(gameObject);
                return;
            }

            MoveAlongPath();
        }
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    private void MoveAlongPath()
    {
        elapsedTime += Time.deltaTime;

        float normalizedTime = elapsedTime *moveSpeed;
        normalizedTime = Mathf.Clamp01(normalizedTime);

        Vector3 newPosition = Vector3.Lerp(pathList[moveIdx - 1], pathList[moveIdx], normalizedTime);
        transform.position = newPosition;

        if (normalizedTime >= 1f)
        {
            elapsedTime = 0f;
            moveIdx++;
        }
    }

}
