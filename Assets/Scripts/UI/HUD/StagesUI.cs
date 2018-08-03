using System.Collections.Generic;
using OMWGame.Scriptable;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace OMWGame{
    public class StagesUI : MonoBehaviour {
        [Title("Prefabs")]
        [SerializeField] private GameObject stageUIPrefab;
        [SerializeField] private GameObject playerUIPrefab;
        
        [Title("Match")]
        [SerializeField] private PlayersInMatch playersInMatch;
        [SerializeField] private GameObjectRunSet playersAlive;
        [SerializeField] private StageSetupRunSet stagesInOrder;
        [SerializeField] private IntReference currentStageIndex;
        
        [Title("Color")]
        [SerializeField] private TeamAsset teamOnTheLeft;
        [SerializeField] private TeamAsset teamOnTheRight;
        [SerializeField] private Color neutralStageColor = Color.white;
        
        
        private GameObject[] stages;
        
        /// Map InputID to GameObject (PlayerUI)
        private Dictionary<int, GameObject> players;

        private void OnEnable() {
            // create player ui
            int numPlayers = playersInMatch.Count;
            players = new Dictionary<int, GameObject>(numPlayers);
            for (int i = 0; i < numPlayers; i++) {
                var player = Instantiate(playerUIPrefab, transform);
                player.GetComponent<Image>().color = playersInMatch[i].Team.Value.Color;
                players[playersInMatch[i].InputID] = player;
            }
        }

        public void CreateUI(int numStages) {
            int firstStage = -numStages / 2;
            // create stages ui
            stages = new GameObject[numStages];
            for (int i = 0; i < stages.Length; i++) {
                stages[i] = Instantiate(stageUIPrefab, transform);
                stages[i].name = $"{firstStage + i:+#;-#;0} - {stageUIPrefab.name}";
            }
            
//            // create player ui
//            int numPlayers = playersInMatch.Count;
//            players = new Dictionary<int, GameObject>(numPlayers);
//            for (int i = 0; i < numPlayers; i++) {
//                var player = Instantiate(playerUIPrefab, transform);
//                player.GetComponent<Image>().color = playersInMatch[i].Team.Value.Color;
//                players[playersInMatch[i].InputID] = player;
//            }
        }

        public void MoveToStage(int currentStage) {
            currentStageIndex = currentStage;
            // color current
            var img = stages[currentStage].GetComponent<Image>();
            img.fillCenter = false;
            img.color = neutralStageColor;

            // color left half
            for (int i = 0; i < currentStage; i++) {
                img = stages[i].GetComponent<Image>();
                img.fillCenter = true;
                img.color = teamOnTheLeft.Value.Color;
            }

            // color right half
            for (int i = currentStage + 1; i < stages.Length; i++) {
                img = stages[i].GetComponent<Image>();
                img.fillCenter = true;
                img.color = teamOnTheRight.Value.Color;
            }
            
            // put player on current stage ui
            foreach (var player in players) {
                player.Value.transform.SetParent(stages[currentStage].transform, false);
            }
        }

        private void Update() {
            var stageGo = stagesInOrder[currentStageIndex].Object;
            var stagePos = stageGo.transform.position;
            
            // TODO @OMW calculate real bounds
            var stageBounds = new Vector3(90, 90, 0);
            var uiBounds = new Vector3(50, 25, 1);

            foreach (var player in playersAlive) {
                var playerID = player.GetComponent<PlayerInput>().PlayerID;
                var playerPos = player.transform.position;
                
                // TODO @OMW make a remap function
                Vector2 posAlpha = Vector2.zero;
                for (int i = 0; i < 2; i++) {
                    posAlpha[i] = Mathf.InverseLerp(stagePos[i] - stageBounds[i] * .5f, stagePos[i] + stageBounds[i] * .5f, playerPos[i]);    
                }

                Vector2 uiPos = Vector2.zero;
                for (int i = 0; i < 2; i++) {
                    uiPos[i] = Mathf.Lerp(-uiBounds[i] * .5f, uiBounds[i] * .5f, posAlpha[i]);    
                }
                
                players[playerID].transform.localPosition = new Vector3(uiPos.x, uiPos.y, 0);
            }
        }

        public void ShowPlayer(GameObject go) {
            var playerID = go.GetComponent<PlayerInput>().PlayerID;
            players[playerID].SetActive(true);
        }

        public void HidePlayer(GameObject go) {
            var playerID = go.GetComponent<PlayerInput>().PlayerID;
            players[playerID].SetActive(false);
        }
    }
}