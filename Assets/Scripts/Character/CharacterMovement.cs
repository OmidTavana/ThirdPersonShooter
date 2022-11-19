using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    public GameObject Pistol;
    Guns SelectedGun;
    CharacterController characterController;
    Animator animator;
    ShooterGame inputActions;
    InputAction move;
    InputAction aim;
    InputAction run;
    InputAction switchGunAction;
    bool isRunning = false;
    bool isAiming = false;
    float AimValue = 0;
    public float AimSpeed = 3f;
    //Movement Speed
    float MoveSpeed = 0f;
    float ASpeed = 0f;
    float deacclerationASpeed = 0.5f;
    float rotationSpeed = 40f;
    float accelationASpeed = 0.4f;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        inputActions = new ShooterGame();
        move = inputActions.Player.Move;
        run = inputActions.Player.Run;
        aim = inputActions.Player.Aim;
        switchGunAction = inputActions.Player.SwitchGun;
        move.Enable();
        run.Enable();
        aim.Enable();
        switchGunAction.Enable();
        run.performed += Run_performed;
        run.canceled += Run_canceled;
        aim.performed += Aim_performed;
        aim.canceled += Aim_canceled;
        switchGunAction.started += SwitchGunAction_performed;
    }

    private void SwitchGunAction_performed(InputAction.CallbackContext obj)
    {
        SwitchGunsAnimation();
    }

    private void Aim_canceled(InputAction.CallbackContext obj)
    {
        isAiming = false;
    }

    private void Aim_performed(InputAction.CallbackContext obj)
    {
        isAiming = true;
    }

    private void Run_canceled(InputAction.CallbackContext obj)
    {
        isRunning = false;
    }

    private void Run_performed(InputAction.CallbackContext obj)
    {
        isRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveVale = move.ReadValue<Vector2>();
        //if move is performed
        if (moveVale.y > 0)
        {
            //active run animation
            if (isRunning)
            {
                if (ASpeed <= 1)
                    ASpeed += accelationASpeed * Time.deltaTime;

                if (MoveSpeed <= 9)
                    MoveSpeed += (accelationASpeed + 2) * Time.deltaTime;
            }
            //active walk animation
            else
            {
                ASpeed = 0.1f;
                MoveSpeed = 4.0f;
            }

        }
        //Run Idle Animation
        else
        {
            if (ASpeed > 0)
                ASpeed -= deacclerationASpeed * Time.deltaTime;
        }
        SwitchAiming();
        this.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime, 0));
        animator.SetFloat("Speed", ASpeed);
        characterController.Move(this.transform.forward * moveVale.y * MoveSpeed * Time.deltaTime);
    }
    void SwitchGunsAnimation()
    {
        int selectedIndex = (int)SelectedGun;
        if (selectedIndex == 1)
            selectedIndex = 0;
        else
            selectedIndex++;
        SelectedGun = (Guns)selectedIndex;
        animator.SetBool("SwitchPistol", true);
        StartCoroutine(SwitchGuns(1f));


    }
    
 

    IEnumerator SwitchGuns(float secs) {
        yield return new WaitForSeconds(secs);
        switch (SelectedGun)
        {
            case Guns.Nogun:
                Pistol.SetActive(false);
                break;
            case Guns.Pistol:
                Pistol.SetActive(true);
                break;
            default:
                break;
        }
        animator.SetBool("SwitchPistol", false);

    }
    void SwitchAiming()
    {
        if (isAiming == true && SelectedGun != Guns.Nogun)
        {
            if (AimValue <= 1)
                AimValue += AimSpeed * Time.deltaTime;
        }
        else
        {
            if (AimValue > 0)
                AimValue -= AimSpeed * Time.deltaTime;
        }
        animator.SetLayerWeight(1, AimValue);
    }
    public enum Guns
    {
        Nogun = 0,
        Pistol = 1
    }
}
