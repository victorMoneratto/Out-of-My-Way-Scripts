using OMWGame.Scriptable;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace OMWGame {
    public class PlayerSelect : MonoBehaviour {
        
        [SerializeField] private PlayersInMatch playersInMatch;
        
        /// <summary>
        /// UI "views" representing players that joined the match
        /// </summary>
        /// <remarks>
        /// Index should match the player id
        /// (e.g. playerViews[0] = Player 1, playerViews[1] = Player 2) 
        /// </remarks>
        [SerializeField] private PlayerSelectedView[] playerViews;

        private int numPlayersAssigned = 0;

        [SerializeField] private UnityEvent playersReadyEvent;

        
        [Title("Teams")]
        // disabling "assigned but not used" warning because we use
        // that assignment to copy team assets (e.g left = red)
        // and do nothing else with these references
        [SerializeField] private TeamAsset teamOnTheLeft;
        [SerializeField] private TeamAsset teamOnTheRight;
        
        // TODO @OMW colored teams should be refactored into 
        // some list that is a match setting  
        [SerializeField] private TeamAsset teamRed;
        [SerializeField] private TeamAsset teamGreen;

        private void Awake() {
            // harcoding red on the left and green on the right
            teamOnTheLeft.Value = teamRed.Value;
            teamOnTheRight.Value = teamGreen.Value;
            
            // reset controller and player data
            playersInMatch.Items.Clear();
            foreach (var player in ReInput.players.Players) {
                player.controllers.ClearAllControllers();
                player.isPlaying = false;
            }
        }

        private void Update() {
            if (!ReInput.isReady) { return; }

            AssignPlayers();
        }

        private void AssignPlayers() {
            var controllers = ReInput.controllers.Controllers;
            for (int iController = 0; iController < controllers.Count; iController++) {
                var controller = controllers[iController];
                
                // ignore mouse or controllers already assigned to someone
                if (controller.type == ControllerType.Mouse ||
                    IsControllerAssignedToAnyPlayer(controller)) {
                    continue;
                }

                if (controller.GetAnyButtonDown()) {
                    Player player = FindPlayerWithoutController();
                    if (player == null) { break; }
                    
                    // assign controller and set player as playing
                    player.controllers.AddController(controller, false);
                    player.isPlaying = true;
                    
                    // show selection on view
                    var team = playerViews[player.id].Selected(player.descriptiveName, controller.type);
                    
                    // setup player state
                    var playerState = new PlayerState() {
                        InputID = player.id,
                        Team = team
                    };
                    playersInMatch.Add(playerState);
                    
                    numPlayersAssigned++;
                    if (numPlayersAssigned == 2) {
                        playersReadyEvent.Invoke();
                    }
                }
            }
        }
        
        /// <summary>
        /// Find the first players that doesn't have any joystick or keyboard assigned 
        /// </summary>
        public static Player FindPlayerWithoutController() {
            var players = ReInput.players.Players;
            for (int iPlayer = 0; iPlayer < players.Count; iPlayer++) {
                var playerControllers = players[iPlayer].controllers;
                if (playerControllers.joystickCount == 0 && !playerControllers.hasKeyboard) {
                    return players[iPlayer];
                }
            }

            return null;
        }

        /// <summary>
        /// Similar to Rewired's IsControllerAssigned that doesn't check against the system player
        /// </summary>
        public static bool IsControllerAssignedToAnyPlayer(Controller controller) {
            for (int iPlayer = 0; iPlayer < ReInput.players.playerCount; iPlayer++) {
                var player = ReInput.players.Players[iPlayer];
                if (ReInput.controllers.IsControllerAssignedToPlayer(controller.type, controller.id, player.id)) {
                    return true;
                }
            }
            
            return false;
        }
        
    }
}