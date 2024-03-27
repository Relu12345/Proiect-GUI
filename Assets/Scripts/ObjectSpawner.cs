using Meta.XR.MRUtilityKit;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // Assign the prefab you want to spawn in the inspector
    public int numberOfObjectsToSpawn = 5; // Set this number in the editor
    public float offsetFromWall = 0.1f; // Adjust this value to set how far in front of the wall the objects should spawn
    public float checkRadius = 0.5f; // Radius for the overlap test to check for existing objects

    public void SpawnObjects()
    {
        MRUKRoom currentRoom = MRUK.Instance.GetCurrentRoom();
        List<MRUKAnchor> wallAnchors = currentRoom.GetWallAnchors();

        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            MRUKAnchor randomWallAnchor = wallAnchors[Random.Range(0, wallAnchors.Count)];
            if (randomWallAnchor.HasPlane)
            {
                Vector2 planeSize = randomWallAnchor.PlaneRect.Value.size;
                Vector3 randomPositionOnPlane = new Vector3(
                    Random.Range(-planeSize.x / 2, planeSize.x / 2),
                    Random.Range(-planeSize.y / 2, planeSize.y / 2),
                    0);

                // Convert the random position on the plane to world space
                Vector3 worldPositionOnPlane = randomWallAnchor.transform.TransformPoint(randomPositionOnPlane);

                // Use GetClosestSurfacePosition to ensure the position is on the wall's surface
                Vector3 positionOnWall;
                randomWallAnchor.GetClosestSurfacePosition(worldPositionOnPlane, out positionOnWall);

                // Get the normalized facing direction of the wall anchor
                Vector3 facingDirection = currentRoom.GetFacingDirection(randomWallAnchor).normalized;

                // Adjust the position to be slightly in front of the wall based on the wall's normal (facing direction)
                positionOnWall += facingDirection * offsetFromWall;

                // Keep trying to find a position until it's within the desired range
                while (!(positionOnWall.y >= 1f && positionOnWall.y <= 1.7f))
                {
                    randomPositionOnPlane = new Vector3(
                        Random.Range(-planeSize.x / 2, planeSize.x / 2),
                        Random.Range(-planeSize.y / 2, planeSize.y / 2),
                        0);

                    worldPositionOnPlane = randomWallAnchor.transform.TransformPoint(randomPositionOnPlane);
                    randomWallAnchor.GetClosestSurfacePosition(worldPositionOnPlane, out positionOnWall);
                    positionOnWall += facingDirection * offsetFromWall;
                }

                // Check for existing objects using an overlap test
                Collider[] hitColliders = Physics.OverlapSphere(positionOnWall, checkRadius);
                if (hitColliders.Length == 0) // No colliders found, safe to spawn
                {
                    GameObject spawnedObject = Instantiate(objectToSpawn, positionOnWall, Quaternion.identity);

                    // Determine the rotation based on the facing direction
                    if (Mathf.Abs(facingDirection.x) > Mathf.Abs(facingDirection.z))
                    {
                        // Rotate around z-axis if x is dominant
                        float zRotation = facingDirection.x > 0 ? -90f : 90f;
                        spawnedObject.transform.Rotate(0, 0, zRotation);
                    }
                    else
                    {
                        // Rotate around x-axis if z is dominant
                        float xRotation = facingDirection.z > 0 ? -90f : 90f;
                        spawnedObject.transform.Rotate(xRotation, 0, 0);
                    }
                }
                else
                {
                    Debug.LogWarning("Cannot spawn object at position: " + positionOnWall + " as it is occupied by other objects.");
                }
            }
            else
            {
                Debug.LogError("Selected wall anchor does not have a plane.");
            }
        }
    }
}
