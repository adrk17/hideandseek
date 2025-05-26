using UnityEngine;

namespace MlAgents {
    public class InnerBaseTrigger : MonoBehaviour {
        
        private int _seekersInBase;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Seeker"))
            {
                _seekersInBase++;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Seeker"))
            {
                _seekersInBase--;
            }
        }

        public int GetNumberOfSeekersInBase() {
            return _seekersInBase;
        }

        public bool AreSeekersInBase() {
            return _seekersInBase != 0;
        }
    }
}