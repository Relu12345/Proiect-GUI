using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorObjectSpawner : MonoBehaviour
{
    public GameObject objectToMove; // Assign the prefab you want to spawn in the inspector
    public int numberOfObjectsToSpawn = 5; // Set this number in the editor
    public float offsetFromFloor = 0.1f; // Adjust this value to set how far in front of the wall the objects should spawn
    public float targetTime = 60.0f;
    public EffectMesh meshes;

    private void Start()
    {
        // StartCoroutine(TimerCoroutine(targetTime));
        SpawnObjects();
    }

    private IEnumerator TimerCoroutine(float target)
    {
        float time = 0f;

        while (time < target)
        {
            time += Time.deltaTime;
            yield return null;
        }

        meshes.HideMesh = false;
        WallObjectSpawner wallScript = GetComponent<WallObjectSpawner>();
        ObjectSpawnInstance[] refs = GameObject.Find("Object References").GetComponentsInChildren<ObjectSpawnInstance>();
        foreach (ObjectSpawnInstance instance in refs)
        {
            if (instance != null)
                wallScript.SpawnObjectsOnWalls(instance);
        }
    }

    public void ShowMeshesManually()
    {
        meshes.HideMesh = false;
    }

    public void SpawnObjects()
    {
        MRUKRoom currentRoom = MRUK.Instance.GetCurrentRoom();
        MRUKAnchor floorAnchor = currentRoom.GetFloorAnchor();

        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            if (floorAnchor.HasPlane)
            {
                // Convert the random position on the plane to world space
                Vector3 worldPositionOnPlane = floorAnchor.GetAnchorCenter();

                // Use GetClosestSurfacePosition to ensure the position is on the floor's surface
                Vector3 positionOnFloor;
                floorAnchor.GetClosestSurfacePosition(worldPositionOnPlane, out positionOnFloor);

                // Get the normalized facing direction of the floor anchor
                Vector3 facingDirection = currentRoom.GetFacingDirection(floorAnchor).normalized;

                // Adjust the position to be slightly in front of the floor based on the floor's normal (facing direction)
                positionOnFloor += facingDirection * offsetFromFloor;

                // Get all renderers of the object's children
                Renderer[] childRenderers = objectToMove.GetComponentsInChildren<Renderer>();

                // Combine bounds of all child renderers
                Bounds combinedBounds = new Bounds();
                foreach (Renderer renderer in childRenderers)
                {
                    combinedBounds.Encapsulate(renderer.bounds);
                }

                // Get the list of room anchors
                List<MRUKAnchor> roomAnchors = currentRoom.GetRoomAnchors();

                // Remove floorAnchor from the list
                roomAnchors.Remove(floorAnchor);

                // Initialize a flag to keep track of whether a free position is found
                bool foundFreePosition = false;
                int tries = 10;

                // Keep trying to find a free position until one is found
                while (!foundFreePosition)
                {
                    if (tries <= 0)
                    {
                        foundFreePosition = true;
                    }
                    tries--;

                    // Check for existing objects using an overlap test
                    Collider[] hitColliders = Physics.OverlapBox(positionOnFloor, combinedBounds.extents);

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
                        objectToMove.transform.position = new Vector3(positionOnFloor.x, 0.5f, positionOnFloor.z);
                        Debug.Log("Object has been moved to position: " + positionOnFloor);

                        // Set the flag to true to exit the loop
                        foundFreePosition = true;
                    }
                    else
                    {
                        // Debug a warning if the position is occupied and try a different position
                        Debug.LogWarning("Position: " + positionOnFloor + " is occupied by other objects or anchors. Trying another position.");

                        // Adjust the position to try a different location around the center on the x-z plane
                        positionOnFloor += new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * offsetFromFloor;
                    }
                }
            }
            else
            {
                Debug.LogError("Selected wall anchor does not have a plane.");
            }
        }
    }
}
