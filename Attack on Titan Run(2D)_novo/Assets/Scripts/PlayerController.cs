using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
         // Variáveis de movimento
    public float moveSpeed = 5f;      // Velocidade de movimento
    public float jumpForce = 10f;     // Força do pulo

    // Verificação de chão
    private bool isGrounded = false;  // Checa se o jogador está no chão
    public Transform groundCheck;     // Ponto de verificação de contato com o chão
    public float groundCheckRadius = 0.2f; // Raio de verificação do chão
    public LayerMask whatIsGround;    // O que é considerado chão
    

    // Referência ao Rigidbody2D do jogador
    private Rigidbody2D rb;
    private GameController gm;
   

    // Variáveis para flipar o sprite
    private bool facingRight = true;

    void Start()
    {
        // Pegando a referência ao Rigidbody2D do jogador
        rb = GetComponent<Rigidbody2D>();
        gm = GameController.gameController;

    }

    void Update()
    {
        if(!gm.playing)return;
        
        // Movimento horizontal
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        //animator.SetFloat("Speed", Mathf.Abs(moveInput));
        //animator.SetBool("isJumping", !isGrounded);

        // Virar o personagem ao mudar de direção
        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }

        // Checa se o jogador está tocando o chão
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        // Pulo
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    // Função para inverter o sprite quando o personagem muda de direção
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1; // Inverte o valor de x
        transform.localScale = scaler;
    }

    public void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Enemy"){
            Debug.Log("Collided with enemy");
            //GameOverPanel.SetActive(true);
            gm.playing = false;
        }
    }

    //public GameObject GameOverPanel;
}
