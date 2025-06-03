using UnityEngine;

namespace SojaExiles
{
    public class PlayerMovement : MonoBehaviour
    {
        public CharacterController controller;
        public Transform groundCheck;         // Check if player is on the ground
        public LayerMask groundMask;        

        public float speed = 5f;
        public float gravity = -9.81f;
        public float groundDistance = 0.4f;

        private Vector3 velocity;
        private bool isGrounded;

        void Update()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Prevent from accumulate jumping or gravity
            }

            // Player Input
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 move = transform.right * x + transform.forward * z;

            // Move Horizontally
            controller.Move(move * speed * Time.deltaTime);

            // Apply Gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}
