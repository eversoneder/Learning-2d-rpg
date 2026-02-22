using System.Collections;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] private float MaxTreeHealth;
    [SerializeField] private float treeHealth;
    [SerializeField] private bool isTreeDead;
    [SerializeField] private Animator anim;
    [SerializeField] private float respawnTime;
    [SerializeField] private float logDestroyTime;
    [SerializeField] private GameObject woodPrefab;
    [SerializeField] private int totalWood;
    [SerializeField] private ParticleSystem leafsPS;

    private bool hasDroppedWood;

    private GameObject playerGameObject;
    private PlayerAnim playerAnim;

    private void Awake()
    {
        MaxTreeHealth = treeHealth;
        playerGameObject = GameObject.FindGameObjectWithTag("Player");
        playerAnim = playerGameObject.GetComponent<PlayerAnim>();

        if (logDestroyTime == 0)
        {
            logDestroyTime = 2f; 
        }
    }

    public void OnHit()
    {
        //tree is alive
        if (treeHealth >= 1)
        {
            anim.SetTrigger("isHit");
            leafsPS.Play();
            treeHealth--;
            anim.SetBool("isTreeAlive", true);
        }

        //tree is dead
        if (treeHealth <= 0)
        {
            //wood hasn't been dropped yet? drop only once, and give dead tree attributes
            if (!hasDroppedWood)
            {
                //dead tree attributes
                anim.SetBool("isTreeAlive", false); // Tree is dead
                anim.SetTrigger("cut");
                isTreeDead = true;

                //drop wood
                for (int i = 0; i < totalWood; i++)
                {
                    GameObject droppedWood = Instantiate(woodPrefab, transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(2f, -2f), 0f), transform.rotation);
                    
                    Destroy(droppedWood, logDestroyTime);
                }
                playerAnim.PlaySFX(playerAnim.ItemDropSound);
                hasDroppedWood = true;
            }

            StartCoroutine(TreeRespawn(respawnTime));
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Axe") && !isTreeDead)
        {
            //Debug.Log("bateu");
            OnHit();
        }
    }

    private IEnumerator TreeRespawn(float time)
    {
        yield return new WaitForSeconds(time);

        treeHealth = MaxTreeHealth;
        isTreeDead = false;
        hasDroppedWood = false;

        // Reset any lingering triggers
        anim.ResetTrigger("cut");
        anim.ResetTrigger("isHit");

        anim.SetBool("isTreeAlive", true);
        anim.SetTrigger("respawn");
    }
}
