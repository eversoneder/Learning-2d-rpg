using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public float speed;
    private int index;
    public List<Transform> paths = new List<Transform>();
    private float initialspeed;

    private Animator anim;

    private void Start()
    {
        initialspeed = speed;
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (DialogueControl.Instance.IsDialogueShowing)
        {
            speed = 0f;
            anim.SetBool("isWalking", false);
        } else
        {
            speed = initialspeed;
            anim.SetBool("isWalking", true);
        }



            transform.position = Vector2.MoveTowards(transform.position, paths[index].position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, paths[index].position) < 0.1f)
        {
            if (index < paths.Count - 1)
            {
                index = Random.Range(0, paths.Count - 1); ;
            } else
            {
                index = 0;
            }
        }
        Vector2 direction = paths[index].position - transform.position;

        if (direction.x > 0)
        {
            transform.eulerAngles = new Vector2(0,0);
        }
        if (direction.x < 0)
        {
            transform.eulerAngles = new Vector2(0, 180);
        }
    }
}
