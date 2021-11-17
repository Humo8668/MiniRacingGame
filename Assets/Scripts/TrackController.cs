using System.Collections.Generic;
using UnityEngine;

namespace MiniRacing
{
    public class TrackController : MonoBehaviour
    {
        protected static HashSet<Road> roadBlocks;

        static TrackController()
        {
            TrackController.roadBlocks = new HashSet<Road>();
        }

        public static void RegisterRoadBlock(Road block)
        {
            if (block == null)
                return;

            roadBlocks.Add(block);
        }

        public static Vector3 GetNearestTrackPoint(Vector3 forPoint)
        {
            float minDistance = float.MaxValue;
            Vector3 nearestTrackPoint = forPoint;
            lock(roadBlocks)
            {
                foreach (Road road in roadBlocks)
                {
                    Vector3 roadPoint = road.getPointToRespawn();
                    if (Vector3.Distance(roadPoint, forPoint) < minDistance)
                    {
                        nearestTrackPoint = roadPoint;
                        minDistance = Vector3.Distance(roadPoint, forPoint);
                    }
                }
            }
            
            return nearestTrackPoint;
        }
    }
}
