using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : Photon.Pun.MonoBehaviourPun
{
     public float speed = 15;
     private Vector2 velocity;
     private Rigidbody2D rb;
     private void Awake() {
          rb = GetComponent<Rigidbody2D>();
          velocity = Vector2.zero;
     }
     private void Update() {
          velocity.x = Input.GetAxisRaw("Horizontal");
          velocity.y = Input.GetAxisRaw("Vertical");
     }
     private void FixedUpdate() {
          if (photonView.IsMine){
           rb.MovePosition(rb.position + 
                (velocity.normalized * speed *
                 Time.fixedDeltaTime)); 
          }
     }
}
