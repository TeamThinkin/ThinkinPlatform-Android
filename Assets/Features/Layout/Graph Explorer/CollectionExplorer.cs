using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CollectionExplorer : MonoBehaviour
{
    [SerializeField] private GraphExplorer Graph;
    [SerializeField] private CollectionList List;

    private GraphNode<CollectionNodeDto> rootNode;

    void Start()
    {
        UserInfo.OnCurrentUserChanged += UserInfo_OnCurrentUserChanged;
        Graph.NodeSelected += Graph_NodeSelected;
        Initialize();
    }

    private void OnDestroy()
    {
        UserInfo.OnCurrentUserChanged -= UserInfo_OnCurrentUserChanged;
        Graph.NodeSelected -= Graph_NodeSelected;
    }

    private void UserInfo_OnCurrentUserChanged(UserInfo obj)
    {
        Initialize();
    }

    private async void Initialize()
    {
        if (UserInfo.CurrentUser == null) return;

        await CollectionManager.UserHomeCollection.Item.PopulateItems();

        rootNode = new GraphNode<CollectionNodeDto>(new CollectionNodeDto("Available Collections"),
            CollectionManager.UserHomeCollection,
            CollectionManager.PublicCollection);

        Graph.SetGraphRoot(rootNode);
        //Graph.SetGraphRoot(getMockGraph());
    }

    private async void Graph_NodeSelected(NodeViewModel SelectedNodeViewModel)
    {
        var node = SelectedNodeViewModel.Node as GraphNode<CollectionNodeDto>;

        List.SetItems(node.Item.Items);

        await Task.WhenAll(node.ChildNodes.Select(i => i.Item.PopulateItems()));
    }

    #region -- Mock Graph Structure --
    private GraphNode<CollectionNodeDto> getMockGraph()
    {
        return getThinkinMockGraph();
    }


    private GraphNode<CollectionNodeDto> getThinkinMockGraph()
    {
        var graph = new GraphNode<CollectionNodeDto>(new CollectionNodeDto("My Collections"),
            new GraphNode<CollectionNodeDto>(new CollectionNodeDto("UCS"),
                new GraphNode<CollectionNodeDto>(new CollectionNodeDto("Improv for Engineers"),
                    new GraphNode<CollectionNodeDto>(new CollectionNodeDto("Classroom"),
                        new GraphNode<CollectionNodeDto>(new CollectionNodeDto("Board Room")),
                        new GraphNode<CollectionNodeDto>(new CollectionNodeDto("Theater")),
                        new GraphNode<CollectionNodeDto>(new CollectionNodeDto("Hotel")))),
                new GraphNode<CollectionNodeDto>(new CollectionNodeDto("Other Such Classes"))
            ),

            new GraphNode<CollectionNodeDto>(new CollectionNodeDto("Medtronic"),
                new GraphNode<CollectionNodeDto>(new CollectionNodeDto("Quantum Doodad"))),

            new GraphNode<CollectionNodeDto>(new CollectionNodeDto("Arts District"),
                new GraphNode<CollectionNodeDto>(new CollectionNodeDto("Missy's Gallery")),
                new GraphNode<CollectionNodeDto>(new CollectionNodeDto("Linquist Gallery")),
                new GraphNode<CollectionNodeDto>(new CollectionNodeDto("Music Machines"),
                    new GraphNode<CollectionNodeDto>(new CollectionNodeDto("Music Machine 1")),
                    new GraphNode<CollectionNodeDto>(new CollectionNodeDto("Music Machine 2")),
                    new GraphNode<CollectionNodeDto>(new CollectionNodeDto("Music Machine 3"))))
        );

        //graph.ChildNodes[0].ChildNodes[2].ChildNodes[2].ChildNodes[2].AddNode(graph);
        return graph;
    }

    //private GraphNode<CollectionNodeDto> getGenericMockGraph()
    //{
    //    var graph = new GraphNode<NodeDto>(new NodeDto("My Collections"),
    //        new GraphNode<NodeDto>(new NodeDto("Paramotor"),
    //            new GraphNode<NodeDto>(new NodeDto("Airfields"),
    //                new GraphNode<NodeDto>(new NodeDto("Kansas City Metro"),
    //                    new GraphNode<NodeDto>(new NodeDto("Gardner")),
    //                    new GraphNode<NodeDto>(new NodeDto("Louisburg")),
    //                    new GraphNode<NodeDto>(new NodeDto("3-EX")))),
    //            new GraphNode<NodeDto>(new NodeDto("Pictures"))
    //        ),

    //        new GraphNode<NodeDto>(new NodeDto("Family"),
    //            new GraphNode<NodeDto>(new NodeDto("Vacations"),
    //                new GraphNode<NodeDto>(new NodeDto("2018"),
    //                    new GraphNode<NodeDto>(new NodeDto("2019"),
    //                        new GraphNode<NodeDto>(new NodeDto("2020"),
    //                            new GraphNode<NodeDto>(new NodeDto("2020"))))))),

    //        new GraphNode<NodeDto>(new NodeDto("Freelance / Art"),
    //            new GraphNode<NodeDto>(new NodeDto("The Met - Projection Mapping")),
    //            new GraphNode<NodeDto>(new NodeDto("How to Train Your Dragon")),
    //            new GraphNode<NodeDto>(new NodeDto("Installations"),
    //                new GraphNode<NodeDto>(new NodeDto("Virtual Music Machine #3")),
    //                new GraphNode<NodeDto>(new NodeDto("Rocking Racers")),
    //                new GraphNode<NodeDto>(new NodeDto("Flying Circus"))))
    //    );

        //graph.GetChild("Arts District").GetChild("Music Machines").GetChild("Music Machine 2").AddNode(mapRoot);
    //    return graph;
    //}
    #endregion
}
