using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;




[System.Serializable]
public class NodeListWrapper
{
    public NodeListWrapper()
    {
        list = new List<AINode>();
    }

    public NodeListWrapper(int size)
    {
        list = new List<AINode>(size);
    }

    public List<AINode> list;
}

public class AINodeConnection
{
    public int destinyI;
    public int destinyJ;

    public AINodeConnection(int destinyI, int destinyJ)
    {
        this.destinyI = destinyI;
        this.destinyJ = destinyJ;
    }
}

[System.Serializable]
public class AINode
{
    public Vector2 localPosition;
    public TilemapAIInfo tilemap;
    public int iIndex, jIndex;
    public int listIndex;
    Vector2 originPos;
    public Vector2 tilemapWorldPos;


    public bool disabled;

    public bool isWall;

    public bool downAvailable;
    public bool upAvailable;
    public bool rightAvailable;
    public bool leftAvailable;

    public Vector2 offsetPosition;

    public List<AINodeConnection> connections;
    
    public AINode(Vector2 _localPosition, TilemapAIInfo _tilemap, int _iIndex, int _jIndex)
    {
        iIndex = _iIndex;
        jIndex = _jIndex;
        
        localPosition = _localPosition;
        tilemap = _tilemap;

        connections = new List<AINodeConnection>();

    }



    public Vector2 GetWorldPosition(Vector2 tilemapWorldPos)
    {
        return tilemapWorldPos - localPosition + offsetPosition;
    }




}


public class TilemapAIInfo : MonoBehaviour
{
    [ReadOnlyAttribute]
    [ShowInInspector]
    [SerializeField]
    public List<NodeListWrapper> nodes;


    [ReadOnlyAttribute]
    [ShowInInspector]
    [SerializeField]
    public List<AINode> availableNodes;

    [ReadOnlyAttribute]
    [SerializeField]
    public int cellsX;

    [ReadOnlyAttribute]
    [SerializeField]
    public int cellsY;


    [ReadOnlyAttribute]
    [SerializeField]
    public float cellSize;


    [ReadOnlyAttribute]
    [SerializeField]
    public Vector2 centerOffset;


    public delegate void Delegation(AINode node);

    //Encapsulamos la colección usada para gestionar nodos
    public void ForEachNode(Delegation method)
    {
        for (int i = 0; i < cellsY; i++)
        {
            for (int j = 0; j < cellsX; j++)
            {
                method(nodes[i].list[j]);
            }
        }
    }

   

    [Button("Precalculate Graph Nodes")]
    public void PrecalculateGraphNodes()
    {

        var unityTilemapComponent = this.GetComponent<Tilemap>();

        cellSize = this.GetComponent<Tilemap>().cellSize.x;
        cellsX = unityTilemapComponent.size.x;
        cellsY = unityTilemapComponent.size.y;



         centerOffset = new Vector2(
            unityTilemapComponent.cellBounds.max.x - unityTilemapComponent.cellSize.x * 0.5f,

            unityTilemapComponent.cellBounds.max.y - unityTilemapComponent.cellSize.x * 0.5f
        );





        //calcular filas y columnas

        this.nodes = new List<NodeListWrapper>(cellsY);

        for (int i = 0; i < cellsY; i++)
        {
            this.nodes.Add(new NodeListWrapper(cellsX));

            for (int j = 0; j < cellsX; j++)
            {
                Vector2 localPos =  new Vector2(j * cellSize, i * cellSize) - centerOffset;
                this.nodes[i].list.Add(new AINode(localPos, this, i, j));
            }

            this.nodes[i].list.Reverse();

        }

        this.nodes.Reverse();



        UpdateWallNodes();



    }    

    
    void CalculateAndSetNodeAvailability(int yIndex, int xIndex )
    {

        if (!nodes[yIndex].list[xIndex].isWall)
            return;

        if (xIndex - 1 >= 0) // if left side is valid
        {
            if (!nodes[yIndex].list[xIndex - 1].isWall)// and is not colliding
            {
                nodes[yIndex].list[xIndex].leftAvailable = true;
                nodes[yIndex].list[xIndex - 1].disabled = true;

            }


        }

        if (xIndex + 1 < nodes[yIndex].list.Count) // if right side is valid
        {
            if (!nodes[yIndex].list[xIndex + 1].isWall)// and is not colliding
            {
                nodes[yIndex].list[xIndex].rightAvailable = true;
                nodes[yIndex].list[xIndex + 1].disabled = true;

            }

        }

        if (yIndex + 1 < nodes.Count)// if down side is valid
        {
            if (!nodes[yIndex + 1].list[xIndex].isWall)// and is not colliding
            {
                nodes[yIndex].list[xIndex].upAvailable = true;
                nodes[yIndex + 1].list[xIndex].disabled = true;

            }


        }

        if (yIndex - 1 >= 0)// if up side is valid
        {
            if (!nodes[yIndex - 1].list[xIndex].isWall) // and is not colliding
            {
                nodes[yIndex].list[xIndex].downAvailable = true;
                nodes[yIndex - 1].list[xIndex].disabled = true;


            }


        }
    }

