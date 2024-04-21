using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class MLScript : Agent
{
    public float jumpForce = 20f;
    public float minVelocityLimit = 0.005f;

    public GameObject target;
    private Rigidbody agentRb;
    private Rigidbody targetRb;

    public float negativeReward = 2f;
    public float positiveReward = 1f;

    private Vector3 spawnpoint = Vector3.zero;

    public override void Initialize()
    {
        agentRb = GetComponent<Rigidbody>();
        targetRb = target.GetComponent<Rigidbody>();
        spawnpoint = transform.position;

        agentRb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(target.transform.localPosition);
        sensor.AddObservation(transform.localPosition);

        // Agent velocity
        sensor.AddObservation(agentRb.velocity.y);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int jumpAction = actions.DiscreteActions[0];


        if (jumpAction > 0)
        {
            Debug.Log("Received action: " + jumpAction);
            // Jump action
            Jump();
        }
    }


    public override void OnEpisodeBegin()
    {
        // Reset agent position and velocity
        transform.position = spawnpoint;
        agentRb.velocity = Vector3.zero;

        // Change target's velocity along the negative z-axis each episode
        //targetRb.velocity = new Vector3(0f, 0f, Random.Range(-15f, -20f));
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        // Spacebar key for jump action
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;

    }

    private void Jump()
    {
        if (Mathf.Abs(agentRb.velocity.y) < minVelocityLimit)
        {
            Debug.Log("Jumping!");
            //agentRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else
        {
            Debug.Log("Cannot jump: Already in the air or y-velocity is too high.");
        }
        agentRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("obstacle"))
        {
            // Punish for collision with obstacle
            AddReward(-negativeReward);
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("points"))
        {
            // Reward for touching the designated points object
            SetReward(positiveReward);
            EndEpisode();
        }
    }
}