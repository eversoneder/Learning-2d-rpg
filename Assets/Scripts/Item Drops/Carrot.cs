using UnityEngine;

public class Carrot : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private float timeMove;

    [SerializeField] private bool isPlayerNearby = false;
    private GameObject player;

    private float timeCount;

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.Space))
        {
            player.GetComponent<PlayerItems>().CurrentCarrot++;
            player.GetComponent<PlayerAnim>().PlaySoundPickUpItem();
            Debug.Log("Carrot collected!");

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
            isPlayerNearby = true;
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            player = null;
        }
    }
}
