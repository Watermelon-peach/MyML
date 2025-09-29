using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
	#region Variables
	private Rigidbody rBody;
    public Transform target;
    public float forceMultiplier = 10f;
    #endregion

    #region Unity Event Method
    private void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }
    #endregion

    #region MLAgents
    public override void OnEpisodeBegin()
    {
        if (transform.localPosition.y < 0)
        {
            rBody.angularVelocity = Vector3.zero;
            rBody.linearVelocity = Vector3.zero;
            transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        target.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localPosition);

        sensor.AddObservation(rBody.angularVelocity.x);
        sensor.AddObservation(rBody.angularVelocity.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];

        rBody.AddForce(controlSignal * forceMultiplier);

        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);

        if(distanceToTarget <1.42)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        if(transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }

    #endregion
}
