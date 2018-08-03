using System;
using System.Linq;
using OMWGame.Scriptable;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OMWGame {
    public class MatchMode : MonoBehaviour {
        [Title("Match")]
        public PlayersInMatch PlayersInMatch;
        
        
        
        [Title("Players")]
        [SerializeField] private Transform playersParent;
        
        public GameObjectRunSet AlivePlayers;
        public GameObjectRunSet DeadPlayers;

        public GameObjectRunSet TrappedPlayers;
         
        
        
        [Title("Stages")]
        [SerializeField] private Transform stagesParent;
        public StageSetupRunSet StageProgression;
        public StageSetupRunSet StagesInOrder;
        
        public IntReference CurrentStageIndex;
        private int nextStageIndex = -1;
        public int CenterStageIndex => StagesInOrder.Count() / 2;

        [SerializeField] private IntEvent stagesCreatedEvent;
        [SerializeField] private IntEvent currentStageChangedEvent;


        [Title("Teams")]
        [SerializeField] private TeamAsset teamOnTheLeft;
        [SerializeField] private TeamAsset teamOnTheRight;
        
        
        
        public void Start() {
            CreateStages();

            var startStage = CenterStageIndex;
            #if UNITY_EDITOR 
            if (CurrentStageIndex >= 0) {
                startStage = CurrentStageIndex;
                if (startStage != CenterStageIndex) {
                    Debug.Log("Starting match on non-center stage");
                }
            }
            #endif
            
            SetCurrentStage(startStage);

            CreatePlayers();
            
            SpawnCraftsInCurrentStage();

            
            // In editor, if we run the game scene directly, we need to
            // perform auto assign, this is it, a poor man's auto assign
            // actually, rewired's isn't working for some reason
            // (probably because all controllers are assigned to system?)
            #if UNITY_EDITOR
            bool controllersAlreadySetup = false;
            foreach (var player in ReInput.players.Players) {
                player.isPlaying = true;
                if (player.controllers.Controllers.Any()) {
                    controllersAlreadySetup = true;
                    break;
                }
            }
            
            if (!controllersAlreadySetup) {
                foreach (var controller in ReInput.controllers.Controllers) {
                    if (controller.type != ControllerType.Mouse) {
                        var player = PlayerSelect.FindPlayerWithoutController();
                        if (player == null) { break; }
                        player.controllers.AddController(controller, false);
                    }
                }
            }
            #endif
            
            ReInput.configuration.autoAssignJoysticks = true;
        }
        
        
        #region Players
        private void CreatePlayers() {
            // Create players for all stages
            for (int iStage = 0; iStage < StagesInOrder.Count; iStage++) {
                var stage = StagesInOrder[iStage];
                int stageNum = (-StagesInOrder.Count / 2) + iStage;
                
                // Create players for this stage
                stage.Object.CraftsFromThisStage = new GameObject[PlayersInMatch.Count];
                for (int iPlayer = 0; iPlayer < PlayersInMatch.Count; iPlayer++) {
                    var player = PlayersInMatch[iPlayer];
                    
                    // Find space craft for this player
                    var arsenal = Array.Find(stage.Arsenals, a => a.Team.Value == player.Team.Value);
                    if (arsenal != null) {
                        // TODO Select appropriate crafts for multiple players per team
                        var prefab = arsenal.Crafts[0];
                        stage.Object.CraftsFromThisStage[iPlayer] = InstantiatePlayer(prefab, player,
                            $"{stageNum:+#;-#;0} - {player.Team.name} - {prefab.name}");
                    } else {
                        Debug.LogError("Arsenal not found for team " + player.Team, stage);
                    }
                }
            }
        }
        
        private GameObject InstantiatePlayer(GameObject prefab, PlayerState state, string playerName) {
            // create player object that should be enabled only on spawn
            var player = Instantiate(prefab, playersParent);
            player.SetActive(false);
            
            player.name = playerName;
            player.GetComponent<TeamComponent>().TeamAsset = state.Team;
            player.GetComponent<PlayerInput>().PlayerID = state.InputID;
            
            return player;
        }
        
        // TODO @OMW Move different implementations to scriptable object
        // (e.g. Team Deathmatch, Free for all)
        public bool CanDamage(IDamager damager, GameObject receiver) {
            // TODO @OMW allow draws as they were super fun on prototype
            
            // if we're moving to some other level (so someone won), don't receive damage
            // to avoid draws (I don't have time to implement draws for now)
            if (nextStageIndex >= 0) {
                return false;
            }
            
            if (damager != null && damager.Instigator) {
                GameObject instigator = damager.Instigator;

                var instigatorTeam = instigator.GetComponent<TeamComponent>()?.Team;
                var receiverTeam = receiver.GetComponent<TeamComponent>()?.Team;

                if (receiverTeam != null && instigatorTeam != null) {
                    return receiverTeam != instigatorTeam;
                }
            }

            return true;
        }

        public void PlayerSpawned(GameObject go) {
            
        }
        
        public void PlayerDied(GameObject go) {
            CheckForStageWinner();
        }
        
        // TODO @OMW abstract for different game modes
        public void CheckForStageWinner() {
            if (AlivePlayers.Any()) {
                // if only players from one team are alive, they won
                bool won = true;
                var winningTeam = AlivePlayers[0].GetComponent<TeamComponent>().Team;
                for (int i = 1; i < AlivePlayers.Count(); i++) {
                    var team = AlivePlayers[i].GetComponent<TeamComponent>().Team;
                    won &= (team == winningTeam);
                }
                
                if (won) {
                    BeginStageTransition(winningTeam);
                }
            } else {
                Debug.Log("Draw (but how? that is not implemented D:)");
            }
        }

        public void CheckForMatchWinner() {
            if (TrappedPlayers.Count > 0 &&
                TrappedPlayers.Count == AlivePlayers.Count) {
                var team = TrappedPlayers[0].GetComponent<TeamComponent>().Team;
                
                Debug.Log(team.Name + " Won The Match");
            } 
        }

        private void SpawnCraftsInCurrentStage() {
            var currentStage = StagesInOrder[CurrentStageIndex];
            
            // don't spawn on ending stage
            if (currentStage.Object.EndingStage) { return; }
            
            // TODO @OMW if we decide not to allow a tie, we maybe have to find
            // a way (bool force argument) to force spawning on ending stage,
            // in case all players die when the last stage is already taken 
            
            // copy spawners array so we can null spawners already used,
            // can be optimized if needed 
            var spawners = new PlayerSpawner[currentStage.Object.Spawners.Length];
            Array.Copy(currentStage.Object.Spawners, spawners, spawners.Length);
            
            // spawn all crafts
            foreach (var craft in currentStage.Object.CraftsFromThisStage) {
                // TODO @OMW optimize getcomponents away
                var playerID = craft.GetComponent<PlayerInput>().PlayerID;
                if (AlivePlayers.Items.Exists(go => go.GetComponent<PlayerInput>().PlayerID == playerID)) {
                    // if craft is for a player already alive, skip it
                    continue;
                }
                
                // find available spawner for the same team
                var craftTeam = craft.GetComponent<TeamComponent>().Team;
                int spawnerId = Array.FindIndex(spawners, s => s != null && s.Team == craftTeam);
                
                // position craft on spawner
                if (spawnerId >= 0) {
                    var spawner = spawners[spawnerId];
                    craft.transform.position = spawner.transform.position;
                    craft.transform.rotation = spawner.transform.rotation;
                } else {
                    Debug.LogError("Can\'t find spawner for team " + craftTeam);
                }
                
                // revive
                craft.GetComponent<Health>().Revive();
            }
        }
        #endregion
        
        
        #region Stages
        private void CreateStages() {
            // TODO @OMW Calc stage bounds
            var stageBounds = new Vector3(90, 90, 10);
            var center = Vector3.zero;
            
            // all but the first stage from progression are duplicated
            int numStages = (StageProgression.Count * 2) - 1;
            StagesInOrder.ClearAndResize(numStages);
            
            // create first stage
            StagesInOrder[CenterStageIndex] = InstantiateStage(StageProgression[0], center,
                                                   $"0 - {StageProgression[0].name}");
            
            for (int iProgression = 1; iProgression < StageProgression.Count; iProgression++) {
                var currentProgression = StageProgression[iProgression];
                var offsetFromCenter = new Vector3(iProgression * stageBounds.x, 0, 0);
                
                // create stage on the left
                int onTheLeft = CenterStageIndex - iProgression;
                StagesInOrder[onTheLeft] = InstantiateStage(currentProgression, 
                                                     center - offsetFromCenter,
                                                    $"{-iProgression} - {currentProgression.name}");
                
                // create stage on the right
                int onTheRight = CenterStageIndex + iProgression;
                StagesInOrder[onTheRight] = InstantiateStage(currentProgression, 
                                                      center + offsetFromCenter,
                                                      $"{iProgression:+#} - {currentProgression.name}");
            }

            if (stagesCreatedEvent) {
                stagesCreatedEvent.Raise(numStages);
            }
        }

        private StageSetup InstantiateStage(StageSetup prefab, Vector3 stagePos, string stageName) {
             var newStage = ScriptableObject.CreateInstance<StageSetup>();
            
            // stage gameobject
            newStage.Object = Instantiate(prefab.Object.gameObject, stagesParent).GetComponent<Stage>();
            newStage.Object.transform.position = stagePos;
            newStage.Object.name = stageName;
            
            // arsenal
            newStage.Arsenals = new StageSetup.Arsenal[prefab.Arsenals.Length];
            Array.Copy(prefab.Arsenals, newStage.Arsenals, prefab.Arsenals.Length);
            
            return newStage;
        }
        
        private void BeginStageTransition(Team winningTeam) {
            var winningSide = Side.Left;
            if (winningTeam == teamOnTheLeft.Value) {
                winningSide = Side.Left;
            } else if (winningTeam == teamOnTheRight.Value) {
                winningSide = Side.Right;
            } else {
                Debug.LogError("Winning team didn't match Team on the left nor right");
            }
            
            
            switch (winningSide) {
                case Side.Left: nextStageIndex = CurrentStageIndex + 1; break;
                case Side.Right: nextStageIndex = CurrentStageIndex - 1; break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (nextStageIndex < StagesInOrder.Count) {
                StagesInOrder[CurrentStageIndex].Object.OpenGate(winningSide.GetOpposite());
                StagesInOrder[nextStageIndex].Object.OpenGate(winningSide);
                StagesInOrder[nextStageIndex].Object.SafeZone.BeginWaitingForPlayers();
            } else {
                Debug.Log("Trying to move to a stage beyond we have");
            }
        }

        public void EndStageTransition() {
            if (nextStageIndex < 0) {
                Debug.LogError("Next Stage Index is invalid, was BeginStageTransition called?");
                return;
            }
            
            {
                var currStage = StagesInOrder[CurrentStageIndex].Object;
                var nextStage = StagesInOrder[nextStageIndex].Object;
                var sideMoving = nextStageIndex > CurrentStageIndex ? Side.Right : Side.Left; 
                currStage.CloseGate(sideMoving);
                nextStage.CloseGate(sideMoving.GetOpposite());
            }
            
            SetCurrentStage(nextStageIndex);
            nextStageIndex = -1;
            
            SpawnCraftsInCurrentStage();
        }

        private void SetCurrentStage(int i) {
            CurrentStageIndex = i;
            currentStageChangedEvent.Raise(i);
        }
        #endregion
        
        
        #if false
        private void Update() {
            
            // Debug revive players
            bool debug = false;
            foreach (var player in ReInput.players.Players) {
                debug |= player.GetButtonDown("Revive");
            }
    
            if (debug) {
                #if false
                for (int i = DeadPlayers.Count() - 1; i >= 0; i--) {
                    DeadPlayers[i].GetComponent<Health>()?.Revive();
                }
                #else
                SceneManager.LoadScene("Game");
                #endif
            }
        }
        #endif
    }
}