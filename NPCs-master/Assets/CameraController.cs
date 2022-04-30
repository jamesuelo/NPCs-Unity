#pragma warning disable
using UnityEngine;

public class CameraController : MonoBehaviour {
//script usado para poder controlar la camara
    [SerializeField] private float movementVelocity = 0.25f;    //velocidad de mov
    private Camera camera;
    private float horizontalMovement;                   //movimiento horizontal
    private float verticalMovement;                     //movimiento vertical

    void Start() {
        camera = GetComponent<Camera>();            //conseguimos el componente de la camara
    }

    void Update() {
        //obtenemos los ejes vertical y horizontal
        horizontalMovement = Input.GetAxis("Horizontal");
        verticalMovement = Input.GetAxis("Vertical");

        //si algun de los ejes esta siedno manipulado le sumamos las posiciones x y z de la camara en funcion de la velocidad
        if (horizontalMovement != 0 || verticalMovement != 0) {
            Vector3 position = transform.localPosition;

            position.x += horizontalMovement * movementVelocity;
            position.z += verticalMovement *movementVelocity;
            transform.localPosition = position;
        }
    }
}