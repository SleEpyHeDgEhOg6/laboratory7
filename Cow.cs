namespace AnimalLibrary
{
    [Comment("Класс коровы - травоядное животное")]
    public class Cow : Animal
    {
        public Cow(string country, bool hideFromOtherAnimals, string name) 
            : base(country, hideFromOtherAnimals, name, "Cow")
        {
        }

        public Cow() : base()
        {
            WhatAnimal = "Cow";
        }

        public override eClassificationAnimal GetClassificationAnimal()
        {
            return eClassificationAnimal.Herbivores;
        }

        public override eFavoriteFood GetFavouriteFood()
        {
            return eFavoriteFood.Plants;
        }

        public override void SayHello()
        {
            Console.WriteLine($"{Name} говорит: Муууу!");
        }
    }
}
