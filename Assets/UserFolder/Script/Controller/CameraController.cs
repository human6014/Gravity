using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Contoller.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private float walkSpeed;

        [SerializeField]
        private float lookSensitivity;

        [SerializeField]
        private float cameraXRotationLimit;
        [SerializeField]
        private float cameraYRotationLimit;

        //private float currentCameraRotationX;
        private float currentCameraRotationY;

        [SerializeField]
        private UnityEngine.Camera mainCamera;
        private Rigidbody myRigid;

        void Start()
        {
            myRigid = GetComponent<Rigidbody>();
            currentCameraRotationY = 0;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()  // 컴퓨터마다 다르지만 대략 1초에 60번 실행
        {
            Move();                 // 1 키보드 입력에 따라 이동
            CameraRotation();       // 2️ 마우스를 위아래(Y) 움직임에 따라 카메라 X 축 회전 
            //CharacterRotation();    // 3 마우스 좌우(X) 움직임에 따라 캐릭터 Y 축 회전 
        }

        private void Move()
        {
            float _moveDirX = Input.GetAxisRaw("Horizontal");
            float _moveDirZ = Input.GetAxisRaw("Vertical");
            Vector3 _moveHorizontal = transform.right * _moveDirX;
            Vector3 _moveVertical = transform.forward * _moveDirZ;

            Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;

            //myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
        }

        public void SetRotation(Quaternion newRotation)
        {
            transform.rotation = newRotation;
            //currentCameraRotationX = 
        }
        private void CameraRotation()
        {
            //float _xRotation = Input.GetAxisRaw("Mouse X");
            float _yRotation = Input.GetAxisRaw("Mouse Y");
            //float _cameraRotationX = _xRotation * lookSensitivity;
            float _cameraRotationY = _yRotation * lookSensitivity;

            //currentCameraRotationX += _cameraRotationX;
            currentCameraRotationY -= _cameraRotationY;

            currentCameraRotationY = Mathf.Clamp(currentCameraRotationY, -cameraXRotationLimit, cameraXRotationLimit);
            //currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraYRotationLimit, cameraYRotationLimit);

            mainCamera.transform.localEulerAngles = new Vector3(currentCameraRotationY, 0, 0f);
        }

        private void CharacterRotation()  // 좌우 캐릭터 회전
        {
            float _yRotation = Input.GetAxisRaw("Mouse X");
            Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
            myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY)); // 쿼터니언 * 쿼터니언
            //Debug.Log(myRigid.rotation);             // Debug.Log(myRigid.rotation);  // 쿼터니언
            //Debug.Log(myRigid.rotation.eulerAngles); // Debug.Log(myRigid.rotation.eulerAngles); // 벡터
        }
    }
}
