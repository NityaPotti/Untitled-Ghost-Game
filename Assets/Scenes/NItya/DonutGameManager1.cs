// using System.Collections;
// using UnityEngine;

// public class DonutGameManager : MonoBehaviour
// {
//     public Transform tongs;           // Reference to the tongs GameObject
//     public Transform tray;            // Reference to the tray where donuts are placed
//     public GameObject donutPrefab;    // Reference to the donut prefab
//     public float moveSpeed = 5f;      // Speed at which the tongs can move horizontally
//     public float dropHeight = 5f;     // How far down the tongs will drop to grab a donut
//     public float grabHeight = 1f;     // Height where the tongs will stop grabbing

//     private bool isMoving = false;    // Flag to prevent movement while dropping
//     private Vector3 initialTongsPos;  // The initial position of the tongs

//     void Start()
//     {
//         initialTongsPos = tongs.position;  // Save initial position
//     }

//     void Update()
//     {
//         if (!isMoving)
//         {
//             // Move tongs left or right
//             float move = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
//             tongs.position = new Vector3(Mathf.Clamp(tongs.position.x + move, tray.position.x - 5f, tray.position.x + 5f), tongs.position.y, tongs.position.z);
//         }

//         // Press "Space" to drop the tongs and try to grab a donut
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             StartCoroutine(DropAndGrab());
//         }
//     }

//     IEnumerator DropAndGrab()
//     {
//         isMoving = true;

//         // Move the tongs downwards
//         Vector3 targetPosition = tongs.position - new Vector3(0, dropHeight, 0);
//         float dropDuration = 1f;
//         float elapsedTime = 0;

//         while (elapsedTime < dropDuration)
//         {
//             tongs.position = Vector3.Lerp(tongs.position, targetPosition, (elapsedTime / dropDuration));
//             elapsedTime += Time.deltaTime;
//             yield return null;
//         }

//         // Check if the tongs grabbed a donut
//         RaycastHit hit;
//         if (Physics.Raycast(tongs.position, Vector3.down, out hit, 1f))
//         {
//             if (hit.collider.CompareTag("Donut"))
//             {
//                 hit.collider.transform.SetParent(tongs);
//                 hit.collider.GetComponent<Rigidbody>().isKinematic = true; // Disable physics for the grabbed donut
//             }
//         }

//         // Move the tongs up with the donut
//         targetPosition = initialTongsPos;
//         elapsedTime = 0;

//         while (elapsedTime < dropDuration)
//         {
//             tongs.position = Vector3.Lerp(tongs.position, targetPosition, (elapsedTime / dropDuration));
//             elapsedTime += Time.deltaTime;
//             yield return null;
//         }

//         // Check if the donut was grabbed correctly
//         if (tongs.childCount > 0)
//         {
//             // Successfully grabbed donut
//             Debug.Log("Donut grabbed!");
//         }
//         else
//         {
//             // Failed to grab donut
//             Debug.Log("Failed to grab donut.");
//         }

//         // Reset tongs to the initial position
//         tongs.position = initialTongsPos;
//         isMoving = false;
//     }
// }

using System.Collections;
using UnityEngine;

public class DonutGameManager : MonoBehaviour
{
    public Transform tongs;            // Tongs GameObject
    public Transform tray;             // Tray where donuts are placed
    public Transform shelf;            // Café shelf where donuts start
    public Transform plate;            // Plate where the donut will be placed
    public GameObject donutPrefab;     // Donut prefab (Blender model)
    public Camera ghostCamera;         // First-person camera for ghost's perspective

    public float moveSpeed = 5f;       // Speed of tongs moving left/right
    public float dropHeight = 5f;      // How far tongs drop to grab a donut
    public float grabHeight = 1f;      // Height where tongs stop to grab

    private bool isMoving = false;
    private Vector3 initialTongsPos;

    void Start()
    {
        // Save initial tongs position
        initialTongsPos = tongs.position;

        // Position camera to look through ghost's eyes
        ghostCamera.transform.position = new Vector3(tongs.position.x, tongs.position.y + 2, tongs.position.z - 5);
        ghostCamera.transform.LookAt(tongs);

        // Spawn donuts on the café shelf
        SpawnDonuts();
    }

    void Update()
    {
        if (!isMoving)
        {
            // Move tongs left and right
            float move = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            tongs.position = new Vector3(
                Mathf.Clamp(tongs.position.x + move, tray.position.x - 5f, tray.position.x + 5f),
                tongs.position.y,
                tongs.position.z
            );
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DropAndGrab());
        }
    }

    void SpawnDonuts()
    {
        // Example: Spawning 3 donuts on the shelf
        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPosition = shelf.position + new Vector3(i * 2f, 0, 0);
            Instantiate(donutPrefab, spawnPosition, Quaternion.identity);
        }
    }

    IEnumerator DropAndGrab()
{
    isMoving = true;
    Vector3 targetPosition = tongs.position - new Vector3(0, dropHeight, 0);
    float dropDuration = 1f;
    float elapsedTime = 0;

    // Lower the tongs
    while (elapsedTime < dropDuration)
    {
        tongs.position = Vector3.Lerp(tongs.position, targetPosition, elapsedTime / dropDuration);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Try grabbing a donut
    Transform grabbedDonut = null;
    RaycastHit hit;
    if (Physics.Raycast(tongs.position, Vector3.down, out hit, 1f))
    {
        if (hit.collider.CompareTag("Donut"))
        {
            grabbedDonut = hit.collider.transform;
            grabbedDonut.SetParent(tongs);
            grabbedDonut.GetComponent<Rigidbody>().isKinematic = true; // Disable physics
            Debug.Log("Donut grabbed!");
        }
    }

    // Move tongs back up
    targetPosition = initialTongsPos;
    elapsedTime = 0;

    while (elapsedTime < dropDuration)
    {
        tongs.position = Vector3.Lerp(tongs.position, targetPosition, elapsedTime / dropDuration);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Move the donut to the plate if it was grabbed
    if (grabbedDonut != null)
    {
        grabbedDonut.SetParent(null); // Detach from tongs
        grabbedDonut.position = plate.position + Vector3.up * 0.5f; // Adjust height
        grabbedDonut.GetComponent<Rigidbody>().isKinematic = false; // Reactivate physics
        Debug.Log("Donut placed on the plate!");
    }
    else
    {
        Debug.Log("Failed to grab a donut.");
    }
    ghostCamera.transform.position = new Vector3(tongs.position.x, tongs.position.y + 2, tongs.position.z - 5);
    ghostCamera.transform.LookAt(plate); // Make the camera look at the plate


    tongs.position = initialTongsPos;
    isMoving = false;
}

}

