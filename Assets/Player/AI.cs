using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System;
public class AI : MonoBehaviour
{
	Radar radar;
	AeroplaneController controller;
	TargetSeeker targetSeeker;
	Rigidbody body;
	PlayerSetup playerSetup;

	PlayerInput playerInput;
    Socket sender;

	public Vector3 desiredDirection;
    public string url = "http://localhost:9999";
    public float lastSent;
	void Start ()
	{
		radar = GetComponent<Radar> ();
		controller = GetComponent<AeroplaneController> ();
		targetSeeker = GetComponent<TargetSeeker> ();
		body = GetComponent<Rigidbody> ();
		playerSetup = GetComponent<PlayerSetup> ();

		playerInput = GetComponent<PlayerInput> ();

		desiredDirection = transform.forward;
        StartClient();
	}

    void StartClient()
    {
                // Data buffer for incoming data.
        byte[] bytes = new byte[1024];

        // Connect to a remote device.
        try {
            // Establish the remote endpoint for the socket.
            // This example uses port 11000 on the local computer.
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint remoteEP = new IPEndPoint(ipAddress,9090);

            // Create a TCP/IP  socket.
            sender = new Socket(AddressFamily.InterNetwork, 
                SocketType.Stream, ProtocolType.Tcp );

            // Connect the socket to the remote endpoint. Catch any errors.
            try {
                sender.Connect(remoteEP);

                Console.WriteLine("Socket connected to {0}",
                    sender.RemoteEndPoint.ToString());

                // Encode the data string into a byte array.
                // byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

                // Send the data through the socket.
                // int bytesSent = sender.Send(msg);

                // Receive the response from the remote device.
                // int bytesRec = sender.Receive(bytes);
                // Console.WriteLine("Echoed test = {0}",
                    // Encoding.ASCII.GetString(bytes,0,bytesRec));

                // Release the socket.
                // sender.Shutdown(SocketShutdown.Both);
                // sender.Close();

            } catch (ArgumentNullException ane) {
                Console.WriteLine("ArgumentNullException : {0}",ane.ToString());
            } catch (SocketException se) {
                Console.WriteLine("SocketException : {0}",se.ToString());
            } catch (Exception e) {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }

        } catch (Exception e) {
            Console.WriteLine( e.ToString());
        }
    }

    void SendMessage()
    {
        byte[] bytes = new byte[1024];
        // Encode the data string into a byte array.
        byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

        // Send the data through the socket.
        int bytesSent = sender.Send(msg);

        // Receive the response from the remote device.
        // int bytesRec = sender.Receive(bytes);
        // Console.WriteLine("Echoed test = {0}",
        //     Encoding.ASCII.GetString(bytes,0,bytesRec));

    }
    void Update ()
	{
        SendMessage();
        TurnBrakesOff ();

		desiredDirection = Vector3.zero;
		if (targetSeeker.target) {
			desiredDirection += Seek ();
		} else {
			desiredDirection += Wander ();
		}
		desiredDirection += StickAroundFriendlyGroundTargets ();
		desiredDirection += AvoidCollision ();
		desiredDirection += KeepAltitude ();
		desiredDirection.Normalize ();
		Attack ();
		Turn ();

		DebugDraw ();
	}


	//give airbrakes time to update
	float nextBrakesCheckTime;

	void TurnBrakesOff ()
	{
		if (controller.AirBrakes && Time.realtimeSinceStartup > nextBrakesCheckTime) {
			playerInput.inputToggleBrakes = true;
			nextBrakesCheckTime = Time.realtimeSinceStartup + 1;
		}
	}


	Vector3 wanderDirection = Vector3.up;
	float nextWanderDirectionSwitchTime;
	Vector3 Wander ()
	{
		if (Time.realtimeSinceStartup > nextWanderDirectionSwitchTime) {
			wanderDirection = UnityEngine.Random.insideUnitSphere;
			nextWanderDirectionSwitchTime = Time.realtimeSinceStartup + 10;
		}

		return wanderDirection;
	}


	Vector3 Seek ()
	{
		if (targetSeeker.target) {
			return (targetSeeker.target.position - transform.position).normalized;
		}
		return Vector3.zero;
	}


