using System;
using UnityEngine;


namespace Model
{
    [ExecuteInEditMode]
    public class Node : MonoBehaviour
    {
        
        public Vector3 GetPosition()
            => transform.position;
        
        
        public void SetPosition(Vector3 position)
            => transform.position = position;
        
        
        

        public static Node Create(Vector3 position, Transform parent)
        {
            
            Node node = new GameObject($"Node #{parent.childCount}").AddComponent<Node>();
            
            
            node.transform.position = position;
            node.transform.SetParent(parent);


            return node;
            
        }


        private void OnDrawGizmos()
        {
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(GetPosition(), 1);
            
        }
    }
}