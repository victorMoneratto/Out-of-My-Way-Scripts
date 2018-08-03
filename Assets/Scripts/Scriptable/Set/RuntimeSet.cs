using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OMWGame.Scriptable {
    public class RuntimeSet<T> : ScriptableObject, IEnumerable<T> {
        [HideInPlayMode]
        public List<T> DefaultItems = new List<T>();

        [ShowInInspector, HideInEditorMode]
        public List<T> Items = new List<T>();

        private void OnEnable() {       
            Items.Clear(); // Needed?
            Items.AddRange(DefaultItems);
        }

        public void Add(T item) {
            if (!Items.Contains(item)) {
                Items.Add(item);
            }
        }

        public void Remove(T item) {
            Items.Remove(item);
        }

        public void ClearAndResize(int size) {
            Items = new List<T>(new T[size]);
        }


        #region List-like interface
        public T this[int i] {
            get { return Items[i]; }
            set { Items[i] = value; }
        }
        
        public int Count => Items.Count;
        public IEnumerator<T> GetEnumerator() { return Items.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return Items.GetEnumerator(); }
        #endregion
    }
}