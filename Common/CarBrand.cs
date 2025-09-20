using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
	public class CarBrand
	{
		private string name;
		private string country;
		public List<Model> models;

        public string Name { get; set; }
        public string Country { get; set; }
        public List<Model> Models { get; set; }

        // Prazan konstruktor za serijalizaciju
        public CarBrand()
        {
            Models = new List<Model>();
        }

        public CarBrand(string name, string country)
        {
            Name = name;
            Country = country;
            Models = new List<Model>();
        }

    }
}
