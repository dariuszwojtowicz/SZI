using System;
using System.Collections.Generic;
using System.Text;

namespace Kelner.Algorithm
{
    /// <summary>
    /// Classe que representará a arvore de decisăo montada;
    /// </summary>
    public class TreeNode
    {
        public TreeNodeCollection _children;
        private TreeAttribute _attribute;
        private Kelner.Algorithm.OutcomeTreeAttribute outcomeTreeAttribute;

        /// <summary>
        /// Inicializa uma nova instância de TreeNode
        /// </summary>
        /// <param name="attribute">Atributo ao qual o node está ligado</param>
        public TreeNode(TreeAttribute attribute)
        {
            if (attribute != null && attribute.PossibleValues != null)
            {
                _children = new TreeNodeCollection();
                for (int i = 0; i < attribute.PossibleValues.Count; i++)
                    _children.Add(null);
            }
            else
            {
                _children = new TreeNodeCollection();
                _children.Add(null);
            }
            _attribute = attribute;
        }

        public TreeNode(Kelner.Algorithm.OutcomeTreeAttribute outcomeTreeAttribute)
        {
            // TODO: Complete member initialization
            this.outcomeTreeAttribute = outcomeTreeAttribute;
        }

        /// <summary>
        /// Adiciona um TreeNode filho a este treenode no galho de nome indicicado pelo ValueName
        /// </summary>
        /// <param name="treeNode">TreeNode filho a ser adicionado</param>
        /// <param name="ValueName">Nome do galho onde o treeNode é criado</param>
        public void AddTreeNode(TreeNode treeNode, string ValueName)
        {
            int index = _attribute.indexValue(ValueName);
            _children[index] = treeNode;
        }

        /// <summary>
        /// Retorna o nro total de filhos do nó
        /// </summary>
        public int ChildrenCount
        {
            get
            {
                return _children.Count;
            }
        }

        /// <summary>
        /// Retorna o nó filho de um nó
        /// </summary>
        /// <param name="index">Indice do nó filho</param>
        /// <returns>Um objeto da classe TreeNode representando o nó</returns>
        public TreeNode GetChildAt(int index)
        {
            return _children[index];
        }

        /// <summary>
        /// Atributo que está conectado ao Nó
        /// </summary>
        public TreeAttribute Attribute
        {
            get
            {
                return _attribute;
            }
        }

        /// <summary>
        /// Retorna o filho de um nó pelo nome do galho que leva até ele
        /// </summary>
        /// <param name="branchName">Nome do galho</param>
        /// <returns>O nó</returns>
        public TreeNode GetChildByBranchName(string branchName)
        {
            int index = _attribute.indexValue(branchName);
            return _children[index];
        }
    }
}
