﻿using DatabaseAnalizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAnalizer.Models
{
    public class Table
    {
        public bool IsMainTable { set; get; }
        public string Name { set; get; }        
        public List<Column> Columns { set; get; }
        public List<TableRelation> RelationsIn {set; get;}
        public List<TableRelation> RelationsFrom { set; get; }

        public Table()
        {
            RelationsIn = new List<TableRelation>();
            RelationsFrom = new List<TableRelation>();
            Columns = new List<Column>();
        }
        public Table(string name)
        {
            this.Name = name;
            RelationsIn = new List<TableRelation>();
            RelationsFrom = new List<TableRelation>();
        }  
    }
}
