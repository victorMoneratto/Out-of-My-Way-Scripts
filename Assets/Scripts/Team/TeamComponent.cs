using OMWGame.Scriptable;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OMWGame {
    public class TeamComponent : MonoBehaviour {
        [SerializeField] private TeamAsset teamAsset;
        public TeamAsset TeamAsset {
            get { return teamAsset; }
            set { teamAsset = value; }
        }
        
        public Team Team => teamAsset ? teamAsset.Value : LookForTeam();
        public bool HasTeam => Team != null;
        
        [HideIf("HasTeam")]
        [SerializeField] private TeamComponent parent;
        
        /// <summary>
        /// Look for a team on:
        /// 1) Parent
        /// 2) Instigator
        /// </summary>
        /// <returns>team found</returns>
        private Team LookForTeam() {
            // 1) Parent
            if (parent && parent.Team != null) {
                return parent.Team;
            }
            
            // 2) Instigator
            var dmgr = GetComponent<IDamager>();
            if (dmgr != null && dmgr.Instigator) {
                var dmgTeam = dmgr.Instigator.GetComponent<TeamComponent>();
                return dmgTeam.Team;
            }

            return null;
        }
    }
}