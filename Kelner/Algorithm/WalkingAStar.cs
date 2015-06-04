// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WalkingAStar.cs" company="">
//   
// </copyright>
// <summary>
//   Algorytm porusznia się po mapie przy użyciu przeszukiwnaiu przestrrzeni stanów A*
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kelner.Algorithm
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Kelner.Model;

    public class Node
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Node ParentNode { get; set; }

        public int CountCostF(Node targetNode)
        {
            var h = (Math.Abs(this.X - targetNode.X) + Math.Abs(this.Y - targetNode.Y)) * 10;
            var g = this.CountCostG();

            return g + h;
        }

        public int CountCostG()
        {
            if (this.ParentNode == null)
            {
                return 0;
            }

            int cost;

            if (this.ParentNode.X == this.X || this.ParentNode.Y == this.Y)
            {
                cost = 10 + this.ParentNode.CountCostG();
            }
            else
            {
                cost = 14 + this.ParentNode.CountCostG();
            }

            return cost;
        }

        public List<Node> GetPath()
        {
            var path = this.GetParentsPath();
            path.Reverse();

            return path;
        }

        public List<Node> GetParentsPath()
        {
            if (this.ParentNode == null)
            {
                return new List<Node>();
            }

            var path = new List<Node> { this };
            path.AddRange(this.ParentNode.GetParentsPath());

            return path;
        } 
    }

    public class WalkingAStar
    {
        private List<Node> openNodes;

        private List<Node> closedNodes;

        public WalkingAStar()
        {
            this.openNodes = new List<Node>();
            this.closedNodes = new List<Node>();
        }

        public List<Node> GetPath(Node currentWaiterNode, Node targetNode, Section[][] restaurantSections)
        {
            if (currentWaiterNode.X == targetNode.X && currentWaiterNode.Y == targetNode.Y)
            {
                return new List<Node>();
            }

            this.openNodes.Add(currentWaiterNode);
            
            var accesibleNodes = this.GetAccesibleNodes(currentWaiterNode, restaurantSections);
            this.openNodes.AddRange(accesibleNodes);

            this.CloseNode(currentWaiterNode);

            var nextNode = this.openNodes.OrderBy(n => n.CountCostF(targetNode)).FirstOrDefault();
            this.CloseNode(nextNode);

            while (nextNode != null && (nextNode.X != targetNode.X || nextNode.Y != targetNode.Y))
            {
                var nextAccesibleNodes = this.GetAccesibleNodes(nextNode, restaurantSections);

                // dodanie nowych węzłów, których nie było wcześniej w liście otwartych
                foreach (var nextAccesibleNode in nextAccesibleNodes)
                {
                    if (this.openNodes.Any(n => n.X == nextAccesibleNode.X && n.Y == nextAccesibleNode.Y))
                    {
                        var oldOpenNode = this.openNodes.FirstOrDefault(n => n.X == nextAccesibleNode.X && n.Y == nextAccesibleNode.Y);

                        if (oldOpenNode != null && nextAccesibleNode.CountCostG() < oldOpenNode.CountCostG())
                        {
                            oldOpenNode.ParentNode = nextNode;
                        }

                        continue;
                    }

                    this.openNodes.Add(nextAccesibleNode);
                }

                nextNode = this.openNodes.OrderBy(n => n.CountCostF(targetNode)).FirstOrDefault();
                this.CloseNode(nextNode);
            }
             
            if (nextNode != null)
            {
                return nextNode.GetPath();
            }

            return new List<Node>();
        }

        // Zwraca listę węzłów (pól w restauracji) do których można się dostać z aktualnej pozycji
        private List<Node> GetAccesibleNodes(Node startNode, Section[][] restaurantSections)
        {
            var neighbouringNodes = new List<Node>();

            neighbouringNodes.Add(new Node
            {
                X = startNode.X - 1,
                Y = startNode.Y,
                ParentNode = startNode
            });

            neighbouringNodes.Add(new Node
            {
                X = startNode.X - 1,
                Y = startNode.Y - 1,
                ParentNode = startNode
            });

            neighbouringNodes.Add(new Node
            {
                X = startNode.X - 1,
                Y = startNode.Y + 1,
                ParentNode = startNode
            });

            neighbouringNodes.Add(new Node
            {
                X = startNode.X,
                Y = startNode.Y - 1,
                ParentNode = startNode
            });

            neighbouringNodes.Add(new Node
            {
                X = startNode.X,
                Y = startNode.Y + 1,
                ParentNode = startNode
            });

            neighbouringNodes.Add(new Node
            {
                X = startNode.X + 1,
                Y = startNode.Y,
                ParentNode = startNode
            });

            neighbouringNodes.Add(new Node
            {
                X = startNode.X + 1,
                Y = startNode.Y - 1,
                ParentNode = startNode
            });

            neighbouringNodes.Add(new Node
            {
                X = startNode.X + 1,
                Y = startNode.Y + 1,
                ParentNode = startNode
            });

            // usunięcie węzłów znajdujących się na liście zamnkiętych
            if (this.closedNodes.Any())
            {
                var tmpNeighbouringNodes = new List<Node>(neighbouringNodes);
                foreach (var tmpNeighbouringNode in tmpNeighbouringNodes)
                {
                    if (this.closedNodes.Any(n => n.X == tmpNeighbouringNode.X && n.Y == tmpNeighbouringNode.Y))
                    {
                        neighbouringNodes.Remove(
                            neighbouringNodes.FirstOrDefault(
                                n => n.X == tmpNeighbouringNode.X && n.Y == tmpNeighbouringNode.Y));
                    }
                }
            }

            // przypisanie tych, które nie wychodzą poza mapę
            var accesibleNodes = neighbouringNodes.Where(n => n.X >= 0 && n.X < 10 && n.Y >= 0 && n.Y < 10).ToList();

            // usunięcie tych, które mają przeszkodę na mapie
            var tmpAccesibleNodes = new List<Node>(accesibleNodes);
            foreach (var tmpAccesibleNode in tmpAccesibleNodes)
            {
                if (!restaurantSections[tmpAccesibleNode.X][tmpAccesibleNode.Y].CanEnter())
                {
                    accesibleNodes.Remove(
                        accesibleNodes.FirstOrDefault(n => n.X == tmpAccesibleNode.X && n.Y == tmpAccesibleNode.Y));
                }
            }

            return accesibleNodes;
        }

        private void CloseNode(Node nodeToClose)
        {
            var openNode = this.openNodes.FirstOrDefault(n => n.X == nodeToClose.X && n.Y == nodeToClose.Y);

            if (openNode != null)
            {
                this.closedNodes.Add(openNode);
                this.openNodes.Remove(openNode);
            }
        }
    }
}