    void UpdateWallNodes()
    {

        var tilemap = GetComponent<Tilemap>();

        BoundsInt bounds = tilemap.cellBounds;

        var tiles = tilemap.GetTilesBlock(bounds);


        availableNodes = new List<AINode>();
        // DETERMINAMOS QUÉ NODOS SON PARED
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                bool isWall = tiles[x + y * bounds.size.x] != null;
                nodes[y].list[x].isWall = isWall;

            }
        }

        // ESTABLECEMOS DISPONNIBILIDAD DE LOS NODOS QUE SON PARED
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                CalculateAndSetNodeAvailability(y,x);
               
            }
        }


        // SEPARAR DISPONIBLES DE NO DISPONIBLES

        ForEachNode((AINode node) =>
        {
            // ONLY WALL NODES
            if(node.isWall && (node.leftAvailable || node.rightAvailable || node.upAvailable || node.downAvailable) /*|| !node.disabled*/)
            {
                availableNodes.Add(node);
            }
        });


        var nodesToAdd = new List<AINode>();

        // FRAGMENTAR WALL NODES CON MÁS DE UNA DISPONIBILIDAD
        foreach (AINode node in availableNodes)
        {
            if(node.isWall)
            {
                bool firstOffsetAssigned = false;

                if (node.leftAvailable)
                {
                    firstOffsetAssigned = true;
                    node.offsetPosition = Vector2.left * cellSize * 0.5f;
                }

                if (node.rightAvailable)
                {
                    if (!firstOffsetAssigned)
                    {
                        firstOffsetAssigned = true;
                        node.offsetPosition = Vector2.right * cellSize * 0.5f;
                    }else
                    {
                        node.rightAvailable = false;
                        var newNode = new AINode(node.localPosition, node.tilemap, node.iIndex, node.jIndex);
                        newNode.offsetPosition = Vector2.right * cellSize * 0.5f;
                        newNode.rightAvailable = true;
                        newNode.isWall = true;
                        nodesToAdd.Add(newNode);
                    }

                }

                if (node.upAvailable)
                {
                    if (!firstOffsetAssigned)
                    {
                        firstOffsetAssigned = true;
                        node.offsetPosition = Vector2.up * cellSize * 0.5f;
                    }
                    else
                    {
                        node.upAvailable = false;
                        var newNode = new AINode(node.localPosition, node.tilemap, node.iIndex, node.jIndex);
                        newNode.upAvailable = true;
                        newNode.offsetPosition = Vector2.up * cellSize * 0.5f;
                        newNode.isWall = true;
                        nodesToAdd.Add(newNode);
                    }
                }

                if (node.downAvailable)
                {
                    if (!firstOffsetAssigned)
                    {
                        firstOffsetAssigned = true;
                        node.offsetPosition = Vector2.down * cellSize * 0.5f;
                    }
                    else
                    {
                        node.downAvailable = false;
                        var newNode = new AINode(node.localPosition, node.tilemap, node.iIndex, node.jIndex);
                        newNode.downAvailable = true;
                        newNode.offsetPosition = Vector2.down * cellSize * 0.5f;
                        newNode.isWall = true;
                        nodesToAdd.Add(newNode);
                    }
                }

                


            }
        }

        foreach (var node in nodesToAdd)
        {
            availableNodes.Add(node);
        }

        // Construct final matrix;

        int heights = nodes.Count*3;
        List<NodeListWrapper> finalNodes = new List<NodeListWrapper>(heights);

        // tenemos el triple de alturas pq tenemos
        // el centro de un wall node
        // el up de un wall noed
        // el down de un wall node

        for (int i = 0; i < heights; i++)
        {
            finalNodes.Add(new NodeListWrapper());
        }

        foreach(var node in availableNodes)
        {
            int iIndex = node.iIndex*3;

            if (node.downAvailable)
            {
                iIndex++;
            }else if (node.upAvailable)
            {
                iIndex--;
            }
            node.iIndex = iIndex;
            //Falta el short en el eje horizontal
            finalNodes[iIndex].list.Add(node);

        }

        finalNodes.Reverse();
        nodes = finalNodes;

}



    private void DrawNodeAvailability(AINode node)
    {
        if (!node.isWall)
            return;

        Gizmos.color = node.isWall ? Color.green : Color.red;
        Gizmos.DrawWireSphere(node.GetWorldPosition(transform.position), 0.1f);
    }

    
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.green;

        var tilemap = GetComponent<Tilemap>();


        Vector2 textSize = new Vector3(0.2f, -0.1f, 0f);
        
    }



   



    public bool NodesPrecalculated()
    {
        return nodes != null && nodes.Count != 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
