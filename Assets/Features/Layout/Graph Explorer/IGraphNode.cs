using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface INodeItem<T> where T : class
{
    void SetNode(GraphNode<T> Node);
}

public interface IGraphNode
{
    object Item { get; set; }
    IGraphNode ParentNode { get; set; }
    IEnumerable<IGraphNode> ChildNodes { get; }
    void AddChildNode(IGraphNode ChildNode);
    void RemoveChildNode(IGraphNode ChildNode);
}

public class GraphNode<T> : IGraphNode where T : class
{
    public List<GraphNode<T>> ChildNodes = new List<GraphNode<T>>();
    public GraphNode<T> ParentNode;

    private T _item;
    public T Item
    {
        get { return _item; }
        set
        {
            _item = value;
            var nodeItem = _item as INodeItem<T>;
            nodeItem.SetNode(this);
        }
    }

    #region -- IGraphNode --
    object IGraphNode.Item
    {
        get => _item;
        set
        {
            Item = value as T;
        }
    }

    IGraphNode IGraphNode.ParentNode
    {
        get => ParentNode;
        set
        {
            ParentNode = value as GraphNode<T>;
        }
    }

    IEnumerable<IGraphNode> IGraphNode.ChildNodes => ChildNodes;
    #endregion

    public GraphNode() { }

    public GraphNode(T Item, params GraphNode<T>[] Nodes)
    {
        this.Item = Item;

        foreach (var node in Nodes)
        {
            AddChildNode(node);
        }
    }

    public void AddChildNode(GraphNode<T> ChildNode)
    {
        ChildNodes.Add(ChildNode);
        ChildNode.ParentNode = this;
    }

    public void AddChildNode(IGraphNode ChildNode)
    {
        AddChildNode(ChildNode as GraphNode<T>);
    }

    public void RemoveChildNode(GraphNode<T> ChildNode)
    {
        ChildNode.ParentNode = null;
        ChildNodes.Remove(ChildNode);
    }

    public void RemoveChildNode(IGraphNode ChildNode)
    {
        RemoveChildNode(ChildNode as GraphNode<T>);
    }

    public GraphNode<T> GetChild(T Item)
    {
        return ChildNodes.FirstOrDefault(i => i.Item == Item);
    }

    /// <summary>
    /// Warning: This does not detect circular graph connections and will hang if used on one
    /// </summary>
    public GraphNode<D> Project<D>(Func<T, D> projector) where D : class
    {
        return new GraphNode<D>(projector(Item), ChildNodes.Select(child => child.Project(projector)).ToArray());
    }

    //public IEnumerable<GraphNode<T>> Flatten()
    //{
    //    yield return this;
    //    foreach(var child in ChildNodes)
    //    {
    //        foreach(var item in child.Flatten())
    //        {
    //            yield return item;
    //        }
    //    }
    //}

    public override string ToString()
    {
        return Item?.ToString() ?? base.ToString();
    }
}