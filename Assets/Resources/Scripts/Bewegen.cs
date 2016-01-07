using UnityEngine;
using System.Collections;

/// <summary>
/// Class used to translate player input(mouse and keyboard) to movement.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class Bewegen : MonoBehaviour
{
	#region Doxygen declarations
/**
  * @var maxSpeed
  * Public variable for the max speed.
  *
  * @var cooldown 
  * Cooldown for the clipping check.
  * 
  * @var speed 
  * Speed at which the player moves, can be edited with SetSpeed().
  * 
  * @var rotationSpeed
  * Speed at which the player rotates, can be edited with SetRotationSpeed().
  * 
  * @var vliegen
  * Bool to allow flying or not.
  * 
  * @var movingBack
  * Bool to show if the player is moving backwards.
  * 
  * @var counter
  * Counter used with cooldown.
  * 
  * @var xSpeed
  * Speed at which the player rotates around the X axis.
  * 
  * @var ySpeed
  * Speed at which the player rotates around the Y axis.
  * 
  * @var yMinLimit 
  * Highest angle the player can look at.
  * 
  * @var yMaxLimit
  * Lowest angle the player look at.
  * 
  * @var horizontal
  * Left and right player input.(A,D,Right arrow,Left arrow) 
  * 
  * @var vertical
  * Forward and backward player input.(W,S,Up arrow, Down arrow)
  * 
  * @var x
  * Default value for the X rotation.
  * 
  * @var y
  * Default value for the Y rotation.
  * 
  * Left and right player input.(A,D,Left arrow,Right Arrow)
  * 
  * @var maxMovingSpeed
  * Speed limiter for the player movement.
  * 
  * @var hit
  * Holds the information found by the ground raycast and the clipping linecast.
  * 
  * @var targetDirection
  * Holds the 0 to 1 Vector3 determined by player input.
  * 
  * @var currentRotation
  * Holds the current rotation , used to rotate from to a new rotation.
  *
  * @var defaultPos
  * Default position used when player wants to return to default location.
  *
  * @var e
  * The current event holder.
  */
	#endregion
	#region Public variables

	internal float maxSpeed = 0.5f;
	internal float rotationSpeed = 1f;
	internal bool vliegen = false;
    public Gui guiDirk;

	#endregion
	#region Private variables

	private float speed = 0.1f;
	private float xSpeed = 28f;
	private float ySpeed = 13f;
	private float x = 0.0f;
	private float y = 0.0f;
	private float yMinLimit = -40;
	private float yMaxLimit = 80;
	private float vertical;
	private float horizontal;
	private float maxMovingSpeed;
	private RaycastHit hit;
	private Vector3 targetDirection;
	private Vector3 defaultPos;
    private Event e;
    private Transform cam1;
    private Transform cam2;
    private Transform skyCam;

	#endregion   
	#region Unity functions

	/// <summary>
	/// Saves the begin position and rotation.
    /// Sets the movement speed and rotation speed.
	/// </summary>
	void Start () 
	{
        cam1 = transform.FindChild("First");
        cam2 = transform.FindChild("Second");
        skyCam = transform.FindChild("SkyRenderer");
        x = transform.eulerAngles.x;
        y = transform.eulerAngles.y;
        
        defaultPos = transform.position;

		SetSpeed(maxSpeed);
		SetRotationSpeed(rotationSpeed);
	  }

	/// <summary>
	/// Gets player input and move/rotate corrosponding to the input.
	/// </summary>
	void Update () 
	  {
		vertical = Input.GetAxisRaw("Vertical");
		horizontal = Input.GetAxisRaw("Horizontal");

		if (horizontal > 0.2f || vertical > 0.2f || horizontal < -0.2f || vertical < -0.2f)
		{

            if (e.shift)
            {
                SetSpeed(maxSpeed * 2);
                targetDirection = horizontal * transform.GetChild(0).right + vertical * transform.GetChild(0).forward;
            }
            else
            {
                SetSpeed(maxSpeed);

                targetDirection = horizontal * transform.GetChild(0).right + vertical * transform.GetChild(0).forward;
                Debug.Log(targetDirection.magnitude);

            }
		}
		else
		{
			SetSpeed(0);
		}

        Moving();

		if (Input.GetMouseButton(0) && !GuiEs.overGui && !guiDirk.selectS && !guiDirk.onGUI) { Rotating();}

		if (speed > maxMovingSpeed) { SetMaxSpeed(maxMovingSpeed); }
	}

    /// <summary>
    /// Just to get where the mouse is
    /// </summary>
    void OnGUI()
    {
        e = Event.current;
    }

#endregion
	#region Custom functions

	 /// <summary>
	/// Move the player.
    /// Corrects height if not flying.
    /// Make you go down if in the air.
	/// </summary>
	 void Moving()
	 {
		 if (!vliegen)
		 {
			 targetDirection.y = 0;
			 GetComponent<CharacterController>().SimpleMove(targetDirection * speed*10);
		 }

		 if(vliegen)
         {
			 if (Input.GetKey("space"))
			 {
                 if (!GetComponent<CharacterController>().isGrounded)
                 {
                     Vector3 downDirection = targetDirection -(transform.up*speed);
                     GetComponent<CharacterController>().SimpleMove(downDirection);
                 }
			 }
			 GetComponent<CharacterController>().Move(targetDirection* speed);
		 }
	 }

	 /// <summary>
	///  Gets mouse input.
    ///  Rotate the player corrosponding to the mouse input.
    ///  Lerp between old rotation and new rotation.
	/// </summary>
	 void Rotating()
	 {
         float tempX = x;
         float tempY = y;

         x += Input.GetAxis("Horizontal Mouse") * xSpeed * Time.fixedDeltaTime * (rotationSpeed / 10);
         y -= Input.GetAxis("Vertical Mouse") * ySpeed * Time.fixedDeltaTime * (rotationSpeed / 10);

		 y = ClampAngle(y, yMinLimit, yMaxLimit);

         Quaternion oldRotation = Quaternion.Euler(tempY, tempX, 0);
         Quaternion newRotation = Quaternion.Euler(y, x, 0);

		//for(int u = 0;u<transform.childCount;u++) 
		//{
        //    if (u > 0)
        //    {
        //        transform.GetChild(u - 1).rotation = Quaternion.Slerp(oldRotation, newRotation, Time.deltaTime);
        //    }
        //    else
        //    {
        //        transform.GetChild(u).rotation = Quaternion.Slerp(oldRotation, newRotation, Time.deltaTime);
        //    }
		//}


        //Quaternion parentRotation =
        //transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, tempX, 0), Quaternion.Euler(0, x, 0), Time.fixedDeltaTime);
        cam1.rotation = Quaternion.Slerp(oldRotation, newRotation,Time.fixedDeltaTime);
        cam2.rotation = Quaternion.Slerp(oldRotation, newRotation, Time.fixedDeltaTime);
        //skyCam.rotation = Quaternion.Euler(0, 0, 0);// Quaternion.Slerp(Quaternion.Euler(0, tempX, 0), Quaternion.Euler(0, x, 0), Time.fixedDeltaTime);
     }

	 /// <summary>
	 /// Set rotation to default.
	 /// </summary>
	 /// <param name="t">Object to apply default rotation to</param>
	 /// <param name="rotation">Default rotation</param>
	 public static void SetToDefault(Transform t, Quaternion rotation)
	 {
		 t.rotation = rotation;
	 }

	 /// <summary>
	 /// Set position to default.
	 /// </summary>
	 /// <param name="t">Object to apply default rotation to</param>
	 /// <param name="position">Default position</param>
	 public static void SetToDefault(Transform t, Vector3 position)
	 {
		 t.position = position;
	 }

	 /// <summary>
	/// Limit the speed.
	/// </summary>
	/// <param name="maxSpeed">Limit</param>
	 public void SetMaxSpeed(float maxSpeed)
	 {
		 maxMovingSpeed = maxSpeed;
	 }

	 /// <summary>
	/// Set the current speed.
	/// </summary>
	/// <param name="newSpeed">Current speed</param>
	 public void SetSpeed(float newSpeed)
	 {
		 speed = newSpeed;
	 }

	 /// <summary>
	/// Set the current rotation speed.
	/// </summary>
	/// <param name="rotationAngle">Current rotation speed</param>
	 public void SetRotationSpeed(float rotationAngle)
	 {
		 rotationSpeed = rotationAngle;
	 }

     /// <summary>
    /// Custom clamp function to bypass dead-lock.
    /// </summary>
    /// <param name="angle">The angle to clamp</param>
    /// <param name="min">The minimal angle</param>
    /// <param name="max">The maximal angle</param>
    /// <returns></returns>
	 public static float ClampAngle(float angle, float min, float max)
	 {
		 if (angle < -360F)
			 angle += 360F;
		 if (angle > 360F)
			 angle -= 360F;
		 return Mathf.Clamp(angle, min, max);
	 }
	#endregion
}
