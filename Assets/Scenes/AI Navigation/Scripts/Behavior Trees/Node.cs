using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{

    public enum NodeState
    {
        RUNNING, SUCCESS, FAILURE
    }

    public class Node
    {
        //Protected to other classes can modify the node's state
        protected NodeState state;

        public Node parent;
        protected List<Node> children = new List<Node>();

        //Hashmap with string as the key.
        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        //Default node constructor
        public Node()
        {
            parent = null;
        }

        public Node(List<Node> children)
        {
            //Iterates through all children, attaching all of them so they all have edges to one another
            foreach (Node child in children) _Attach(child);
        }

        //Attaches node to its new child, creating an edge
        private void _Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        //Virtual so all classes attached can override Evaluate with their own version
        public virtual NodeState Evaluate() => NodeState.FAILURE;

        //Adds to _dataContext dictionary
        public void SetData(string key, object value)
        {
            //_dataContext[key] = value;
            _dataContext.Add(key, value);
        }

        //Gets object associated with the key of a node
        public object GetData(string key)
        {
            object value = null;
            if(_dataContext.TryGetValue(key, out value)) return value; //Checks current node

            //Checks other nodes dictionaries
            Node node = parent;
            while(node != null)
            {
                value = node.GetData(key);
                if(value != null) return value;
                node = node.parent;
            }
            return null;
        }

        //Removes object associated with the key of a node
        public bool ClearData(string key)
        {
            if(_dataContext.ContainsKey(key)) //Removes if on current node
            {
                _dataContext.Remove(key);
                return true;
            }

            //Checks other nodes dictionaries
            Node node = parent;
            while(node != null)
            {
                if(node._dataContext.ContainsKey(key))
                {
                    node._dataContext.Remove(key);
                    return true;
                }
                node = node.parent;
            }
            return false;
        }


    }

}
