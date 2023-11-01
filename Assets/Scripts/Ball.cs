using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private new AudioSource audio;

    [SerializeField]
    private AudioClip clip;

    [SerializeField]
    private Rigidbody rb;

    private bool isTravelling;
    private Vector3 travelDirection;
    private Vector3 nextCollisionPosition;

    private int minSwipeRecognition = 1000;

    private Vector2 swipePosLastFrame;
    private Vector2 swipePosCurrentFrame;
    private Vector2 currentSwipe;

    private Color solveColor;


    private void Start()
    {
        solveColor = Random.ColorHSV(0.5f, 1);
        GetComponent<MeshRenderer>().material.color = solveColor;
        audio = GetComponent<AudioSource>();
    }
    void FixedUpdate()
    {
        if (isTravelling)
        {
            rb.velocity = speed * travelDirection;
        }

        var hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up / 2), .05f);

        int i = 0;
        while (i < hitColliders.Length)
        {
            var ground = hitColliders[i].transform.GetComponent<GroundPiece>();
            if (ground && !ground.isColored)
            {
                ground.ChangeColor(solveColor);
            }
            i++;
        }

        if (nextCollisionPosition != Vector3.zero)
        {
            if (Vector3.Distance(transform.position, nextCollisionPosition) < 1)
            {
                isTravelling = false;
                travelDirection = Vector3.zero;
                nextCollisionPosition = Vector3.zero;
                audio.PlayOneShot(clip, 1f);
            }
        }

        if (isTravelling) { return; }

        if (Input.GetMouseButton(0))
        {
            swipePosCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if (swipePosLastFrame != Vector2.zero)
            {
                currentSwipe = swipePosCurrentFrame - swipePosLastFrame;

                //ignore mistake swiping i.e swipe less than 500
                if (currentSwipe.sqrMagnitude < minSwipeRecognition) { return; }

                currentSwipe.Normalize();
                Vector3 direction;
                //up/Down
                if (currentSwipe.x > -0.5f && currentSwipe.x < 0.5)
                {
                    // Go Up/Down
                    direction = currentSwipe.y > 0 ? Vector3.forward : Vector3.back;
                    SetDestination(direction);
                }

                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5)
                {
                    // Go Left/Right
                    direction = currentSwipe.x > 0 ? Vector3.right : Vector3.left;
                    SetDestination(direction);
                }
            }
            swipePosLastFrame = swipePosCurrentFrame;
        }
        if (Input.GetMouseButtonUp(0))
        {
            swipePosLastFrame = Vector2.zero;
            currentSwipe = Vector2.zero;
        }

    }

    private void SetDestination(Vector3 direction)
    {
        travelDirection = direction;

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 100f))
        {
            nextCollisionPosition = hit.point;
        }
        isTravelling = true;
    }
}
