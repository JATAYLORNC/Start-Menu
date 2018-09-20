using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Web;

namespace Start.Models
{
    [Table(Name = "Program")]
    public class Program
    {
        private int _ID;
        [Column(IsPrimaryKey = true, Storage = "_ID")]
        public int ID {
            get
            {
                return this.ID;
            }
            set
            {
                this.ID = value;
            }
        }
        private String _Title;
        [Column(Storage = "_Title")]
        public String Title {
            get
            {
                return this.Title;
            }
            set
            {
                this.Title = value;
            }
        }

        private String _Path;
        [Column(Storage = "_Path")]
        public String Path {
            get
            {
                return this.Path;
            }
            set
            {
                this.Path = value;
            }
        }

        private String _IconPath;
        [Column(Storage = "_IconPath")]        
        public String IconPath {
            get
            {
                return this.IconPath;
            }
            set
            {
                this.IconPath = value;
            } 
        }

        private int _Count;
        [Column(Storage = "_Count")]
        public int Count {
            get
            {
                return this.Count;
            }
            set
            {
                this.Count = value;
            } 
        }
    }
}