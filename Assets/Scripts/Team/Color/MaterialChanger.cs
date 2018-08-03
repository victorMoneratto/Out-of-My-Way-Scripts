using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace OMWGame {
    [RequireComponent(typeof(TeamComponent))]
    public class MaterialChanger : MonoBehaviour {
        [Serializable]
        public struct TeamMaterialChanger {
            public Renderer Target;
            public int MaterialIndex;
        }
        
        [SerializeField]
        private TeamMaterialChanger[] materialChangers;
        
        // Start and not Awake because the match mode will 
        // change teams as needed and awake is called upon
        // instantiation but start is called on first frame
        private void Start() {
            var team = GetComponent<TeamComponent>();
            Assert.IsTrue(team);
            
            foreach (var changer in materialChangers) {
                // TODO @OMW Changing material is not working for some reason,
                // only color is being changed for now
                
                // changer.Target.materials[changer.MaterialIndex] = team.Team.Material;
                changer.Target.materials[changer.MaterialIndex].color = team.Team.Color;
            }
        }
    }
}