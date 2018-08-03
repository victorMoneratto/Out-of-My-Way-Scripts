using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OMWGame.Scriptable {
    enum VariableInitMethod {
        Value, Reference
    }
    public class CustomAsset<T> : ScriptableObject {
#if UNITY_EDITOR
        [Multiline]
        public string Description = "";
#endif

        #region Initialization
        [HideInPlayMode]
        [SerializeField] private VariableInitMethod initializationMethod = VariableInitMethod.Value;
        
        [HideInPlayMode]
        [ShowIf("initializationMethod", VariableInitMethod.Value)]
        public T DefaultValue;
        
        [HideInPlayMode]
        [ShowIf("initializationMethod", VariableInitMethod.Reference)]
        public ScriptableObject DefaultValueReference;

        private bool initialized = false;
        #endregion
        
        [ShowInInspector, HideInEditorMode]
        private T value;

        public T Value {
            get {
                if (!initialized) {
                    OnEnable();
                }
                return value; 
            }
            set { this.value = value; }
        }

        private void OnEnable() {
            if (initialized) { return; }

            switch (initializationMethod) {
                case VariableInitMethod.Value:
                    value = DefaultValue;
                    break;
                
                case VariableInitMethod.Reference:
                    var reference = DefaultValueReference as CustomAsset<T>;
                    if (reference) {
                        value = reference.Value;
                    } else {
                        if (DefaultValueReference == null) {
                            Debug.LogError("Default value reference is null", this);
                        } else {
                            Debug.LogError("Default value reference is of wrong type: " +
                                           $"{DefaultValueReference.GetType()}. " +
                                           $"Should be {GetType()}", this);
                        }
                    }
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }

            initialized = true;
        }
    }
}