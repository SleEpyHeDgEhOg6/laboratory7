using System;

namespace AnimalLibrary
{
    [Comment("Абстрактный базовый класс для всех животных")]
    public abstract class Animal
    {
        public string Country { get; set; }
        public bool HideFromOtherAnimals { get; set; }
        public string Name { get; set; }
        public string WhatAnimal { get; protected set; }

        protected Animal(string country, bool hideFromOtherAnimals, string name, string whatAnimal)
        {
            Country = country;
            HideFromOtherAnimals = hideFromOtherAnimals;
            Name = name;
            WhatAnimal = whatAnimal;
        }

        protected Animal() { }

        public void Deconstruct(out string country, out bool hideFromOtherAnimals, out string name, out string whatAnimal)
        {
            country = Country;
            hideFromOtherAnimals = HideFromOtherAnimals;
            name = Name;
            whatAnimal = WhatAnimal;
        }

        public abstract eClassificationAnimal GetClassificationAnimal();
        
        public abstract eFavoriteFood GetFavouriteFood();
        
        public abstract void SayHello();
    }
}
