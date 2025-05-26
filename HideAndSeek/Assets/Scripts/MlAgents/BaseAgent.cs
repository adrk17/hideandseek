using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

namespace MlAgents
{
    public class BaseAgent : Agent
    {
        private readonly Dictionary<int, int> _agentInputToPlayerControls = new()
        {
            { 0, 0 },
            { 1, 1 },
            { 2, -1 },
            { -1, -1 },
        };

        private readonly Dictionary<int, int> _playerInputToAgentControls = new()
        {
            { 0, 0 },
            { 1, 1 },
            { -1, 2 },
        };


        private AgentMovement _agentMovement;
        private DataReferenceCollector  _dataReferenceCollector;

        private double _cumReward = 0;
        
        [SerializeField] private bool enableGradeLogging;

        [Header("What agent should observe")]
        public bool ownPosition;
        public bool ownVelocity;
        public bool seekersPosition;
        public bool hidersPosition;
        public bool doorsPosition;
        public bool doorsAgentsAmount;
        public bool doorsState;
        public bool leftHiders;
        public bool leftSeekers;
        public bool areSeekersInBase;

        [Header("Required references")] 
        public bool distanceToNearestHider;
        public float distanceToNearestHiderReward;
        public bool hiderCaught;
        public float hiderCaughtReward;
        public bool seekersInvadedBase;
        public float seekersInvadedBaseReward;
        public bool perSecondOfLife;
        public float perSecondOfLifeReward;
        public bool winByTimeout;
        public float winByTimeoutReward;
        public bool baseIsSecure;
        public float baseIsSecureReward;

        public bool IsActive { get; set; }
        private float _lastPositionCheck;
        private float _lastRewardGranted;
        private bool _baseWasInvaded = false;

        private const float _diagonalMapLength = 111; 
        
        public void Awake()
        {
        }

        public void Start()
        {
            _agentMovement = transform.GetComponent<AgentMovement>();
            _agentMovement = transform.GetComponent<AgentMovement>();
        }

        public void Update()
        {
        }

        public override void OnEpisodeBegin()
        {
            _cumReward = 0;
            _baseWasInvaded = false;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            if(ownPosition) 
            {
                sensor.AddObservation(_agentMovement.GetPosition());
            }

            if (ownVelocity)
            {
                sensor.AddObservation(_agentMovement.GetVelocity());
            }
            if(seekersPosition)
            {
                foreach (var seeker in _dataReferenceCollector.GetAllSeekers())
                {
                    sensor.AddObservation(seeker.GetPosition());
                }
            }
            if(hidersPosition) 
            {
                foreach (var hider in _dataReferenceCollector.GetAllHiders())
                {
                    sensor.AddObservation(hider.GetPosition());
                }
            }
            if(doorsPosition) 
            {
                foreach (var door in _dataReferenceCollector.GetAllDoors())
                {
                    sensor.AddObservation(door.GetPosition());
                }
                
            }
            if(doorsAgentsAmount) 
            {
                foreach (var door in _dataReferenceCollector.GetAllDoors())
                {
                    sensor.AddObservation(door.GetOccupiers().hidersAmount);
                    sensor.AddObservation(door.GetOccupiers().seekersAmount);
                }
            }
            if(doorsState) 
            {
                foreach (var door in _dataReferenceCollector.GetAllDoors())
                {
                    sensor.AddObservation(door.GetState());
                }
            }
            if(leftHiders)
            {
                int leftHidersCount = 0;
                foreach (var hider in _dataReferenceCollector.GetAllHiders())
                {
                    leftHidersCount += hider.IsAlive() ? 1 : 0;
                }
                sensor.AddObservation(leftHidersCount);
            }
            if(leftSeekers) 
            {
                int leftSeekersCount = 0;
                foreach (var seeker in _dataReferenceCollector.GetAllSeekers())
                {
                    leftSeekersCount += seeker.IsAlive() ? 1 : 0;
                }
                sensor.AddObservation(leftSeekersCount);
                 
            }
            if(areSeekersInBase) 
            {
                sensor.AddObservation(_dataReferenceCollector.GetSeekersInBaseCount());
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            _agentMovement.Move(
                new Vector2(
                    _agentInputToPlayerControls[actions.DiscreteActions[0]],
                    _agentInputToPlayerControls[actions.DiscreteActions[1]]
                    )
                );

            float rewardSum = 0;
            
            if (distanceToNearestHider) {
                float lowerDistanceToNearestHider = 0;
                foreach (var hider in _dataReferenceCollector.GetAllHiders())
                {
                    var distance = Vector3.Distance(hider.GetPosition(), _agentMovement.GetPosition());
                    if (distance < lowerDistanceToNearestHider)
                    {
                        lowerDistanceToNearestHider = distance;
                    }
                }
                rewardSum += - (lowerDistanceToNearestHider / _diagonalMapLength) * distanceToNearestHiderReward; ;
            }
            if (hiderCaught) {
                
            }
            if (seekersInvadedBase) {
                if (_baseWasInvaded == false && _dataReferenceCollector.GetSeekersInBaseCount() > 0 ) {
                    _cumReward += seekersInvadedBaseReward;
                    _baseWasInvaded = true;
                }
            }
            if (perSecondOfLife) {
                if (Time.time - _lastRewardGranted >= 1f)
                {
                    _lastRewardGranted = Time.time;
                }
            }
            if (winByTimeout) {
                
            }
            if (baseIsSecure) {
                if (_dataReferenceCollector.GetSeekersInBaseCount() <= 0) {
                    _cumReward += baseIsSecureReward;
                }
                
            }
            
            AddReward(rewardSum);
            _cumReward +=  rewardSum;
            Debug.Log($"Agent was rewarded by: {rewardSum}. Got in total: {_cumReward}");
        }
        
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
            Vector2 input = Vector2.zero;
            if (Input.GetKey(KeyCode.W)) input.x -= 1f;
            if (Input.GetKey(KeyCode.S)) input.x += 1f;
            if (Input.GetKey(KeyCode.D)) input.y += 1f;
            if (Input.GetKey(KeyCode.A)) input.y -= 1f;

            discreteActions[0] = _playerInputToAgentControls[(int)input.x];
            discreteActions[1] = _playerInputToAgentControls[(int)input.y];
        }
    }
}