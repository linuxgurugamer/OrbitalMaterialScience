﻿/*
 *   This file is part of Orbital Material Science.
 *   
 *   Orbital Material Science is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   Orbital Material Sciencee is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with Orbital Material Science.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NE_Science
{
    /*
     *Module used to add Lab Equipment to the Tech tree. 
     */
    public class LabEquipmentModule : PartModule
    {

        [KSPField(isPersistant = false)]
        public string abbreviation = "";

        [KSPField(isPersistant = false)]
        public string eqName = "";

        [KSPField(isPersistant = true)]
        public float productPerHour = 0;
        [KSPField(isPersistant = false)]
        public string product = "";

        [KSPField(isPersistant = true)]
        public float reactantPerProduct = 0;
        [KSPField(isPersistant = false)]
        public string reactant = "";

    }

    /*
     * Class used to add LabEquipment to Containers
     */
    public class LabEquipment
    {
        public const string CONFIG_NODE_NAME = "NE_LabEquipment";
        private const string ABB_VALUE = "abb";
        private const string NAME_VALUE = "name";
        private const string TYPE_VALUE = "type";
        private const string MASS_VALUE = "mass";
        private const string PRODUCT_VALUE = "product";
        private const string PRODUCT_PER_HOUR_VALUE = "productPerHour";
        private const string REACTANT_VALUE = "reactant";
        private const string REACTANT_PER_PRODUCT_VALUE = "reactantPerProduct";

        private string abb;
        private string name;
        private float mass;
        private EquipmentRacks type;

        private float productPerHour = 0;
        private string product = "";

        private float reactantPerProduct = 0;
        private string reactant = "";

        private Generator gen;

        public LabEquipment(string abb, string name, EquipmentRacks type, float mass, float productPerHour, string product, float reactantPerProduct, string reactant)
        {
            this.abb = abb;
            this.name = name;
            this.type = type;
            this.mass = mass;

            this.product = product;
            this.productPerHour = productPerHour;

            this.reactant = reactant;
            this.reactantPerProduct = reactantPerProduct;
        }

        public string getAbbreviation()
        {
            return abb;
        }

        public string getName()
        {
            return name;
        }

        public EquipmentRacks getType()
        {
            return type;
        }

        public float getMass()
        {
            return mass;
        }

        static public LabEquipment getNullObject()
        {
             return new LabEquipment("empty", "empty", EquipmentRacks.NONE, 0f, 0f, "", 0f, "");
        }

        public ConfigNode getNode()
        {
            ConfigNode node = new ConfigNode(CONFIG_NODE_NAME);

            node.AddValue(ABB_VALUE, abb);
            node.AddValue(NAME_VALUE, name);
            node.AddValue(MASS_VALUE, mass);
            node.AddValue(TYPE_VALUE, type.ToString());

            node.AddValue(PRODUCT_VALUE, product);
            node.AddValue(PRODUCT_PER_HOUR_VALUE, productPerHour);

            node.AddValue(REACTANT_VALUE, reactant);
            node.AddValue(REACTANT_PER_PRODUCT_VALUE, reactantPerProduct);

            return node;
        }

        public static LabEquipment getLabEquipmentFromNode(ConfigNode node)
        {
            if (node.name != CONFIG_NODE_NAME)
            {
                NE_Helper.logError("getLabEquipmentFromNode: invalid Node: " + node.name);
                return getNullObject();
            }

            string abb = node.GetValue(ABB_VALUE);
            string name = node.GetValue(NAME_VALUE);
            float mass = float.Parse(node.GetValue(MASS_VALUE));

            string product = node.GetValue(PRODUCT_VALUE);
            float productPerHour = float.Parse(node.GetValue(PRODUCT_PER_HOUR_VALUE));

            string reactant = node.GetValue(REACTANT_VALUE);
            float reactantPerProduct = float.Parse(node.GetValue(REACTANT_PER_PRODUCT_VALUE));

            EquipmentRacks type = EquipmentRacksFactory.getType(node.GetValue(TYPE_VALUE));

            return new LabEquipment(abb, name, type, mass, productPerHour, product, reactantPerProduct, reactant);
        }


        public bool isRunning()
        {
            if (gen != null)
            {
                double last = gen.rates[product].last_produced;
                bool state = (last < -0.0000001);
                return state;
            }
            return false;
        }

        public void install(Lab lab)
        {
            gen = createGenerator(product, productPerHour, reactant, reactantPerProduct, lab);
            lab.addGenerator(gen);
        }

        private Generator createGenerator(string resToCreate, float creationRate, string useRes, float usePerUnit, Lab lab)
        {
            Generator gen = new Generator(lab.part);
            gen.addRate(resToCreate, -creationRate);
            if (usePerUnit > 0)
                gen.addRate(useRes, usePerUnit);
            return gen;
        }
    }
}