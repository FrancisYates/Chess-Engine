using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessUI
{
    public class BookNode
    {
        public string rootMove;
        public BookNode parent;
        public List<BookNode> children;

        public BookNode()
        {
            children = new List<BookNode>();
        }

        public BookNode(string move, BookNode parent)
        {
            this.rootMove = move;
            this.parent = parent;
            children = new List<BookNode>();
        }

        public void AddChild(BookNode node)
        {
            children.Add(node);
        }

        public bool HasChild(string move)
        {
            if(children.Count == 0) { return false; }
            foreach(BookNode node in children)
            {
                if(node.rootMove == move) { return true; }
            }
            return false;
        }
        public BookNode GetChild(string move)
        {
            if (children.Count == 0) { return null; }
            foreach (BookNode node in children)
            {
                if (node.rootMove == move) { return node; }
            }
            return null;
        }
    }
}
