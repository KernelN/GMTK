using System;
using System.Collections;
using System.Collections.Generic;
using PlanetMover.Gameplay.Objects;
using UnityEngine;

namespace PlanetMover.Gameplay
{
    public class GridKeeper : MonoBehaviour
    {
        [Header("Store Values")]
        [SerializeField] LayerMask storableLayers;
        [SerializeField, Min(0)] Vector3 checkSize = Vector3.one;
        [SerializeField, Min(0)] Vector3 checkOffset;
        [SerializeField, Min(0)] float shrinkTime = 1f;
        
        [Header("Grid Values")]
        [SerializeField, Min(1)] Vector2Int gridSize = Vector2Int.one;
        [SerializeField, Min(0)] float cellSize = 1f;
        
        [Header("Runtime Values")]
        [SerializeField] List<Transform> storedObjects;
        [SerializeField] Vector2Int freeCellIndex;
        Vector3[][] cellPositions;
        
        Vector3 CellSize => Vector3.one.normalized * cellSize;

        //Unity Events
        void Start()
        {
            cellPositions = new Vector3[gridSize.x][];
            Vector3 center = transform.position;
            Vector2 gridExtent = (Vector2)gridSize/2;
            for (int i = 0; i < gridSize.x; i++)
            {
                cellPositions[i] = new Vector3[gridSize.y];
                for (int j = 0; j < gridSize.y; j++)
                {
                    //Get pos of cell
                    Vector3 offset = CellSize;
                    offset.x *= i-gridExtent.x;
                    offset.y = 0;
                    offset.z *= j-gridExtent.y;
                        
                    //Offset by cell extents
                    offset += CellSize/2;
                        
                    //Store in grid
                    cellPositions[i][j] = center + offset;
                }
            }
        }
        void LateUpdate()
        {
            if(freeCellIndex == gridSize) return; //grid is full
            
            Collider[] hits = Physics.OverlapBox(transform.position, checkSize / 2, 
                                                transform.rotation, storableLayers);

            for (int i = 0; i < hits.Length; i++)
            {
                if (!hits[i].TryGetComponent(out Scalable scale)) continue;
                
                hits[i].enabled = false; //disable collider
                
                if(hits[i].TryGetComponent(out Movable movable))
                    movable.RemoveTarget();
                
                if(hits[i].TryGetComponent(out Rigidbody rb))
                    rb.isKinematic = true;

                StartCoroutine(ShrinkAndStore(hits[i].transform, scale)); //store in grid
            }
        }
        void OnDrawGizmos()
        {
            //Draw grid
            Gizmos.color = Color.green;
            if (cellPositions == null)
            {
                Vector3 center = transform.position;
                Vector2 gridExtent = (Vector2)gridSize/2;
                for (int i = 0; i < gridSize.x; i++)
                {
                    for (int j = 0; j < gridSize.y; j++)
                    {
                        //Get pos of cell
                        Vector3 offset = CellSize;
                        offset.x *= i-gridExtent.x;
                        offset.y = 0;
                        offset.z *= j-gridExtent.y;
                        
                        //Offset by cell extents
                        offset += CellSize/2;
                        
                        //Draw in world
                        Gizmos.DrawWireCube(center + offset, CellSize);
                    }
                }
            }
            else
            {
                for (int i = 0; i < gridSize.x; i++)
                {
                    for (int j = 0; j < gridSize.y; j++)
                    {
                        Gizmos.DrawWireCube(cellPositions[i][j], CellSize);
                    }
                }
            }
            
            //Draw overlap box
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + checkOffset, checkSize);
        }

        //Methods

        IEnumerator ShrinkAndStore(Transform tran, Scalable scale)
        {
            //Get positions
            Vector3 origin = tran.position;
            Vector3 target = cellPositions[freeCellIndex.x][freeCellIndex.y];

            //Increase cell index
            freeCellIndex.x++;
            if(freeCellIndex.x >= gridSize.x)
            {
                freeCellIndex.x = 0;
                freeCellIndex.y++;
            }
            
            Quaternion sourceRot = tran.rotation;
            
            float sourceScale = scale.CurrentScale;
            
            float timer = 0;
            float t = 0;
            while (timer < shrinkTime)
            {
                timer += Time.deltaTime;
                t = Mathf.SmoothStep(0, 1, timer);
                scale.SetScale(Mathf.Lerp(sourceScale, cellSize/2, t), false);
                tran.position = Vector3.Lerp(origin, target, t);
                tran.rotation = Quaternion.Lerp(sourceRot, Quaternion.identity, t);
                
                yield return null;
            }
        }
    }
}
