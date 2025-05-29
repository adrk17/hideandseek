using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
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
        private GameManager _gameManager;

        private double _cumReward = 0;
        
        [SerializeField] private bool enableGradeLogging;
        [SerializeField] private bool enableDetailedGradeLogging;
        [SerializeField] private bool enableDetailedObservationLogging;

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
        public bool holdingDoor;
        public float holdingDoorReward;

        public bool IsActive { get; set; }
        private float _lastPositionCheck;
        private float _lastRewardGranted;
        private bool _baseWasInvaded = false;

        private const float _diagonalMapLength = 111;
        private int agentId;
        private bool _newHiderCaught = false;
        
        public void Awake()
        {
            var managers = GameObject.Find("GameManagers");
            _agentMovement = transform.GetComponent<AgentMovement>();
            _dataReferenceCollector = managers.GetComponent<DataReferenceCollector>();
            _gameManager = managers.GetComponent<GameManager>();
        }

        public void Start()
        {
            agentId = _dataReferenceCollector.RegisterAgent(_agentMovement, CompareTag("Seeker"));
            RegisterEvents();
        }

        private void OnDestroy() {
            DeregisterEvents();
        }

        public override void OnEpisodeBegin()
        {
            _cumReward = 0;
            _baseWasInvaded = false;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            String detailedObservationLog = "Agent Detailed Observations:";
            if(ownPosition) 
            {
                sensor.AddObservation(_agentMovement.GetPosition());
                detailedObservationLog += $"\n\tAgents position: {_agentMovement.GetPosition()}\n";
            }

            if (ownVelocity)
            {
                sensor.AddObservation(_agentMovement.GetVelocity());
                detailedObservationLog += $"\n\tAgents velocity: {_agentMovement.GetVelocity()}\n";
            }
            if(seekersPosition)
            {
                foreach (var seeker in _dataReferenceCollector.GetAllSeekers())
                {
                    sensor.AddObservation(seeker.GetPosition());
                    detailedObservationLog += $"\n\tSeeker position: {seeker.GetPosition()}\n";
                }
            }
            if(hidersPosition) 
            {
                foreach (var hider in _dataReferenceCollector.GetAllHiders())
                {
                    sensor.AddObservation(hider.GetPosition());
                    detailedObservationLog  += $"\n\tHider position: {hider.GetPosition()}\n";
                }
            }
            if(doorsPosition) 
            {
                foreach (var door in _dataReferenceCollector.GetAllDoors())
                {
                    sensor.AddObservation(door.GetPosition());
                    detailedObservationLog += $"\n\tDoor position: {door.GetPosition()}\n";
                }
                
            }
            if(doorsAgentsAmount) 
            {
                foreach (var door in _dataReferenceCollector.GetAllDoors())
                {
                    sensor.AddObservation(door.GetOccupiers().hidersAmount);
                    sensor.AddObservation(door.GetOccupiers().seekersAmount);
                    detailedObservationLog += $"\n\tDoor occupiers; seekers: {door.GetOccupiers().seekersAmount}, hiders: {door.GetOccupiers().hidersAmount}";
                }
            }
            if(doorsState) 
            {
                foreach (var door in _dataReferenceCollector.GetAllDoors())
                {
                    sensor.AddObservation(door.GetState());
                    detailedObservationLog += $"\n\tDoor state: {door.GetState()}\n";
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
                detailedObservationLog += $"\n\tHiders left: {leftHidersCount}";
            }
            if(leftSeekers) 
            {
                int leftSeekersCount = 0;
                foreach (var seeker in _dataReferenceCollector.GetAllSeekers())
                {
                    leftSeekersCount += seeker.IsAlive() ? 1 : 0;
                }
                sensor.AddObservation(leftSeekersCount);
                detailedObservationLog += $"\n\tSeekers left: {leftSeekersCount}";
                 
            }
            if(areSeekersInBase) 
            {
                sensor.AddObservation(_dataReferenceCollector.innerBaseTrigger.AreSeekersInBase());
                detailedObservationLog += $"\n\tAreSeekersInBase: {_dataReferenceCollector.innerBaseTrigger.AreSeekersInBase()}";
            }

            if (enableDetailedObservationLogging)
            {
                Debug.Log(detailedObservationLog);
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

            String detailedGradeLog = "Detailed Grade Log:";
            
            if (distanceToNearestHider) {
                float lowerDistanceToNearestHider = float.MaxValue;
                foreach (var hider in _dataReferenceCollector.GetAllHiders())
                {
                    var distance = Vector3.Distance(hider.GetPosition(), _agentMovement.GetPosition());
                    if (distance < lowerDistanceToNearestHider)
                    {
                        lowerDistanceToNearestHider = distance;
                    }
                }

                var distanceToNearestHiderRewardResult =  - (lowerDistanceToNearestHider / _diagonalMapLength) * distanceToNearestHiderReward;
                detailedGradeLog += $"\n\tdistance to nearest hider: {distanceToNearestHiderRewardResult}";
                rewardSum += distanceToNearestHiderRewardResult;
            }
            if (hiderCaught) {
                var hiderCaughtRewardResult = 0f;
                if (_newHiderCaught) {
                    hiderCaughtRewardResult += hiderCaughtReward;
                    _newHiderCaught = false;
                }

                rewardSum += hiderCaughtRewardResult;
                detailedGradeLog += $"\n\tHider Caught: {hiderCaughtRewardResult}";
            }
            if (seekersInvadedBase) {
                var seekersInvadedRewardResult = 0f;
                if (_baseWasInvaded == false && _dataReferenceCollector.innerBaseTrigger.AreSeekersInBase() ) {
                    seekersInvadedRewardResult += seekersInvadedBaseReward;
                    _baseWasInvaded = true;
                }
                rewardSum += seekersInvadedRewardResult;
                detailedGradeLog += $"\n\tSeekers invaded: {seekersInvadedRewardResult}";
            }
            if (perSecondOfLife)
            {
                var perSecondOfLifeRewardResult = 0f;
                    perSecondOfLifeRewardResult += perSecondOfLifeReward;
                    _lastRewardGranted = Time.time;

                rewardSum += perSecondOfLifeRewardResult;
                detailedGradeLog +=  $"\n\tPer second of life: {perSecondOfLifeRewardResult}";
            }
            if (baseIsSecure)
            {
                var baseIsSecureRewardResult = 0f;
                if (!_dataReferenceCollector.innerBaseTrigger.AreSeekersInBase()) {
                    baseIsSecureRewardResult += baseIsSecureReward;
                }
                rewardSum += baseIsSecureRewardResult;
                detailedGradeLog += $"\n\tBase is Secure: {baseIsSecureRewardResult}";
            }
            if (holdingDoor)
            {
                var holdingDoorRewardResult = 0f;
                foreach (var door in _dataReferenceCollector.GetAllDoors())
                {
                    var occupiers = door.GetOccupiers();
                    if (occupiers.hidersAmount != 0 && occupiers.seekersAmount != 0) {
                        if (occupiers.hidersAmount == occupiers.seekersAmount) {
                            holdingDoorRewardResult += holdingDoorReward;
                        }
                    }
                }
                rewardSum += holdingDoorRewardResult;
                detailedGradeLog += $"\n\tHolding door Secure: {holdingDoorRewardResult}";
            }
            
            AddReward(rewardSum);
            _cumReward +=  rewardSum;
            
            if (enableGradeLogging) {
                Debug.Log($"Agent was rewarded by: {rewardSum}. Got in total: {_cumReward}");
            }

            if (enableDetailedGradeLogging) {
                detailedGradeLog += $"\n\tAgent was rewarded by: {rewardSum}. Got in total: {_cumReward}";
                Debug.Log(detailedGradeLog);
            }
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
        
        private void RegisterEvents() {
            _gameManager.onHiderCaught.AddListener(RegisterHiderCaught);
            _gameManager.onGameEnd.AddListener(RegisterGameEnded);
        }

        private void DeregisterEvents() {
            _gameManager.onHiderCaught.RemoveListener(RegisterHiderCaught);
            _gameManager.onGameEnd.RemoveListener(RegisterGameEnded);
        }

        private void RegisterHiderCaught() {
            _newHiderCaught = true;
        }
        
        private void RegisterGameEnded(bool byTimeout) {
            if (winByTimeout)
            {
                var winByTimeoutRewardResult = 0f;
                if (byTimeout) {
                    winByTimeoutRewardResult += winByTimeoutReward;
                }

                _cumReward += winByTimeoutRewardResult;
                AddReward(winByTimeoutRewardResult);
                if (enableDetailedGradeLogging)
                {
                    Debug.Log($"Agent was rewarded by win by timeout: {winByTimeoutRewardResult}");
                }

                if (enableGradeLogging)
                {
                    Debug.Log($"Agent was rewarded by: {winByTimeoutRewardResult}. Got in total: {_cumReward}");
                }
            }
            EndEpisode();
        }
    }
}