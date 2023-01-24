using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSETWebCore.Model.Diagram
{
    public class ComponentSoftwareItem
    {
        private string vendorNameField { get; set; }
        private string nameField { get; set; }
        private string versionField { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string VendorName 
        {
            get 
            {
                return vendorNameField;
            }
            set 
            {
                vendorNameField = value;
            }
        }


        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return nameField;
            }
            set
            {
                nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Version
        {
            get
            {
                return versionField;
            }
            set
            {
                versionField = value;
            }
        }
    }
}
