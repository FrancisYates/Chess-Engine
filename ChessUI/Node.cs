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
        public Move move;
        public List<Node> children;

        public Node()
        {
            isRoot = true;
            children = new();
        }

        public Node(Move move)
        {
            isRoot = false;
            this.move = move;
            children = new();
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