	Vector3 awayFromCollision = Vector3.zero;
	float flyAwayFromCollisionUntil = 0;
	Vector3 AvoidCollision ()
	{
		if (Time.realtimeSinceStartup > flyAwayFromCollisionUntil)
			awayFromCollision = Vector3.zero;

		float distance = 500;
		RaycastHit hit;
		bool colliderAhead = Physics.Raycast (transform.position + transform.forward * 10, 
			                     transform.forward, out hit, distance);

		if (colliderAhead) {
			awayFromCollision = -transform.forward * hit.distance * 0.1f;
			flyAwayFromCollisionUntil = Time.realtimeSinceStartup + 5;
		}

		return awayFromCollision;
	}

	Vector3 KeepAltitude ()
	{
		Vector3 dir = new Vector3 ();

		float lowAltitude = 500;
		if (controller.Altitude < lowAltitude)
			dir += Vector3.up * (lowAltitude - controller.Altitude);

		float highAltitude = 5000;
		if (controller.Altitude > highAltitude)
			dir += Vector3.down * (controller.Altitude - highAltitude);

		return dir;
	}


	Vector3 StickAroundFriendlyGroundTargets ()
	{
		GameObject closestFriendlyGroundTarget = null;

		foreach (GameObject target in radar.groundTargets) {
			if (target && target.GetComponent<GroundTarget> ().team == playerSetup.team) {
				if (closestFriendlyGroundTarget == null
					|| (Vector3.Distance(closestFriendlyGroundTarget.transform.position, transform.position)
						> Vector3.Distance(target.transform.position, transform.position))) {
					closestFriendlyGroundTarget = target;
				}
			}
		}

		if (closestFriendlyGroundTarget) {
			Vector3 toTarget = closestFriendlyGroundTarget.transform.position - transform.position;
			return toTarget * 0.001f;
		}
		return Vector3.zero;
	}


	float firePeriod = 10;
	float lastFireTime = 0;

	void Attack ()
	{
		if (targetSeeker.target) {
			Vector3 toTarget = targetSeeker.target.position - transform.position;
			if (Vector3.Angle (transform.forward, toTarget) < 10) {
				if (Time.realtimeSinceStartup - lastFireTime > firePeriod) {
					lastFireTime = Time.realtimeSinceStartup;
					playerInput.inputFire = true;
				}
			}
		}
	}

	//The following function was copied from AeroplaneAiControl.cs and modified
	void Turn ()
	{
		Vector3 targetPos = transform.position + desiredDirection;

		// adjust the yaw and pitch towards the target
		Vector3 localTarget = transform.InverseTransformPoint (targetPos);
		float targetAngleYaw = Mathf.Atan2 (localTarget.x, localTarget.z);
		float targetAnglePitch = -Mathf.Atan2 (localTarget.y, localTarget.z);

		// calculate the difference between current pitch and desired pitch
		float changePitch = targetAnglePitch - controller.PitchAngle;

		// AI always applies gentle forward throttle
		const float throttleInput = 0.5f;

		float m_PitchSensitivity = 0.5f;
		// AI applies elevator control (pitch, rotation around x) to reach the target angle
		float pitchInput = changePitch * m_PitchSensitivity;

		// clamp the planes roll
		float desiredRoll = targetAngleYaw;
		float yawInput = 0;
		float rollInput = 0;

		float m_RollSensitivity = 0.2f;
		yawInput = targetAngleYaw;
		rollInput = -(controller.RollAngle - desiredRoll) * m_RollSensitivity;


		// adjust how fast the AI is changing the controls based on the speed. Faster speed = faster on the controls.
		float m_SpeedEffect = 0.1f;
		float currentSpeedEffect = 1 + (controller.ForwardSpeed * m_SpeedEffect);
		rollInput *= currentSpeedEffect;
		pitchInput *= currentSpeedEffect;
		yawInput *= currentSpeedEffect;

		playerInput.inputRoll = rollInput;
		playerInput.inputPitch = pitchInput;
		playerInput.inputYaw = yawInput;
	}

	void DebugDraw ()
	{
		float len = 40;
		Debug.DrawRay (transform.position, desiredDirection * len, Color.yellow);
		Debug.DrawRay (transform.position, Vector3.ProjectOnPlane (desiredDirection, transform.forward).normalized * len, Color.gray);
	}
}
