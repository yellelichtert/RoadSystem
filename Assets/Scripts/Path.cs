using System;
using UnityEditor;
using UnityEngine;

public class Path : MonoBehaviour
{
    [Tooltip("List of nodes that make up the path")] [SerializeField]
    private Vector3[] nodes = Array.Empty<Vector3>();
    
    
    public int NodeCount => nodes.Length;
    
    public Vector3 GetNode(int index)
        =>nodes[index];

    public void SetNode(int index, Vector3 position)
        => nodes[index] = position;
    
    
    public void AddNode(Vector3 position)
    {
        Array.Resize(ref nodes, nodes.Length + 1);
        nodes[nodes.Length - 1] = position;
    }
    
    public void RemoveNode(int index)
    {
        if (index < 0 || index >= nodes.Length) return;
        
        for (int i = index; i < nodes.Length - 1; i++)
        {
            nodes[i] = nodes[i + 1];
        }
        
        Array.Resize(ref nodes, nodes.Length - 1);
    }
    
    
    private void OnDrawGizmos()
    {
        if (nodes == null) return;
        
        Vector3? previousNode = null;
        
        for (int i = 0; i < nodes.Length; i++)
        {
            Vector3 node = GetNode(i);
            
            if (previousNode.HasValue)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(previousNode.Value, node);
            }
            
            previousNode = node;
        }
    }
}
