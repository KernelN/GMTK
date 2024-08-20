using System;
using KinematicCharacterController;
using UnityEngine;

namespace PlanetMover.Gameplay.Props
{
    public class ObjectTeleporter : MonoBehaviour
    {
        [Header("Set Values")]
        [SerializeField] ObjectTeleporter target;
        [SerializeField] Vector3 checkSize;
        [Header("OPTIONALS")]
        [SerializeField, Tooltip("Will tp only on door closed")] 
        Circuit.DoorCircuitTarget[] doors;
        //[Header("Runtime Values")]

        //Unity Events
        void Start()
        {
            if(doors.Length > 0) doors[0].DoorsClosed += () => {Teleport(GetObjects());};
        }
        void LateUpdate()
        {
            if(doors.Length > 0) return;
            
            Transform[] objs = GetObjects();
            
            if(objs.Length == 0) return;
            
            Teleport(objs);
        }
        void OnDrawGizmos()
        {
            //Set rotation matrix to this objects
            Matrix4x4 oldM = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position,
                                            transform.rotation, Vector3.one);
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(Vector3.zero, checkSize);
            
            //Reset matrix
            Gizmos.matrix = oldM;
        }

        //Methods
        public void OpenDoors()
        {
            if(doors.Length == 0) return;
            
            for (int i = 0; i < doors.Length; i++)
                doors[i].ForceOpen();
        }
        Transform[] GetObjects()
        {
            Transform[] objs;

            Vector3 pos = transform.position;
            var hits = Physics.OverlapBox(pos, checkSize / 2, transform.rotation);
            
            objs = new Transform[hits.Length];
            
            for (int i = 0; i < hits.Length; i++)
            {
                objs[i] = hits[i].transform;
            }
            
            return objs;
        }
        void Teleport(Transform[] objs)
        {
            if(!target) return;
            
            //Vector3 dist = target.position - transform.position;
            
            for (int i = 0; i < objs.Length; i++)
            {
                if(objs[i].TryGetComponent(out KinematicCharacterMotor motor))
                    motor.SetPosition(target.transform.position);
                else
                    objs[i].position = target.transform.position;
            }
            
            target.OpenDoors();
        }
    }
}
