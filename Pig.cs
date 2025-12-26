namespace AnimalLibrary
{
    [Comment("Класс свиньи - всеядное животное")]
    public class Pig : Animal
    {
        public Pig(string country, bool hideFromOtherAnimals, string name) 
            : base(country, hideFromOtherAnimals, name, "Pig")
        {
        }

        public Pig() : base()
        {
            WhatAnimal = "Pig";
        }

        public override eClassificationAnimal GetClassificationAnimal()
        {
            return eClassificationAnimal.Omnivores;
        }

        public override eFavoriteFood GetFavouriteFood()
        {
            return eFavoriteFood.Everything;
        }

        public override void SayHello()
        {
            Console.WriteLine($" {Name} хрюкает: Хрю-хрю!");
        }
    }
}
