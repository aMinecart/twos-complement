using Unity.VisualScripting;
using UnityEngine;

namespace BehaviorTree
{
    
    public abstract class Tree : MonoBehaviour
    {
        //Contains the entire tree
        private Node _root = null;

        //Builds the bahavior tree at start
        protected void Start()
        {
            _root = SetupTree();
        }

        //If there is a tree, evaluate it continuously
        private void Update()
        {
            if(_root != null) _root.Evaluate();
        }
        
        protected abstract Node SetupTree();
    }

}
