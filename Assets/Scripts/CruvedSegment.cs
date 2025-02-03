using System;
using UnityEngine;

public abstract class CruvedSegment : Segment
{
        protected Vector3[] Nodes = Array.Empty<Vector3>();
        
        public int NodeAmount
            => Nodes.Length;
        
        
        public override void SetControlPoint(int index, Vector3 position)
        {
            base.SetControlPoint(index, position);
            GenerateSegment();
            
        }
        
        public Vector3 GetNode(int index)
            => Nodes[index];

        protected abstract void GenerateSegment();

}