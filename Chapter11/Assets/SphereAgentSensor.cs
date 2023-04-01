using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class SphereAgentSensor : Agent {

    Rigidbody rBody;
    public Transform Target;
    public Transform Sensore;
    private Vector3 initPos;
    private float initLocaly;
    
    public Transform Agento;
    public float forceMultiplier = 10;

    void Start() {
        rBody = Agento.GetComponent<Rigidbody>();
        initPos = Agento.position;
    }

    override public void OnEpisodeBegin() {
        
        ////print(Agento.position.y);
            
        if (Agento.position.y < -1.0) {
            
            //print("start");
            // The agent fell
            Agento.position = Vector3.zero;
            rBody.angularVelocity = Vector3.zero;
            rBody.velocity = Vector3.zero;
            // Move the target to a new spot
            Target.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
            
        } else {
            //print("HEY");
            //print(Agento.position.y);
            // Move the target to a new spot
            Target.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
        }
    }

    override public void CollectObservations(VectorSensor sensor) {
        // Calculate relative position
        //Vector3 relativePosition = Target.position - Agento.position;
        // Relative position
        //sensor.AddObservation(relativePosition.x/5);
        //sensor.AddObservation(relativePosition.z / 5);
        // Distance to edges of platform
        sensor.AddObservation((Agento.position.x + 5) / 5);
        sensor.AddObservation((Agento.position.x - 5) / 5);
        sensor.AddObservation((Agento.position.z + 5) / 5);
        sensor.AddObservation((Agento.position.z - 5) / 5);
        // agent velocity
        sensor.AddObservation(rBody.velocity.x / 5);
        sensor.AddObservation(rBody.velocity.z / 5);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers) {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        // Rewards
        float distanceToTarget = Vector3.Distance(Agento.localPosition, Target.localPosition);

        // Reached target

        if (distanceToTarget < 1.42f) {
            
            SetReward(1.0f);
            EndEpisode();
        }

        // Fell off platform
        else if (Agento.position.y < -1.0) {
            
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    // Update is called once per frame
    void Update() {

       

       Sensore.position = new Vector3(Agento.position.x, Agento.position.y, Agento.position.z);

    }
}
