using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextBasedMiniRPG
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Character Arthur = new Warrior("Arthur", 100, 15);
            Character Merlin = new Wizard("Merlin", 80, 20);

            while(Arthur.Health > 0 && Merlin.Health >0)
            {
                Arthur.Attack(Merlin);
                Arthur.showInfo();
                Console.WriteLine();
                Merlin.showInfo();
                Console.ReadLine();

                Merlin.Attack(Arthur);
                Merlin.showInfo();
                Console.WriteLine();
                Arthur.showInfo();
                Console.ReadLine();

            }

            if(Arthur.Health == 0)
            {
                Console.WriteLine($"Kazanan {Merlin.Name}");
            }
            else
            {
                Console.WriteLine($"Kazanan {Arthur.Name}");
            }

            Console.ReadLine();
        }
    }

    abstract class Character
    {
        private string name;
        private int health;
        private int damage;

        public string Name
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    name = value;
                }
                else
                {
                    Console.WriteLine("Boş değer girilemez.");
                }
            }
            get { return name; }
        }

        public int Health
        {
            set
            {
                if (value < 0)
                {
                    health = 0;
                }
                else
                {
                    health = value;
                }
            }
            get
            {
                return health;
            }
        }

        public int Damage
        {
            set
            {
                if (value < 0)
                {
                    Console.WriteLine("Hasar sıfırın altında olamaz.");
                }
                else
                {
                    damage = value;
                }
            }
            get
            {
                return damage;
            }
        }

        public Character(string name, int health, int damage)
        {
            Name = name;
            Health = health;
            Damage = damage;
        }

        public abstract void Attack(Character hedef); //hedef nesnesini parametre olarak alıyor

        public void showInfo()
        {
            Console.WriteLine($"İsim: {Name}, Can: {Health}");
        }
    }

    class Warrior : Character
    {

        public Warrior(string name, int health, int damage) : base(name, health, damage)
        {

        }
        public override void Attack(Character hedef)
        {
            hedef.Health -= Damage; //Hedefin canını warrior'ın damage'ı kadar düşürdük.
            Console.WriteLine($"Savaşçı {Name} {hedef.Name}'e kılıcı ile saldırdı ve {Damage} vurdu!");
        }
    }

    class Wizard : Character
    {
        public Wizard(string name, int health, int damage) : base(name, health, damage)
        {

        }

        Random random = new Random();
        
        public override void Attack(Character hedef)
        {
            int number = random.Next(1, 4);

            if (number == 3)
            {
                hedef.Health -= 2 * Damage;
                Console.WriteLine($"Büyücü {Name} kritik ateş topu fırlattı.");
            }
            else
            {
                hedef.Health -= Damage;
                Console.WriteLine($"Büyücü {Name} ateş topu fırlattı.");
            }
        }
    }
}



