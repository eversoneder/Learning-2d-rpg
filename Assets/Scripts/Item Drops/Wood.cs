using UnityEngine;

public class Wood : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private float timeMove;

    private bool playerIsNearby = false;
    private GameObject player;

    private float timeCount;

    private void Update()
    {
        if (playerIsNearby && Input.GetKeyDown(KeyCode.Space))
        {
            player.GetComponent<PlayerItems>().CurrentWood++;
            player.GetComponent<PlayerAnim>().PlaySoundPickUpItem();
            Debug.Log("Wood collected!");

            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeCount += Time.deltaTime;

        if (timeCount < timeMove)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsNearby = true;
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsNearby = false;
            player = null;
        }
    }
}
