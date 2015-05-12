namespace Kelner.Algorithm
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.IO;
    using System.Diagnostics;

    /// <summary>
    /// Classe que implementa uma árvore de Decisăo usando o algoritmo ID3
    /// </summary>
    public class DecisionTree
    {
        private DataTable _sampleData;
        private int mTotalPositives = 0;
        private int mTotal = 0;
        private string mTargetAttribute = "result";
        private double mEntropySet = 0.0;

        /// <summary>
        /// The total positive count in the source data
        /// </summary>
        /// <param name="samples">DataTable com as amostras</param>
        /// <returns>O nro total de amostras positivas</returns>
        private int countTotalPositives(DataTable samples)
        {
            int result = 0;

            foreach (DataRow aRow in samples.Rows)
            {
                if (aRow[mTargetAttribute].ToString().ToUpper().Trim() == "TRUE")
                    result++;
            }

            return result;
        }

        /// <summary>
        /// Calcula a entropia dada a seguinte fórmula
        /// -p+log2p+ - p-log2p-
        /// 
        /// onde: p+ é a proporçăo de valores positivos
        ///		  p- é a proporçăo de valores negativos
        /// </summary>
        /// <param name="positives">Quantidade de valores positivos</param>
        /// <param name="negatives">Quantidade de valores negativos</param>
        /// <returns>Retorna o valor da Entropia</returns>
        private double getCalculatedEntropy(int positives, int negatives)
        {
            int total = positives + negatives;
            double ratioPositive = (double)positives / total;
            double ratioNegative = (double)negatives / total;

            if (ratioPositive != 0)
                ratioPositive = -(ratioPositive) * System.Math.Log(ratioPositive, 2);
            if (ratioNegative != 0)
                ratioNegative = -(ratioNegative) * System.Math.Log(ratioNegative, 2);

            double result = ratioPositive + ratioNegative;

            return result;
        }

        /// <summary>
        /// Varre tabela de amostras verificando um atributo e se o resultado é positivo ou negativo
        /// </summary>
        /// <param name="samples">DataTable com as amostras</param>
        /// <param name="attribute">Atributo a ser pesquisado</param>
        /// <param name="value">valor permitido para o atributo</param>
        /// <param name="positives">Conterá o nro de todos os atributos com o valor determinado com resultado positivo</param>
        /// <param name="negatives">Conterá o nro de todos os atributos com o valor determinado com resultado negativo</param>
        private void getValuesToAttribute(DataTable samples, TreeAttribute attribute, string value, out int positives, out int negatives)
        {
            positives = 0;
            negatives = 0;

            foreach (DataRow aRow in samples.Rows)
            {
                ///To do:   Figure out if this is correct - it looks bad
                if (((string)aRow[attribute.AttributeName] == value))
                    if (aRow[mTargetAttribute].ToString().Trim().ToUpper() == "TRUE")
                        positives++;
                    else
                        negatives++;

            }
        }

        /// <summary>
        /// Calcula o ganho de um atributo
        /// </summary>
        /// <param name="attribute">Atributo a ser calculado</param>
        /// <returns>O ganho do atributo</returns>
        private double gain(DataTable samples, TreeAttribute attribute)
        {
            PossibleValueCollection values = attribute.PossibleValues;
            double sum = 0.0;

            for (int i = 0; i < values.Count; i++)
            {
                int positives, negatives;

                positives = negatives = 0;

                getValuesToAttribute(samples, attribute, values[i], out positives, out negatives);

                double entropy = getCalculatedEntropy(positives, negatives);
                sum += -(double)(positives + negatives) / mTotal * entropy;
            }
            return mEntropySet + sum;
        }

        /// <summary>
        /// Retorna o melhor atributo.
        /// </summary>
        /// <param name="attributes">Um vetor com os atributos</param>
        /// <returns>Retorna o que tiver maior ganho</returns>
        private TreeAttribute getBestAttribute(DataTable samples, TreeAttributeCollection attributes)
        {
            double maxGain = 0.0;
            TreeAttribute result = null;

            foreach (TreeAttribute attribute in attributes)
            {
                double aux = gain(samples, attribute);
                if (aux > maxGain)
                {
                    maxGain = aux;
                    result = attribute;
                }
            }
            return result;
        }

        /// <summary>
        /// Retorna true caso todos os exemplos da amostragem săo positivos
        /// </summary>
        /// <param name="samples">DataTable com as amostras</param>
        /// <param name="targetAttribute">Atributo (coluna) da tabela a qual será verificado</param>
        /// <returns>True caso todos os exemplos da amostragem săo positivos</returns>
        private bool allSamplesArePositive(DataTable samples, string targetAttribute)
        {
            foreach (DataRow row in samples.Rows)
            {
                if (row[targetAttribute].ToString().ToUpper().Trim() == "FALSE")
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Retorna true caso todos os exemplos da amostragem săo negativos
        /// </summary>
        /// <param name="samples">DataTable com as amostras</param>
        /// <param name="targetAttribute">Atributo (coluna) da tabela a qual será verificado</param>
        /// <returns>True caso todos os exemplos da amostragem săo negativos</returns>
        private bool allSamplesAreNegative(DataTable samples, string targetAttribute)
        {
            foreach (DataRow row in samples.Rows)
            {
                if (row[targetAttribute].ToString().ToUpper().Trim() == "TRUE")
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Retorna uma lista com todos os valores distintos de uma tabela de amostragem
        /// </summary>
        /// <param name="samples">DataTable com as amostras</param>
        /// <param name="targetAttribute">Atributo (coluna) da tabela a qual será verificado</param>
        /// <returns>Um ArrayList com os valores distintos</returns>
        private ArrayList getDistinctValues(DataTable samples, string targetAttribute)
        {
            ArrayList distinctValues = new ArrayList(samples.Rows.Count);

            foreach (DataRow row in samples.Rows)
            {
                if (distinctValues.IndexOf(row[targetAttribute]) == -1)
                    distinctValues.Add(row[targetAttribute]);
            }

            return distinctValues;
        }

        /// <summary>
        /// Retorna o valor mais comum dentro de uma amostragem
        /// </summary>
        /// <param name="samples">DataTable com as amostras</param>
        /// <param name="targetAttribute">Atributo (coluna) da tabela a qual será verificado</param>
        /// <returns>Retorna o objeto com maior incidęncia dentro da tabela de amostras</returns>
        private object getMostCommonValue(DataTable samples, string targetAttribute)
        {
            ArrayList distinctValues = getDistinctValues(samples, targetAttribute);
            int[] count = new int[distinctValues.Count];

            foreach (DataRow row in samples.Rows)
            {
                int index = distinctValues.IndexOf(row[targetAttribute]);
                count[index]++;
            }

            int MaxIndex = 0;
            int MaxCount = 0;

            for (int i = 0; i < count.Length; i++)
            {
                if (count[i] > MaxCount)
                {
                    MaxCount = count[i];
                    MaxIndex = i;
                }
            }

            return distinctValues[MaxIndex];
        }

        /// <summary>
        /// Monta uma árvore de decisăo baseado nas amostragens apresentadas
        /// </summary>
        /// <param name="samples">Tabela com as amostragens que serăo apresentadas para a montagem da árvore</param>
        /// <param name="targetAttribute">Nome da coluna da tabela que possue o valor true ou false para 
        /// validar ou năo uma amostragem</param>
        /// <returns>A raiz da árvore de decisăo montada</returns></returns?>
        private TreeNode internalMountTree(DataTable samples, string targetAttribute, TreeAttributeCollection attributes)
        {
            if (allSamplesArePositive(samples, targetAttribute) == true)
                return new TreeNode(new OutcomeTreeAttribute(true));

            if (allSamplesAreNegative(samples, targetAttribute) == true)
                return new TreeNode(new OutcomeTreeAttribute(false));

            if (attributes.Count == 0)
                return new TreeNode(new OutcomeTreeAttribute(getMostCommonValue(samples, targetAttribute)));

            mTotal = samples.Rows.Count;
            mTargetAttribute = targetAttribute;
            mTotalPositives = countTotalPositives(samples);

            mEntropySet = getCalculatedEntropy(mTotalPositives, mTotal - mTotalPositives);

            TreeAttribute bestAttribute = getBestAttribute(samples, attributes);

            TreeNode root = new TreeNode(bestAttribute);

            if (bestAttribute == null)
                return root;

            DataTable aSample = samples.Clone();

            foreach (string value in bestAttribute.PossibleValues)
            {
                // Seleciona todas os elementos com o valor deste atributo				
                aSample.Rows.Clear();

                DataRow[] rows = samples.Select(bestAttribute.AttributeName + " = " + "'" + value + "'");

                foreach (DataRow row in rows)
                {
                    aSample.Rows.Add(row.ItemArray);
                }
                // Seleciona todas os elementos com o valor deste atributo				

                // Cria uma nova lista de atributos menos o atributo corrente que é o melhor atributo		
                TreeAttributeCollection aAttributes = new TreeAttributeCollection();
                //ArrayList aAttributes = new ArrayList(attributes.Count - 1);
                for (int i = 0; i < attributes.Count; i++)
                {
                    if (attributes[i].AttributeName != bestAttribute.AttributeName)
                        aAttributes.Add(attributes[i]);
                }
                // Cria uma nova lista de atributos menos o atributo corrente que é o melhor atributo

                if (aSample.Rows.Count == 0)
                {
                    return new TreeNode(new OutcomeTreeAttribute(getMostCommonValue(aSample, targetAttribute)));
                }
                else
                {
                    DecisionTree dc3 = new DecisionTree();
                    TreeNode ChildNode = dc3.mountTree(aSample, targetAttribute, aAttributes);
                    root.AddTreeNode(ChildNode, value);
                }
            }

            return root;
        }


        /// <summary>
        /// Monta uma árvore de decisăo baseado nas amostragens apresentadas
        /// </summary>
        /// <param name="samples">Tabela com as amostragens que serăo apresentadas para a montagem da árvore</param>
        /// <param name="targetAttribute">Nome da coluna da tabela que possue o valor true ou false para 
        /// validar ou năo uma amostragem</param>
        /// <returns>A raiz da árvore de decisăo montada</returns></returns?>
        public TreeNode mountTree(DataTable samples, string targetAttribute, TreeAttributeCollection attributes)
        {
            _sampleData = samples;
            return internalMountTree(_sampleData, targetAttribute, attributes);
        }
    }

    /// <summary>
    /// Classe que exemplifica a utilizaçăo do ID3
    /// </summary>
    public class DecisionTreeImplementation
    {

        string _sourceFile;

        public TreeNode GetTree(string sourceFile)
        {
            _sourceFile = sourceFile;
            RawDataSource samples = new RawDataSource(_sourceFile);

            TreeAttributeCollection attributes = samples.GetValidAttributeCollection();

            DecisionTree id3 = new DecisionTree();
            TreeNode root = id3.mountTree(samples, "result", attributes);

            Debug.WriteLine(this.PrintNode(root, ""));
            return root;
           // return PrintNode(root, "");

        }
        public string PrintNode(TreeNode root, string tabs)
        {
            string returnString = String.Empty;
            string prefix = "Best Attribute: ";

            if (tabs != String.Empty)
                prefix = " -> Likely Outcome: ";
            returnString += (tabs + prefix + root.Attribute) + Environment.NewLine;

            if (root != null && root.Attribute != null && root.Attribute.PossibleValues != null)
            {
                for (int i = 0; i < root.Attribute.PossibleValues.Count; i++)
                {
                    returnString += (Environment.NewLine + tabs + "\t" + "Input:  " + root.Attribute.PossibleValues[i]) + Environment.NewLine;
                    TreeNode childNode = root.GetChildByBranchName(root.Attribute.PossibleValues[i]);
                    returnString += PrintNode(childNode, "\t" + tabs);
                }
            }
            // Debug.Write(returnString);
            return returnString;
        }

    }
}
