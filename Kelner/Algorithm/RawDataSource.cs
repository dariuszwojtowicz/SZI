using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Kelner.Algorithm
{
    public class RawDataSource : DataTable
    {
        string _sourcePathAndFile;

        public RawDataSource(string sourcePathAndFile)
        {
            _sourcePathAndFile = sourcePathAndFile;
            load();
        }

        public RawDataSource()
        {
            _sourcePathAndFile = String.Empty;
        }

        private void load()
        {
            this.TableName = "samples";

            string[] lines = File.ReadAllLines(_sourcePathAndFile);
            string[] columns;

            for (int k = 0; k < lines.Length; k++)
            {
                string line = lines[k];

                if (k == 0)
                {
                    columns = line.Split(',');

                    for (int j = 0; j < columns.Length; j++)
                    {
                        if (!this.Columns.Contains(columns[j]))
                            this.Columns.Add(columns[j].Trim());
                    }
                    continue;
                }

                DataRow row = this.NewRow();

                columns = line.Split(',');

                for (int i = 0; i < columns.Length; i++)
                {
                    string val = columns[i].Trim();
                    val = val.Replace("\"", String.Empty);

                    if (val.ToUpper() == "TRUE" || val.ToUpper() == "FALSE")
                    {
                        bool booleanValue = bool.Parse(val);
                        row[i] = booleanValue;
                        continue;
                    }
                    row[i] = val;
                }
                this.Rows.Add(row);
            }
        }

        public PossibleValueCollection GetValuesFromColumn(string columnName)
        {
            PossibleValueCollection returnList = new PossibleValueCollection();
            foreach (DataRow row in this.Rows)
            {
                foreach (DataColumn column in this.Columns)
                {
                    if (column.ColumnName.ToUpper() == columnName.ToUpper().Trim())
                    {
                        if (!returnList.Contains(row[column].ToString()))
                            returnList.Add(row[column].ToString());
                    }
                }
            }
            return returnList;
        }

        public TreeAttributeCollection GetValidAttributeCollection()
        {
            TreeAttributeCollection returnCollection = new TreeAttributeCollection();

            foreach (DataColumn column in this.Columns)
            {
                TreeAttribute currentAttribute = new TreeAttribute(column.ColumnName, GetValuesFromColumn(column.ColumnName));

                if (returnCollection.ContainsAttribute(currentAttribute) || currentAttribute.AttributeName.ToUpper().Trim() == "RESULT")
                    continue;
                returnCollection.Add(currentAttribute);
            }
            return returnCollection;

        }
    }
}
