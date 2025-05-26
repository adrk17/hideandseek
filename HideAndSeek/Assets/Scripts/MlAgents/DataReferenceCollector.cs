using System;
using System.Collections.Generic;
using UnityEngine;


namespace MlAgents
{
    public class DataReferenceCollector : MonoBehaviour
    {
        public DoorController firstDoor;
        public DoorController secondDoor;

        public List<AgentMovement> seekers;
        public List<AgentMovement> hiders;

        public InnerBaseTrigger innerBaseTrigger;
        
        public int RegisterSeeker(AgentMovement agent)
        {
            var currCount = seekers.Count;
            seekers.Add(agent);
            return currCount;
        }
        
        public int RegisterHider(AgentMovement agent)
        {
            var currCount = seekers.Count;
            seekers.Add(agent);
            return currCount;
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