using System;
using OMWGame.Scriptable;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

namespace OMWGame {
    public class PlayerSelectedView : MonoBehaviour {
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text controllerNameText;
        [SerializeField] private Text pressToJoinText;
        [SerializeField] private GameObject mesh;
        
        public TeamAsset Selected(String playerName, ControllerType controllerType) {
            // TODO @OMW Localization
            string controllerName = "Controller";
            switch (controllerType) {
                case ControllerType.Keyboard: controllerName = "Keyboard"; break;
                case ControllerType.Joystick: controllerName = "Gamepad"; break;
            }
            
            // show player and controller texts
            controllerNameText.text = controllerName.ToUpper();
            controllerNameText.gameObject.SetActive(true);
            
            playerNameText.text = playerName.ToUpper();
            playerNameText.gameObject.SetActive(true);
            
            // hide join text
            pressToJoinText.gameObject.SetActive(false);
            
            mesh.SetActive(true);

            return GetComponent<TeamComponent>()?.TeamAsset;
        }
    }
}