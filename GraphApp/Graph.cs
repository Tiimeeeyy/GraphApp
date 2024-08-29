namespace GraphApp;

public class Graph
{
    public List<string> Nodes { get; private set; }
    public List<(string, string)> Edges { get; private set; }

    public Graph()
    {
        Nodes = new List<string>();
        Edges = new List<(string, string)>();
    }

    public void AddNode(string node)
    {
        if (!Nodes.Contains(node))
        {
            Nodes.Add(node);
        }
    }

    public void AddEdge(string node1, string node2)
    {
        if (Nodes.Contains(node1) && Nodes.Contains(node2) && !Edges.Contains((node1, node2)))
        {
            Edges.Add((node1, node2));
        }
    } 
}