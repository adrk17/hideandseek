using System.ComponentModel;
using UnityEngine;

namespace MlAgents {
    public class InnerBaseTrigger : MonoBehaviour {
        
        [SerializeField]
        private int _seekersInBase;
        [SerializeField]
        private bool enableLogging;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Seeker"))
            {
                _seekersInBase++;
                if (enableLogging)
                {
                    Debug.Log($"New Seeker in base, _seekersInBase = {_seekersInBase}");
                }                
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Seeker"))
            {
                _seekersInBase--;
                if (enableLogging)
                {
                    Debug.Log($"Seeker Left the base, _seekersInBase = {_seekersInBase}");
                }               
            }
        }

        public int GetNumberOfSeekersInBase() {
            return _seekersInBase;
        }

        public bool AreSeekersInBase() {
            var result =  _seekersInBase != 0;
            if (enableLogging)
            {
                Debug.Log($"Value was read. Are Seekers In Base: {result}, seekers in base: {_seekersInBase}");
            }
            return result;
        }
    }
}