using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using xpidea.neuro.net.patterns;
using xpidea.neuro.net.backprop;
using xpidea.neuro.net;
using System.IO;
using System.Xml;

namespace Kelner.Algorithm
{
    class NeuralNet : BackPropagationRPROPNetwork
    {


            public NeuralNet(int[] nodesInEachLayer): base(nodesInEachLayer)
            {

            }
            private int OutputPatternIndex(Pattern pattern)
            {
                for (int i = 0; i < pattern.OutputsCount; i++)
                    if (pattern.Output[i] == 1)
                        return i;
                return -1;
            }

            public void AddNoiseToInputPattern(int levelPercent)
            {
                int i = ((NodesInLayer(0) - 1) * levelPercent) / 100;
                while (i > 0)
                {
                    nodes[(int)(BackPropagationNetwork.Random(0, NodesInLayer(0) - 1))].Value = BackPropagationNetwork.Random(0, 100);
                    i--;
                }

            }

            public int BestNodeIndex
            {
                get
                {
                    int result = -1;
                    double aMaxNodeValue = 0;
                    double aMinError = double.PositiveInfinity;
                    for (int i = 0; i < this.OutputNodesCount; i++)
                    {
                        NeuroNode node = OutputNode(i);
                        if ((node.Value > aMaxNodeValue) || ((node.Value >= aMaxNodeValue) && (node.Error < aMinError)))
                        {
                            aMaxNodeValue = node.Value;
                            aMinError = node.Error;
                            result = i;
                        }

                    }
                    return result;
                }
            }

            /* Trenowanie sieci */
            public override void Train(PatternsCollection patterns)
            {

                int iteration = 0;
                if (patterns != null)
                {
                    double error = 0;
                    int good = 0;
                    while (good < patterns.Count) // Train until all patterns are correct
                    {
                        error = 0;
                        good = 0;
                        for (int i = 0; i < patterns.Count; i++)
                        {
                            for (int k = 0; k < NodesInLayer(0); k++)
                                nodes[k].Value = patterns[i].Input[k];
                            this.Run();
                            for (int k = 0; k < this.OutputNodesCount; k++)
                            {
                                error += Math.Abs(this.OutputNode(k).Error);
                                this.OutputNode(k).Error = patterns[i].Output[k];
                            }
                            this.Learn();
                            if (BestNodeIndex == OutputPatternIndex(patterns[i]))
                                good++;

                            iteration++;
                        }

                        foreach (NeuroLink link in links) ((EpochBackPropagationLink)link).Epoch(patterns.Count);

                        if ((iteration % 2) == 0)
                            Console.WriteLine("AVG Error: " + (error / OutputNodesCount).ToString() + "  Iteration: " + iteration.ToString());
                    }
                    Console.WriteLine("AVG Error: " + (error / OutputNodesCount).ToString() + "  Iteration: " + iteration.ToString());
                }

            }
    }
}
