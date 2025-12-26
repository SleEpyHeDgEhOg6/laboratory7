namespace AnimalLibrary
{
    [Comment("Класс льва - хищное животное")]
    public class Lion : Animal
    {
        public Lion(string country, bool hideFromOtherAnimals, string name) 
            : base(country, hideFromOtherAnimals, name, "Lion")
        {
        }

        public Lion() : base()
        {
            WhatAnimal = "Lion";
        }

        public override eClassificationAnimal GetClassificationAnimal()
        {
            return eClassificationAnimal.Carnivores;
        }

        public override eFavoriteFood GetFavouriteFood()
        {
            return eFavoriteFood.Meat;
        }

        public override void SayHello()
        {
            Console.WriteLine($" {Name} рычит: Ррррр!");
        }
    }
}
