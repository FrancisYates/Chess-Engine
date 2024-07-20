using System.Collections.Generic;

namespace ChessUI
{
    public record BookNode
    {
        public string Move { get; set; }
        public List<BookNode> Children { get; set; }
        public BookNode()
        {
            Children = [];
            Move = "0000";
        }

        public BookNode(string move)
        {
            this.Move = move;
            Children = [];
        }

        public void AddChild(BookNode node)
        {
            Children.Add(node);
        }

        public bool HasChild(string move)
        {
            if(Children.Count == 0) { return false; }
            foreach(BookNode node in Children)
            {
                if(node.Move == move) { return true; }
            }
            return false;
        }
        public BookNode GetChild(string move)
        {
            if (Children.Count == 0) { return null; }
            foreach (BookNode node in Children)
            {
                if (node.Move == move) { return node; }
            }
            return null;
        }
    }
}
