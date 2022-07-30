using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI
{
    public class Node
    {
        public bool isRoot;
        public int evaluation;
        //public int key;
        public Move move;
        public Node parent;
        public List<Node> children;

        public Node()
        {
            isRoot = true;
            children = new List<Node>();
        }

        public Node(Move move, Node parent)
        {
            isRoot = false;
            this.move = move;
            this.parent = parent;
            children = new List<Node>();
        }

        public void AddChild(Node node)
        {
            children.Add(node);
        }

        public void SetEvaluation(int evaluation)
        {
            this.evaluation = evaluation;
        }

    }
}
