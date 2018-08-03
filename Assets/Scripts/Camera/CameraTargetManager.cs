using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using OMWGame.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace OMWGame {

	[RequireComponent(typeof(CinemachineTargetGroup))]
	public class CameraTargetManager : MonoBehaviour {
		private CinemachineTargetGroup cmGroup;

		[Serializable]
		public class TargetData {
			[HideInPlayMode]
			public AnimationCurve WeightInCurve = AnimationCurve.EaseInOut(0, 0, .5f, 1);
			[HideInPlayMode]
			public AnimationCurve WeightOutCurve = AnimationCurve.EaseInOut(0, 1, .25f, 0);

			public float Radius = 0;


			#region Transient data

			[NonSerialized, ShowInInspector, HideInEditorMode]
			public Transform Transform;

			[NonSerialized, ShowInInspector, HideInEditorMode]
			public bool CurrentlyOnEnteringAnimation = true;

			public AnimationCurve CurrentCurve => CurrentlyOnEnteringAnimation ? WeightInCurve : WeightOutCurve;

			[NonSerialized, ShowInInspector, HideInEditorMode]
			public float AnimationTime;

			#endregion
		}
		
		[ShowInInspector, HideInEditorMode]
		private List<TargetData> targets = new List<TargetData>();

		private void Awake() {
			cmGroup = GetComponent<CinemachineTargetGroup>();
		}


		/// GameObject is expected to have a CameraTarget component
		public void Add(GameObject go) {
			// TODO @OMW we won't take CameraTarget as parameter until parameterized events
			// of different types are easy enough to create and use
			var target = go.GetComponent<CameraTarget>();
			if (target) {
				var data = target.Data;

				// initialize transient data
				data.Transform = go.transform;
				data.AnimationTime = 0;
				data.CurrentlyOnEnteringAnimation = true;

				// search for item
				var index = targets.FindIndex(t => t.Transform == data.Transform);
				if (index >= 0) {
					// item already exists, just reset it data as if 
					// it was just added
					targets[index] = data;

					// Our targets array should be indexed the same way as CM's, this is a sanity check
					Assert.AreEqual(targets[index].Transform, cmGroup.m_Targets[index].target);
				} else {
					// item is new, add it to targets and cinemachine's transforms array
					targets.Add(data);

					// TODO @OMW how much garbage on a common match? 
					Array.Resize(ref cmGroup.m_Targets, cmGroup.m_Targets.Length + 1);
					index = cmGroup.m_Targets.Length - 1;
				}

				// set cm values
				cmGroup.m_Targets[index].target = data.Transform;
				cmGroup.m_Targets[index].radius = data.Radius;
				cmGroup.m_Targets[index].weight = data.CurrentCurve.Evaluate(data.AnimationTime);
			} else {
				Debug.LogError("Game Object should have CameraTarget component", go);
			}
		}

		/// GameObject is expected to have a CameraTarget component
		public void Remove(GameObject go) {
			// TODO @OMW we won't take CameraTarget as parameter until parameterized events
			// of different types are easy enough to create and use
			var target = go.GetComponent<CameraTarget>();
			if (target) {
				var data = target.Data;
				data.AnimationTime = 0;
				data.CurrentlyOnEnteringAnimation = false;

				StartCoroutine(RemoveFromArray(target, data.CurrentCurve.Time()));
			}
		}

		private IEnumerator RemoveFromArray(CameraTarget target, float waitTime) {
			yield return new WaitForSeconds(waitTime);
			
			var data = target.Data;
			int index = targets.FindIndex(t => t.Transform == data.Transform);
			if (index >= 0) {
				targets.RemoveAt(index);
					
				// note: I really wanted some Go slices now 
				var newCMTargets = new CinemachineTargetGroup.Target[cmGroup.m_Targets.Length-1];
					
				// TODO @OMW Replace copying loops with Buffer.Copy 
				// or Array.Copy if it ever becomes a problem
					
				// copy previous
				for (int i = 0; i < index; i++) {
					newCMTargets[i] = cmGroup.m_Targets[i];
				}
					
				// copy following
				for (int i = index+1; i < cmGroup.m_Targets.Length; i++) {
					newCMTargets[i-1] = cmGroup.m_Targets[i];
				}

				cmGroup.m_Targets = newCMTargets;
			}
		}

	private void Update() {
			// Our targets array should be indexed the same way as CM's, this is a sanity check
			Assert.AreEqual(targets.Count, cmGroup.m_Targets.Length);
			for (int i = 0; i < targets.Count; i++) {
				var target = targets[i];
				
				// Min so it's easier to see when it's done in inspector during play
				target.AnimationTime = Mathf.Min(target.AnimationTime + Time.deltaTime, target.CurrentCurve.Time());
				cmGroup.m_Targets[i].weight = target.CurrentCurve.Evaluate(target.AnimationTime);
			}
		}
	}
}