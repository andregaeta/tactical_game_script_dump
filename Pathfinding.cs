using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding 
{

    private PathNode[,] NodeGrid;
    private int height;
    private int width;
    private float maxHeightDifference;
    private float maxHeightToAttackFrom;

    private List<PathNode> openList;
    private List<PathNode> closedList;
    private List<PathNode> foundList;
    public Pathfinding()
    {
        this.height = GridManager.Instance.Grid.GetLength(0);
        this.width = GridManager.Instance.Grid.GetLength(1);
        maxHeightDifference = 0.5f;
        maxHeightToAttackFrom = 2f;
        NodeGrid = new PathNode[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                NodeGrid[i, j] = new PathNode();
                GridTile gridTile = GridManager.Instance.Grid[i, j].GetComponent<GridTile>();
                NodeGrid[i, j].i = i;
                NodeGrid[i, j].j = j;
                NodeGrid[i, j].height = gridTile.height + gridTile.yOffset;
                NodeGrid[i, j].isWalkable = gridTile.isWalkable;
                if (gridTile.occupiedBy == null)
                    NodeGrid[i, j].isOccupied = false;
                else
                    NodeGrid[i, j].isOccupied = true;
                NodeGrid[i, j].steps = 0;
                NodeGrid[i, j].jumps = 0;
            }
        }
    }

    /*
    public List<Vector3> FindVectorPath(GameObject unit, GameObject tile)
    {
        GridObject unitPos = unit.GetComponent<GridObject>();
        GridTile tilePos = tile.GetComponent<GridTile>();
        List<GameObject> objectPath = FindTilePath(unitPos.i, unitPos.j, tilePos.i, tilePos.j);
        if (objectPath == null)
            return null;
        List<Vector3> vectorPath = new List<Vector3>();
        foreach(GameObject tileObject in objectPath)
        {
            vectorPath.Add(GridManager.Instance.CalculatePositionAtTile(unit, tileObject));
        }
        return vectorPath;
    }
    */
    public List<Vector3> FindVectorPath(GameObject unit, List<GameObject> objectPath)
    {
        if (objectPath == null)
            return null;
        List<Vector3> vectorPath = new List<Vector3>();
        foreach (GameObject tileObject in objectPath)
        {
            vectorPath.Add(GridManager.Instance.CalculatePositionAtTile(unit, tileObject));
        }
        return vectorPath;
    }

    public List<GameObject> FindTilePath(GameObject unit, GameObject tile)
    {
        GridObject unitPos = unit.GetComponent<GridObject>();
        GridTile tilePos = tile.GetComponent<GridTile>();
        List<PathNode> nodePath = FindNodePath(unitPos.i, unitPos.j, tilePos.i, tilePos.j);
        if (nodePath == null)
            return null;
        List<GameObject> tilePath = new List<GameObject>();
        foreach(PathNode node in nodePath)
        {
            tilePath.Add(GridManager.Instance.Grid[node.i, node.j]);
        }
        return tilePath;
    }

    public List<GameObject> FindTilesAround(GridTile tile, int distance)
    {
        List<PathNode> nodesAround = FindNodesAround(tile, distance);
        if (nodesAround == null)
            return null;
        List<GameObject> tilesAround = new List<GameObject>();
        foreach (PathNode node in nodesAround)
        {
            tilesAround.Add(GridManager.Instance.Grid[node.i, node.j]);
        }
        return tilesAround;
    }

    public List<GameObject> FindTilesAroundToWalkTo(GridTile tile, int distance)
    {
        List<PathNode> nodesAround = FindNodesAroundToWalkTo(tile, distance);
        if (nodesAround == null)
            return null;
        List<GameObject> tilesAround = new List<GameObject>();
        foreach(PathNode node in nodesAround)
        {
            tilesAround.Add(GridManager.Instance.Grid[node.i, node.j]);
        }
        return tilesAround;
    }

    public List<GameObject> FindTilesAroundSimple(GridTile tile, int distance)
    {
        List<PathNode> nodesAround = FindNodesAroundSimple(tile, distance);
        if (nodesAround == null)
            return null;
        List<GameObject> tilesAround = new List<GameObject>();
        foreach (PathNode node in nodesAround)
        {
            tilesAround.Add(GridManager.Instance.Grid[node.i, node.j]);
        }
        return tilesAround;
    }

    public List<PathNode> FindNodesAround(GridTile tile, int distance)
    {//checks walkable and height
        PathNode tileNode = NodeGrid[tile.i, tile.j];
        openList = new List<PathNode>();
        closedList = new List<PathNode>();
        foundList = new List<PathNode>();
        openList.Add(tileNode);
        foundList.Add(tileNode);

        while (openList.Count > 0)
        {
            PathNode currentNode = openList[0];
            if (currentNode.steps >= distance)
            {
                openList.Remove(currentNode);
                closedList.Add(currentNode);
                continue;
            }
            List<PathNode> adjacentNodes = GetAdjacentNodes(currentNode);
            foreach (PathNode adjacentNode in adjacentNodes)
            {
                if (!foundList.Contains(adjacentNode))
                {
                    foundList.Add(adjacentNode);
                    adjacentNode.steps = currentNode.steps + 1;
                }
                else if (foundList.Contains(adjacentNode))
                {
                    if (adjacentNode.steps > currentNode.steps)
                    {
                        adjacentNode.steps = currentNode.steps + 1;
                    }
                }
                if (closedList.Contains(adjacentNode))
                    continue;
                if (!openList.Contains(adjacentNode))
                    openList.Add(adjacentNode);

            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);
        }

        return foundList;
    }

    public List<PathNode> FindNodesAroundToWalkTo(GridTile tile, int distance)
    {//checks for height, walkable and occupied
        PathNode tileNode = NodeGrid[tile.i, tile.j];
        openList = new List<PathNode>();
        closedList = new List<PathNode>();
        foundList = new List<PathNode>();
        openList.Add(tileNode);
        foundList.Add(tileNode);

        while(openList.Count > 0)
        {
            PathNode currentNode = openList[0];
            if (currentNode.steps >= distance)
            {
                openList.Remove(currentNode);
                closedList.Add(currentNode);
                continue;
            }
            List<PathNode> adjacentNodes = GetAdjacentNodesToWalkInto(currentNode);
            foreach(PathNode adjacentNode in adjacentNodes)
            {
                if (!foundList.Contains(adjacentNode))
                {
                    foundList.Add(adjacentNode);
                    adjacentNode.steps = currentNode.steps + 1;
                }
                else if (foundList.Contains(adjacentNode))
                {
                    if(adjacentNode.steps > currentNode.steps)
                    {
                        adjacentNode.steps = currentNode.steps + 1;
                    }
                }
                if (closedList.Contains(adjacentNode))
                    continue;
                if (!openList.Contains(adjacentNode))
                    openList.Add(adjacentNode);

            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);
        }

        return foundList;
    }

    public List<PathNode> FindNodesAroundSimple(GridTile tile, int distance)
    { //checks nothing
        PathNode tileNode = NodeGrid[tile.i, tile.j];
        openList = new List<PathNode>();
        closedList = new List<PathNode>();
        foundList = new List<PathNode>();
        openList.Add(tileNode);
        foundList.Add(tileNode);

        while (openList.Count > 0)
        {
            PathNode currentNode = openList[0];
            if (currentNode.steps >= distance)
            {
                openList.Remove(currentNode);
                closedList.Add(currentNode);
                continue;
            }
            List<PathNode> adjacentNodes = GetAdjacentNodesSimple(currentNode);
            foreach (PathNode adjacentNode in adjacentNodes)
            {
                if (!foundList.Contains(adjacentNode))
                {
                    foundList.Add(adjacentNode);
                    adjacentNode.steps = currentNode.steps + 1;
                }
                else if (foundList.Contains(adjacentNode))
                {
                    if (adjacentNode.steps > currentNode.steps)
                    {
                        adjacentNode.steps = currentNode.steps + 1;
                    }
                }
                if (closedList.Contains(adjacentNode))
                    continue;
                if (!openList.Contains(adjacentNode))
                    openList.Add(adjacentNode);

            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);
        }

        return foundList;
    }

    public List<GameObject> FindTilesAtDistance(GridTile tile, int distance)
    {
        List<PathNode> nodesAround = FindNodesAtDistance(tile, distance);
        if (nodesAround == null)
            return null;
        List<GameObject> tilesAround = new List<GameObject>();
        foreach (PathNode node in nodesAround)
        {
            tilesAround.Add(GridManager.Instance.Grid[node.i, node.j]);
        }
        return tilesAround;
    }

    public List<PathNode> FindNodesAtDistance(GridTile tile, int distance)
    {
        PathNode nodeTile = NodeGrid[tile.i, tile.j];
        List<PathNode> nodesAtDistance = new List<PathNode>();
        List<PathNode> nodesAround = FindNodesAroundToWalkTo(tile, distance);
        foreach (PathNode node in nodesAround)
        {
            if (CalculateDistanceSimple(nodeTile, node) != distance)
                continue;
            if (!HasClearPath(node, nodeTile))
                continue;
            nodesAtDistance.Add(node);
        }
        if (nodesAtDistance.Count == 0)
            return null;
        else if (nodesAtDistance[0] == null)
            return null;
        return nodesAtDistance;
    }

    public List<PathNode> FindNodePath(int startI, int startJ, int endI, int endJ)
    {
        PathNode startNode = NodeGrid[startI, startJ];
        PathNode endNode = NodeGrid[endI, endJ];

        openList = new List<PathNode>();
        closedList = new List<PathNode>();
        openList.Add(startNode);
        
        SetInitialCosts();
        startNode.gCost = 0;
        startNode.hCost = CalculateDistance(startNode, endNode);

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode adjacentNode in GetAdjacentNodes(currentNode))
            {
                if (closedList.Contains(adjacentNode))
                    continue;
                if (!openList.Contains(adjacentNode))
                    openList.Add(adjacentNode);

                bool hasStep = false;
                if (Mathf.Abs(currentNode.height - adjacentNode.height) > 0)
                {
                    hasStep = true;
                }

                int gCostAttempt = currentNode.gCost + CalculateDistance(currentNode, adjacentNode);
                int stepsAttempt = hasStep ? currentNode.jumps + 1 : currentNode.jumps;
                if (gCostAttempt == adjacentNode.gCost)
                {
                    if (stepsAttempt <= adjacentNode.jumps)
                    {
                        adjacentNode.previousNode = currentNode;
                        adjacentNode.gCost = gCostAttempt;
                        adjacentNode.hCost = CalculateDistance(adjacentNode, endNode);
                        adjacentNode.CalculateFCost();
                        adjacentNode.jumps = stepsAttempt;
                    }
                }

                if (gCostAttempt < adjacentNode.gCost)
                {
                    adjacentNode.previousNode = currentNode;
                    adjacentNode.gCost = gCostAttempt;
                    adjacentNode.hCost = CalculateDistance(adjacentNode, endNode);
                    adjacentNode.CalculateFCost();
                    adjacentNode.jumps = stepsAttempt;
                }

            }

        }

        return null;
    }

    private List<PathNode> GetAdjacentNodes(PathNode node)
    {
        List<PathNode> adjacentNodeList = new List<PathNode>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                //self
                if (i == 0 && j == 0)
                    continue;

                int adjacentI = node.i + i;
                int adjacentJ = node.j + j;

                //bounds
                if (adjacentI < 0 || adjacentI >= height || adjacentJ < 0 || adjacentJ >= width)
                    continue;
                //cant walk there
                if (!NodeGrid[adjacentI, adjacentJ].isWalkable)
                    continue;
                //height difference
                if (Mathf.Abs(NodeGrid[adjacentI, adjacentJ].height - node.height) > maxHeightDifference)
                    continue;

                //top, bottom, left, right
                else if (i == 0 || j == 0)
                {
                    adjacentNodeList.Add(NodeGrid[adjacentI, adjacentJ]);
                }

                //diagonal
                else if (i != 0 && j != 0)
                {
                    if (!NodeGrid[node.i, adjacentJ].isWalkable)
                        continue;
                    if (Mathf.Abs(NodeGrid[node.i, adjacentJ].height - node.height) > 0)
                        continue;

                    if (!NodeGrid[adjacentI, node.j].isWalkable)
                        continue;
                    if (Mathf.Abs(NodeGrid[adjacentI, node.j].height - node.height) > 0)
                        continue;

                    //adjacentNodeList.Add(NodeGrid[adjacentI, adjacentJ]);
                }

            }
        }

        return adjacentNodeList;
    }

    private List<PathNode> GetAdjacentNodesToWalkInto(PathNode node)
    {
        List<PathNode> adjacentNodeList = new List<PathNode>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                //self
                if (i == 0 && j == 0)
                    continue;

                int adjacentI = node.i + i;
                int adjacentJ = node.j + j;

                if (adjacentI < 0 || adjacentI >= height || adjacentJ < 0 || adjacentJ >= width)
                    continue;

                if (!NodeGrid[adjacentI, adjacentJ].isWalkable)
                    continue;
                if (NodeGrid[adjacentI, adjacentJ].isOccupied)
                    continue;

                if (Mathf.Abs(NodeGrid[adjacentI, adjacentJ].height - node.height) > maxHeightDifference)
                    continue;

                //top, bottom, left, right
                else if (i == 0 || j == 0)
                {
                    adjacentNodeList.Add(NodeGrid[adjacentI, adjacentJ]);
                }

                //diagonal
                else if (i != 0 && j != 0)
                {
                    if (!NodeGrid[node.i, adjacentJ].isWalkable)
                        continue;
                    if (Mathf.Abs(NodeGrid[node.i, adjacentJ].height - node.height) > 0)
                        continue;

                    if (!NodeGrid[adjacentI, node.j].isWalkable)
                        continue;
                    if (Mathf.Abs(NodeGrid[adjacentI, node.j].height - node.height) > 0)
                        continue;

                    //adjacentNodeList.Add(NodeGrid[adjacentI, adjacentJ]);
                }

            }
        }

        return adjacentNodeList;
    }

    private List<PathNode> GetAdjacentNodesSimple(PathNode node)
    {
        List<PathNode> adjacentNodeList = new List<PathNode>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                //self
                if (i == 0 && j == 0)
                    continue;

                int adjacentI = node.i + i;
                int adjacentJ = node.j + j;

                if (adjacentI < 0 || adjacentI >= height || adjacentJ < 0 || adjacentJ >= width)
                    continue;

                //top, bottom, left, right
                else if (i == 0 || j == 0)
                {
                    adjacentNodeList.Add(NodeGrid[adjacentI, adjacentJ]);
                }

                //diagonal
                else if (i != 0 && j != 0)
                {
                    if (!NodeGrid[node.i, adjacentJ].isWalkable)
                        continue;
                    if (Mathf.Abs(NodeGrid[node.i, adjacentJ].height - node.height) > 0)
                        continue;

                    if (!NodeGrid[adjacentI, node.j].isWalkable)
                        continue;
                    if (Mathf.Abs(NodeGrid[adjacentI, node.j].height - node.height) > 0)
                        continue;

                    //adjacentNodeList.Add(NodeGrid[adjacentI, adjacentJ]);
                }

            }
        }

        return adjacentNodeList;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.previousNode != null)
        {
            path.Add(currentNode.previousNode);
            currentNode = currentNode.previousNode;
        }

        path.Reverse();
        return path;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodes)
    {
        PathNode lowestNode = pathNodes[0];
        for (int i = 0; i < pathNodes.Count; i++)
        {
            if (pathNodes[i].fCost < lowestNode.fCost)
                lowestNode = pathNodes[i];
        }

        return lowestNode;
    }

    private void SetInitialCosts()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {

                NodeGrid[i, j].gCost = int.MaxValue;
                NodeGrid[i, j].CalculateFCost();
                NodeGrid[i, j].previousNode = null;
            }
        }
    }

    private int CalculateDistance(PathNode a, PathNode b)
    {
        int distanceI = Mathf.Abs(a.i - b.i);
        int distanceJ = Mathf.Abs(a.j - b.j);
        int remaining = Mathf.Abs(distanceI - distanceJ);
        int diagonal = Mathf.Min(distanceI, distanceJ);
        return remaining * 10 + diagonal * 14;
    }
    private int CalculateDistanceSimple(PathNode a, PathNode b)
    {
        int distanceI = Mathf.Abs(a.i - b.i);
        int distanceJ = Mathf.Abs(a.j - b.j);
        return distanceI + distanceJ;
    }
    public List<NearbyEntityData> GetEntitiesAround(GameObject gameObject)
    {
        List<NearbyEntityData> data = new List<NearbyEntityData>();
        GridObject gridObject = gameObject.GetComponent<GridObject>();
        int startI = gridObject.i;
        int startJ = gridObject.j;
        foreach (GameObject entity in BattleManager.Instance.enemyList)
        {
            GridObject entityObject = entity.GetComponent<GridObject>();
            int endI = entityObject.i;
            int endJ = entityObject.j;
            Pathfinding pathfinder = new Pathfinding();
            List<PathNode> nodeList = pathfinder.FindNodePath(startI, startJ, endI, endJ);

            NearbyEntityData entityData = new NearbyEntityData();
            entityData.entity = entity;
            entityData.distance = nodeList.Count - 1;
            entityData.type = entityType.enemy;
            data.Add(entityData);
        }

        foreach (GameObject entity in BattleManager.Instance.charList)
        {
            GridObject entityObject = entity.GetComponent<GridObject>();
            int endI = entityObject.i;
            int endJ = entityObject.j;
            Pathfinding pathfinder = new Pathfinding();
            List<PathNode> nodeList = pathfinder.FindNodePath(startI, startJ, endI, endJ);

            NearbyEntityData entityData = new NearbyEntityData();
            entityData.entity = entity;
            entityData.distance = nodeList.Count - 1;
            entityData.type = entityType.unit;
            data.Add(entityData);
        }

        return data;
    }
    public List<NearbyEntityData> GetUnitsAround(GameObject gameObject)
    {
        List<NearbyEntityData> data = new List<NearbyEntityData>();
        GridObject gridObject = gameObject.GetComponent<GridObject>();
        int startI = gridObject.i;
        int startJ = gridObject.j;

        foreach (GameObject entity in BattleManager.Instance.charList)
        {
            GridObject entityObject = entity.GetComponent<GridObject>();
            int endI = entityObject.i;
            int endJ = entityObject.j;
            Pathfinding pathfinder = new Pathfinding();
            List<PathNode> nodeList = pathfinder.FindNodePath(startI, startJ, endI, endJ);
            if (nodeList == null)
                continue;
            NearbyEntityData entityData = new NearbyEntityData();
            entityData.entity = entity;
            entityData.distance = nodeList.Count - 1;
            entityData.type = entityType.unit;
            data.Add(entityData);
        }

        return data;
    }

    public List<NearbyEntityData> GetEnemiesAround(GameObject gameObject)
    {
        List<NearbyEntityData> data = new List<NearbyEntityData>();
        GridObject gridObject = gameObject.GetComponent<GridObject>();
        int startI = gridObject.i;
        int startJ = gridObject.j;
        foreach (GameObject entity in BattleManager.Instance.enemyList)
        {
            GridObject entityObject = entity.GetComponent<GridObject>();
            int endI = entityObject.i;
            int endJ = entityObject.j;
            Pathfinding pathfinder = new Pathfinding();
            List<PathNode> nodeList = pathfinder.FindNodePath(startI, startJ, endI, endJ);

            NearbyEntityData entityData = new NearbyEntityData();
            entityData.entity = entity;
            entityData.distance = nodeList.Count - 1;
            entityData.type = entityType.enemy;
            data.Add(entityData);
        }

        return data;
    }

    public bool HasClearPath(PathNode initial, PathNode target)
    {
        if (initial.height - target.height > maxHeightToAttackFrom)
            return false;
        if (target.height - initial.height > maxHeightDifference)
            return false;

        int differenceI = Mathf.Abs(initial.i - target.i);
        int differenceJ = Mathf.Abs(initial.j - target.j);

        int relevantI1 = 0;
        int relevantI2 = 0;
        int relevantJ1 = 0;
        int relevantJ2 = 0;

        if (differenceI % 2 == 0)
        {
            relevantI1 = (initial.i + target.i) / 2;
            relevantI2 = relevantI1;
        }
        else if (differenceI % 2 == 1)
        {
            relevantI1 = (initial.i + target.i - 1) / 2;
            relevantI2 = relevantI1 + 1;
        }
        if (differenceJ % 2 == 0)
        {
            relevantJ1 = (initial.j + target.j) / 2;
            relevantJ2 = relevantJ1;
        }
        else if (differenceJ % 2 == 1)
        {
            relevantJ1 = (initial.j + target.j - 1) / 2;
            relevantJ2 = relevantJ1 + 1;
        }

        if (NodeGrid[relevantI1, relevantJ1].height - initial.height > maxHeightDifference)
            return false;
        if (NodeGrid[relevantI1, relevantJ2].height - initial.height > maxHeightDifference)
            return false;
        if (NodeGrid[relevantI2, relevantJ1].height - initial.height > maxHeightDifference)
            return false;
        if (NodeGrid[relevantI2, relevantJ2].height - initial.height > maxHeightDifference)
            return false;



        return true;
    }

    public GameObject GetClosestTile(GameObject gameObject, List<GameObject> tileList)
    {
        GridObject gridObject = gameObject.GetComponent<GridObject>();
        int startI = gridObject.i;
        int startJ = gridObject.j;

        GameObject closestTile = tileList[0];
        int shortestDistance = int.MaxValue;
        foreach (GameObject tile in tileList)
        {
            GridTile gridTile = tile.GetComponent<GridTile>();
            int endI = gridTile.i;
            int endJ = gridTile.j;

            Pathfinding pathfinder = new Pathfinding();
            int distance = pathfinder.FindNodePath(startI, startJ, endI, endJ).Count;
            if (distance < 1)
                continue;
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestTile = tile;
            }
        }

        return closestTile;
    }

    public int CalculateDistance(int startI, int startJ, int endI, int endJ)
    {
        PathNode startNode = NodeGrid[startI, startJ];
        PathNode endNode = NodeGrid[endI, endJ];

        openList = new List<PathNode>();
        closedList = new List<PathNode>();
        openList.Add(startNode);

        SetInitialCosts();
        startNode.gCost = 0;
        startNode.hCost = CalculateDistance(startNode, endNode);

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode)
            {
                return CalculateNodeDistance(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode adjacentNode in GetAdjacentNodes(currentNode))
            {
                if (closedList.Contains(adjacentNode))
                    continue;
                if (!openList.Contains(adjacentNode))
                    openList.Add(adjacentNode);

                bool hasStep = false;
                if (Mathf.Abs(currentNode.height - adjacentNode.height) > 0)
                {
                    hasStep = true;
                }

                int gCostAttempt = currentNode.gCost + CalculateDistance(currentNode, adjacentNode);
                int stepsAttempt = hasStep ? currentNode.jumps + 1 : currentNode.jumps;
                if (gCostAttempt == adjacentNode.gCost)
                {
                    if (stepsAttempt <= adjacentNode.jumps)
                    {
                        adjacentNode.previousNode = currentNode;
                        adjacentNode.gCost = gCostAttempt;
                        adjacentNode.hCost = CalculateDistance(adjacentNode, endNode);
                        adjacentNode.CalculateFCost();
                        adjacentNode.jumps = stepsAttempt;
                    }
                }

                if (gCostAttempt < adjacentNode.gCost)
                {
                    adjacentNode.previousNode = currentNode;
                    adjacentNode.gCost = gCostAttempt;
                    adjacentNode.hCost = CalculateDistance(adjacentNode, endNode);
                    adjacentNode.CalculateFCost();
                    adjacentNode.jumps = stepsAttempt;
                }

            }

        }

        return int.MaxValue;
    }

    private int CalculateNodeDistance(PathNode endNode)
    {
        PathNode currentNode = endNode;
        int distance = 0;
        while (currentNode.previousNode != null)
        {
            distance++;

            currentNode = currentNode.previousNode;
        }


        return distance;
    }

    public List<GameObject> FindTilesAroundSquare(GridTile tile, int size)
    {
        List<PathNode> nodeList = FindNodesAround(tile, size - 1);
        List<GameObject> list = new List<GameObject>();
        int radius = (size - 1) / 2;
        foreach(PathNode node in nodeList)
        {
            if (Mathf.Abs(node.i - tile.i) <= radius && Mathf.Abs(node.j - tile.j) <= radius)
            {
                list.Add(GridManager.Instance.Grid[node.i, node.j]);
            }
        }
        return list;
    }

}

public enum entityType
{
    unit,
    enemy
}
public class NearbyEntityData
{
    public GameObject entity;
    public entityType type;
    public int distance;
}
