using Meta.XR.MRUtilityKit;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallObjectSpawner : MonoBehaviour
{
    public float checkRadius = 0.5f; // Radius for the overlap test to check for existing objects
    public float targetTime = 60.0f;

    public void SpawnObjectsOnWalls(ObjectSpawnInstance obj)
    {
        GameObject objectToSpawn = obj.gameObj;
        int numberOfObjectsToSpawn = obj.intValue;
        float offset = obj.offset;

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
                positionOnWall += facingDirection * offset;

                // Keep trying to find a position until it's within the desired range
                while (!(positionOnWall.y >= 1f && positionOnWall.y <= 1.7f))
                {
                    randomPositionOnPlane = new Vector3(
                        Random.Range(-planeSize.x / 2, planeSize.x / 2),
                        Random.Range(-planeSize.y / 2, planeSize.y / 2),
                        0);

                    worldPositionOnPlane = randomWallAnchor.transform.TransformPoint(randomPositionOnPlane);
                    randomWallAnchor.GetClosestSurfacePosition(worldPositionOnPlane, out positionOnWall);
                    positionOnWall += facingDirection * offset;
                }

                // Get all renderers of the object's children
                Renderer[] childRenderers = objectToSpawn.GetComponentsInChildren<Renderer>();

                // Combine bounds of all child renderers
                Bounds combinedBounds = new Bounds();
                foreach (Renderer renderer in childRenderers)
                {
                    combinedBounds.Encapsulate(renderer.bounds);
                }

                // Get the list of room anchors
                List<MRUKAnchor> roomAnchors = currentRoom.GetRoomAnchors();

                foreach (MRUKAnchor wAnchor in wallAnchors)
                {
                    // Remove floorAnchor from the list
                    roomAnchors.Remove(wAnchor);
                }

                // Initialize a flag to keep track of whether a free position is found
                bool foundFreePosition = false;
                int tries = 10;

                // Check for existing objects using an overlap test
                Collider[] hitColliders = Physics.OverlapBox(positionOnWall, combinedBounds.extents);

                // Keep trying to find a free position until one is found
                while (!foundFreePosition)
                {
                    if (tries <= 0)
                    {
                        foundFreePosition = true;
                    }
                    tries--;

                    // Check if the position overlaps with existing objects or anchors
                    bool overlaps = false;
                    foreach (MRUKAnchor anchor in roomAnchors)
                    {
                        Vector3 anchorPosition = anchor.GetAnchorCenter();
                        Vector3 anchorSize = anchor.GetAnchorSize();
                        Bounds anchorBounds = new Bounds(anchorPosition, anchorSize);
                        if (anchorBounds.Intersects(combinedBounds))
                        {
                            overlaps = true;
                            break;
                        }
                    }

                    if (hitColliders.Length == 0 && !overlaps) // No overlapping bounds found and no anchor overlaps, safe to spawn
                    {
                        Debug.Log("Object's new position: " + positionOnWall);

                        // Set the flag to true to exit the loop
                        foundFreePosition = true;
                    }
                    else
                    {
                        // Debug a warning if the position is occupied and try a different position
                        Debug.LogWarning("Position: " + positionOnWall + " is occupied by other objects or anchors. Trying another position.");

                        // Try a new position
                        while (!(positionOnWall.y >= 1f && positionOnWall.y <= 1.7f))
                        {
                            randomPositionOnPlane = new Vector3(
                                Random.Range(-planeSize.x / 2, planeSize.x / 2),
                                Random.Range(-planeSize.y / 2, planeSize.y / 2),
                                0);

                            worldPositionOnPlane = randomWallAnchor.transform.TransformPoint(randomPositionOnPlane);
                            randomWallAnchor.GetClosestSurfacePosition(worldPositionOnPlane, out positionOnWall);
                            positionOnWall += facingDirection * offset;
                        }
                    }
                }

                GameObject spawnedObject = Instantiate(objectToSpawn, positionOnWall, Quaternion.identity);

                // Determine the rotation based on the facing direction
                if (Mathf.Abs(facingDirection.x) > Mathf.Abs(facingDirection.z))
                {
                    // Rotate around z-axis if x is dominant
                    float yRotation = facingDirection.x > 0 ? -90f : 90f;
                    spawnedObject.transform.Rotate(0, yRotation, 0);
                }
                else
                {
                    // Rotate around x-axis if z is dominant
                    float yRotation = facingDirection.z > 0 ? -180f : 0f;
                    spawnedObject.transform.Rotate(0, yRotation, 0);
                }
                Debug.Log("Object " + i + " has direction: " + facingDirection);
            }
            else
            {
                Debug.LogError("Selected wall anchor does not have a plane.");
            }
        }
    }
}
