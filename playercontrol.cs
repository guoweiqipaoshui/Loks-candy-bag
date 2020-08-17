using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playercontrol : MonoBehaviour
{
    private Rigidbody2D rb;//获得刚体
    private Animator anim;//获得动画
    public Collider2D coll;//获得玩家碰撞体，用于落地判定

    public float speed;//速度
    public float jumpforce;//跳跃力
    public LayerMask ground;//落地判定
    public AudioSource jumpAudio, hurtAudio,getCherry;

    public int cherry;//樱桃数量
    public Text CherryNum;//UI显示樱桃数量

    private bool isHurt;//默认false

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isHurt)
        {
            Movement();
        }
        Switchanim();
    }
    void Movement()//角色移动
    {
        float HorizontalMove = Input.GetAxis("Horizontal");
        float facedirection = Input.GetAxisRaw("Horizontal");
        if (HorizontalMove != 0)//左右移动
        {
            rb.velocity = new Vector2(HorizontalMove * speed * Time.fixedDeltaTime, rb.velocity.y);
            anim.SetFloat("running", Mathf.Abs(facedirection));
        }
        if (facedirection != 0)
        {
            transform.localScale = new Vector3(facedirection, 1, 1);//角色面向
        }
        if (coll.IsTouchingLayers(ground))
        {
            if (Input.GetButtonDown("Jump"))//跳跃
            {
                anim.SetBool("idle", false);
                rb.velocity = new Vector2(rb.velocity.x, jumpforce);
                jumpAudio.Play();
                anim.SetBool("jumpingup", true);
            }
        }
    }
    void Switchanim()//动画转换
    {
        anim.SetBool("idle", false);
        if(rb.velocity.y < 0.1f && !coll.IsTouchingLayers(ground))
        {
            anim.SetBool("jumpingdown", true);
        }

        if (anim.GetBool("jumpingup")) 
        {
            if (rb.velocity.y < 0) 
            {
                anim.SetBool("jumpingup", false);
                anim.SetBool("jumpingdown", true);
            }
        }else if(isHurt)//受伤动画切换
        {
            anim.SetBool("isHurt", true);
            anim.SetFloat("running", 0);
            if (Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                anim.SetBool("isHurt", false);
                anim.SetBool("idle", true);              
                isHurt = false;
            }
        }
        else if(coll.IsTouchingLayers(ground))
            {
                anim.SetBool("jumpingdown", false);
                anim.SetBool("idle", true);
            }

    }
    private void OnTriggerEnter2D(Collider2D collision)//收集物
    {
        if (collision.tag == "Collision")
        {
            getCherry.Play();
            Destroy(collision.gameObject);
            cherry += 1;
            CherryNum.text = cherry.ToString();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)//消灭敌人
    {
        if (collision.gameObject.tag == "Enemies")
        {
            Enemies enemy = collision.gameObject.GetComponent<Enemies>();
            if (anim.GetBool("jumpingdown"))
            {
                enemy.JumpOn();
                rb.velocity = new Vector2(rb.velocity.x,10);
            }
            else if (transform.position.x < collision.gameObject.transform.position.x)
            {
                hurtAudio.Play();
                rb.velocity = new Vector2(-10, rb.velocity.y);
                isHurt = true;
            }
            else if (transform.position.x > collision.gameObject.transform.position.x)
            {
                hurtAudio.Play();
                rb.velocity = new Vector2(10, rb.velocity.y);
                isHurt = true;
            }
        }
    }
}
