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


            public NeuralNet(int[] nodesInEachLayer) : base(nodesInEachLayer){}

            /* Zwraca indeks wzorca wyjściowego mającego wartość 1. 
             * Jeśli wartości Output.. i Best... są zgodne to rozwiązanie jest prawidłowe.
             */
            private int OutputPatternIndex(Pattern pattern)
            {
                for (int i = 0; i < pattern.OutputsCount; i++)
                    if (pattern.Output[i] == 1)
                        return i;
                return -1;
            }

            //Property, która zwraca indeks korzenia o maksymalnej wartości i mającego minimalny błąd.
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

            /* Trenowanie sieci (nadpis)*/
            /*public override void Train(PatternsCollection patterns)
            {

                int iteration = 0;
                if (patterns != null)
                {
                    double error = 0;
                    int good = 0;
                    while (good < patterns.Count) // Trenuj tak długo dopóki wszystkie wzorce nie będą poprawne
                    {
                        error = 0;
                        good = 0;

                        for (int i = 0; i < patterns.Count; i++)
                        {
                            //Ustawienia wartości sieci na wejściu
                            for (int k = 0; k < NodesInLayer(0); k++)
                                nodes[k].Value = patterns[i].Input[k];
                            //Uruchamianie sieci
                            this.Run();
                            //Ustawianie przeiwdywanych wyników
                            for (int k = 0; k < this.OutputNodesCount; k++)
                            {
                                error += Math.Abs(this.OutputNode(k).Error);
                                this.OutputNode(k).Error = patterns[i].Output[k];
                            }
                            //Uczenie sieci
                            this.Learn();
                            //Sprawdzanie czy sieć wypluł oczekiwany rezultat podczas tej iteracji
                            if (BestNodeIndex == OutputPatternIndex(patterns[i]))
                                good++;

                            iteration++;
                        }

                        //Regulacja wag połączeń w sieci do ich średniej wartości. (epoch)
                        foreach (NeuroLink link in links) ((EpochBackPropagationLink)link).Epoch(patterns.Count);

                        //Błąd średniokwadratowy
                        if ((iteration % 2) == 0)
                            Console.WriteLine("Błąd średni: " + (error / OutputNodesCount).ToString() + "  Iteracja: " + iteration.ToString());
                    }
                    Console.WriteLine("Błąd średni: " + (error / OutputNodesCount).ToString() + "  Iteracja: " + iteration.ToString());
                }

            }*/
    }
}
