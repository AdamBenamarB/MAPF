using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager
{
    public List<GameObject> agentObj;
    public bool processing = false;

    public AgentManager()
    {
        agentObj = new List<GameObject>();
    }
    public void AddAgent(Vector3 startLoc,Grid<PathNode> grid)
    {
        GameObject agent = new GameObject("Agent");
        agent.AddComponent<Agent>();
        agent.GetComponent<Agent>().SetLoc(startLoc);
        agent.GetComponent<Agent>().SetPathFinding(grid);
        agent.GetComponent<Agent>().SetMgr(this);
        agentObj.Add(agent);
    }

    public void SetGoal(int idx,Vector3 goal)
    {
        agentObj[idx].GetComponent<Agent>().SetGoal(goal);
    }

    public void SetGoal(Vector3 goal)
    {
        int idx = 0;
        if(agentObj.Count>0)
        {
            idx = agentObj.Count - 1;
        }
        agentObj[idx].GetComponent<Agent>().SetGoal(goal);
    }

    public void FindPaths()
    {
        Vector3 avoidPoint = Vector3.zero;
        bool collided = false;
        bool unresolved = true;
        int collidedActorIdx0 = 0;

        int nrOfTries = 0;
        processing = true;

        for (int x = 0; x < agentObj.Count; x++)
        {
            agentObj[x].GetComponent<Agent>().FindPath();
        }

        List<Vector3> avoidPoints = new List<Vector3>();
        while (unresolved && nrOfTries < 20)
        {

            for (int y = 0; y < agentObj.Count; y++)
            {
                List<Vector3> path = agentObj[y].GetComponent<Agent>().pathList;
                if (path != null)
                {
                    Color color = Random.ColorHSV();
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        Debug.DrawLine(path[i], path[i + 1], color, 1f);
                    }
                }

                for (int z = y + 1; z < agentObj.Count; z++)
                {
                    List<Vector3> path2 = agentObj[z].GetComponent<Agent>().pathList;
                    if (path2 != null)
                    {

                        Color color = Random.ColorHSV();
                        for (int i = 0; i < path2.Count - 1; i++)
                        {

                            Debug.DrawLine(path2[i], path2[i + 1], color, 1f);
                        }
                    }

                    int shortest = Mathf.Min(agentObj[y].GetComponent<Agent>().pathList.Count,
                        agentObj[z].GetComponent<Agent>().pathList.Count);

                    for (int a = 0; a < shortest - 1; a++)
                    {
                        Vector3 intersect = Vector3.zero;
                        if (AreIntersecting(agentObj[y].GetComponent<Agent>().pathList[a],
                                agentObj[y].GetComponent<Agent>().pathList[a + 1],
                                agentObj[z].GetComponent<Agent>().pathList[a],
                                agentObj[z].GetComponent<Agent>().pathList[a + 1], out intersect))
                        {
                            if (!avoidPoints.Contains(intersect))
                            {

                                collided = true;

                                avoidPoints.Add(intersect);
                                collidedActorIdx0 = y;
                                GameObject target = new GameObject("Collision", typeof(Collision));
                                target.transform.position = intersect;
                            }
                        }
                    }
                }
            }

            if (!collided)
            {
                unresolved = false;
                for (int i = 0; i < agentObj.Count; i++)
                {
                    agentObj[i].GetComponent<Agent>().SetMoving(true);
                }

                processing = false;
            }
            else
            {
                agentObj[collidedActorIdx0].GetComponent<Agent>().FindPath(avoidPoints);
                collided = false;
                nrOfTries++;
            }

        }

        for (int y = 0; y < agentObj.Count; y++)
        {
            List<Vector3> path = agentObj[y].GetComponent<Agent>().pathList;
            if (path != null)
            {
                Color color = Color.green;
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(path[i], path[i + 1], color, 5f);
                }
            }

            for (int z = y + 1; z < agentObj.Count; z++)
            {
                List<Vector3> path2 = agentObj[z].GetComponent<Agent>().pathList;
                if (path2 != null)
                {

                    Color color = Color.green;
                    for (int i = 0; i < path2.Count - 1; i++)
                    {

                        Debug.DrawLine(path2[i], path2[i + 1], color, 5f);
                    }
                }
            }

        }
    }

    public bool AreIntersecting(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2, out Vector3 intersection)
    {
        intersection = Vector3.zero;
        float dx1 = a2.x - a1.x;
        float dy1 = a2.y - a1.y;
        float dx2 = b2.x - b1.x;
        float dy2 = b2.y - b1.y;

        float determinant = dx1 * dy2 - dy1 * dx2;

        // Check if the lines are parallel (determinant is very close to 0)
        if (Mathf.Abs(determinant) < 1e-6)
        {
            
            return false;
        }

        float s = (1.0f / determinant) * ((b1.x - a1.x) * dy2 - (b1.y - a1.y) * dx2);
        float t = (1.0f / determinant) * ((b1.x - a1.x) * dy1 - (b1.y - a1.y) * dx1);

        // Check if the intersection point lies within the line segments
        if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
        {
            intersection.x = a1.x + s * dx1;
            intersection.y = a1.y + s * dy1;
            return true;

        }

        return false;
    }

    public bool IsMoving()
    {
        bool moving = false;
        foreach (var agent in agentObj)
        {
           moving = agent.GetComponent<Agent>().IsMoving();
        }
        return moving;
    }
}

//while(unresolved && nrOfTries<20)
//{
//    List<Vector3> avoidPoints = new List<Vector3>();

//      for (int y = 0; y < agentObj.Count; y++)
//        {
//            List<Vector3> path = agentObj[y].GetComponent<Agent>().pathList;
//            if (path != null)
//            {
//                for (int i = 0; i < path.Count - 1; i++)
//                {
//                    Debug.DrawLine(path[i], path[i + 1], Color.red, 5f);
//                }
//            }

//        for(int z=y+1; z<agentObj.Count; z++)
//        {
//            List<Vector3> path2 = agentObj[z].GetComponent<Agent>().pathList;
//            if (path2 != null)
//            {
//                for (int i = 0; i < path2.Count - 1; i++)
//                {

//                    Debug.DrawLine(path2[i], path2[i + 1], Color.green, 5f);
//                }
//            }

//            int shortest = Mathf.Min(agentObj[y].GetComponent<Agent>().pathList.Count, agentObj[z].GetComponent<Agent>().pathList.Count);

//            for(int a=0;a<shortest-1;a++)
//            {
//                Vector3 intersect = Vector3.zero;
//                if (AreIntersecting(agentObj[y].GetComponent<Agent>().pathList[a], agentObj[y].GetComponent<Agent>().pathList[a + 1],
//                    agentObj[z].GetComponent<Agent>().pathList[a], agentObj[z].GetComponent<Agent>().pathList[a + 1], out intersect))
//                {
//                    if (!avoidPoints.Contains(intersect))
//                    {

//                        collided = true;

//                        avoidPoints.Add(intersect);
//                        Debug.Log(intersect);
//                        collidedActorIdx0 = y;

//                        GameObject target = new GameObject("Collision", typeof(Collision));
//                        target.transform.position = intersect;
//                    }
//                }
//            }
//        }
//    }
//    if(!collided)
//    {
//        unresolved = false;
//        for(int i=0;i<agentObj.Count;i++)
//        {
//            agentObj[i].GetComponent<Agent>().SetMoving(true);
//        }
//        processing = false;
//    }
//    else
//    {
//        agentObj[collidedActorIdx0].GetComponent<Agent>().FindPath(avoidPoints);
//        collided = false;
//        nrOfTries++;
//     }

//}