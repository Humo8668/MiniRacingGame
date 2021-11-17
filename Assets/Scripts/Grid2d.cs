using System.Collections.Generic;
using UnityEngine;

namespace MiniRacing
{
    public class Grid2d : MonoBehaviour
    {
        [SerializeField]
        int XCount = 4;
        [SerializeField]
        int ZCount = 4;
        [SerializeField]
        float sizeX = 10;
        [SerializeField]
        float sizeZ = 10;

        int[,] gridData;
        int unique_sq = 0;
        BoxCollider triggerCollider;

        //Transform[] ObjectsOnGrid;
        // Reflection of number to Object
        Dictionary<int, Transform> ObjectsList;

        public Transform getObject(float xPos, float zPos)
        {
            return getObject(Mathf.FloorToInt(xPos), Mathf.FloorToInt(zPos));
        }

        private void Reset()
        {
            if (!TryGetComponent<BoxCollider>(out this.triggerCollider))
                throw new System.Exception("Append collider!");

            Vector3 size = new Vector3(XCount * sizeX, 0.1f, ZCount * sizeZ);
            Vector3 center = size / 2;
            this.triggerCollider.center = center;
            this.triggerCollider.size = size;
            this.triggerCollider.isTrigger = true;
        }

        public Transform getObject(int xPos, int zPos)
        {
            if (0 < xPos && xPos < gridData.GetLength(0) &&
               0 < zPos && zPos < gridData.GetLength(1) && gridData[xPos, zPos] != 0)
                return ObjectsList[gridData[xPos, zPos]];
            else
                return null;
        }

        public void setObject(Transform obj, int xPos, int zPos)
        {
            Vector3 halfSize = new Vector3(sizeX / 2, 0.0f, sizeZ / 2);

            if (gridData[xPos, zPos] != 0)
            {
                ObjectsList.Remove(gridData[xPos, zPos]);
                gridData[xPos, zPos] = 0;
            }

            unique_sq++;
            ObjectsList.Add(unique_sq, obj);
            gridData[xPos, zPos] = unique_sq;

            obj.position = new Vector3(xPos * sizeX, 0, zPos * sizeZ) + halfSize;
        }

        public void removeObject(int xPos, int zPos)
        {
            gridData[xPos, zPos] = 0;
        }

        public void setObject(Transform obj, float xPos, float zPos)
        {
            setObject(obj, Mathf.FloorToInt(xPos), Mathf.FloorToInt(zPos));
        }

        void updateGridData()
        {
            Vector3 halfSize = new Vector3(sizeX / 2, 0.0f, sizeZ / 2);
            int[,] newGridData = new int[XCount, ZCount];
            for (int x = 0; x < gridData.GetLength(0); x++)
                for (int z = 0; z < gridData.GetLength(1); z++)
                {
                    newGridData[x, z] = gridData[x, z];
                    ObjectsList[gridData[x, z]].position = new Vector3(x * sizeX, 0, z * sizeZ) + halfSize;
                }

            this.Reset();
            this.gridData = newGridData;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            Vector3 halfSize = new Vector3(sizeX / 2, 0.0f, sizeZ / 2);

            for (int i = 0; i < XCount; i++)
            {
                for (int j = 0; j < ZCount; j++)
                {
                    Gizmos.DrawWireCube(new Vector3(i * sizeX, 0, j * sizeZ) + halfSize, halfSize * 2);
                }
            }
        }
    }
}