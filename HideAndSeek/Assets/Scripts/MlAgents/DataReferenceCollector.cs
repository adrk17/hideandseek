using System;
using System.Collections.Generic;
using UnityEngine;


namespace MlAgents
{
    public class DataReferenceCollector : MonoBehaviour
    {
        [Header("System references")]
        public GameManager gameManager;
        
        [Header("References for training")]
        public DoorController firstDoor;
        public DoorController secondDoor;

        public List<AgentMovement> seekers;
        public List<AgentMovement> hiders;

        public InnerBaseTrigger innerBaseTrigger;

        private int _agentsCount = 0;

        public int RegisterSeeker(AgentMovement agent)
        {
            seekers.Add(agent);
            return ++_agentsCount;
        }
        
        public int RegisterHider(AgentMovement agent)
        {
            seekers.Add(agent);
            return ++_agentsCount;
        }
        
        public int RegisterAgent(AgentMovement agent, bool isSeeker) {
            return isSeeker ? RegisterSeeker(agent) : RegisterHider(agent);
        }

        public AgentMovement GetSeeker(int id)
        {
            return seekers[id];
        }

        public AgentMovement GetHider(int id)
        {
            return hiders[id];
        }

        public List<AgentMovement> GetAllSeekers()
        {
            return seekers;
        }
        
        public List<AgentMovement> GetAllHiders()
        {
            return seekers;
        }

        public List<DoorController> GetAllDoors() {
            return new List<DoorController>() { firstDoor, secondDoor };
        }
    }
}