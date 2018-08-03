using Rewired;
using UnityEngine;

namespace OMWGame {
    public class AddControllersToSystem : MonoBehaviour {

        public void Start() {
            AddControllers();
            ReInput.ControllerConnectedEvent += OnControllerConnected;
        }

        private void OnControllerConnected(ControllerStatusChangedEventArgs args) {
            AddControllers();
        }

        private void AddControllers() {
            var system = ReInput.players.GetSystemPlayer();
            foreach (var controller in ReInput.controllers.Controllers) {
                system.controllers.AddController(controller, false);
            }

            ReInput.controllers.Keyboard.enabled = true;
            system.controllers.hasKeyboard = true;
        }
    }
}