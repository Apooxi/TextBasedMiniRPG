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
            Random random = new Random();

            Character Arthur = new Warrior("Arthur",100, 15, 70);
            Character Merlin = new Wizard("Merlin", 80, 20, 80);

            Console.WriteLine("=== ARENAYA HOŞGELDİNİZ ===");

            while(Arthur.Health > 0 && Merlin.Health > 0)
            {
                Console.WriteLine("\n--------------------------------");
                Console.WriteLine($"Sıra {Arthur.Name}'da! (Can: {Arthur.Health}, Mana: {Arthur.Mana})");
                Console.WriteLine("1 - Normal Saldırı");
                Console.WriteLine("2 - Özel Yetenek");
                Console.Write("Seçiminiz: ");
                string userChoice = Console.ReadLine();

                if(userChoice == "1")
                {
                    Arthur.Attack(Merlin);
                }
                else if(userChoice == "2")
                {
                    Arthur.SpecialSkill(Merlin);
                }
                else
                {
                    Console.WriteLine("Yanlış tuşa bastın, elin ayağına dolaştı ve hamle sıranı kaybettin!");
                }

                if (Merlin.Health <= 0) break;

                Console.WriteLine("\nDevam etmek için Enter'a bas...");
                Console.ReadLine();

                Console.WriteLine("--------------------------------");
                Console.WriteLine($"Sıra {Merlin.Name}'de! (Can: {Merlin.Health}, Mana: {Merlin.Mana})");

                int merlinChoice = random.Next(1, 3);

                if(merlinChoice == 1)
                {
                    Merlin.Attack(Arthur);
                }
                else
                {
                    Merlin.SpecialSkill(Arthur);
                }

                if (Arthur.Health <= 0) break;

                Console.WriteLine("\nYeni tura geçmek için Enter'a bas...");
                Console.ReadLine();
            }

            Console.WriteLine("\n=== SAVAŞ BİTTİ ===");
            if (Arthur.Health <= 0)
            {
                Console.WriteLine($"Kazanan {Merlin.Name} oldu!");
            }
            else
            {
                Console.WriteLine($"Kazanan {Arthur.Name} oldu!");
            }

            Console.ReadLine();
        }
    }

    abstract class Character
    {
        private string name;
        private int health;
        private int damage;
        private int mana;

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

        public int Mana
        {
            set
            {
                if(value < 0)
                {
                    Console.WriteLine("Negatif mana girilemez.");
                }
                else
                {
                    mana = value;
                }
            }
            get { return mana; }
        }

        public Character(string name, int health, int damage, int mana)
        {
            Name = name;
            Health = health;
            Damage = damage;
            Mana = mana;
        }

        public abstract void Attack(Character hedef); //hedef nesnesini parametre olarak alıyor
        public abstract void SpecialSkill(Character hedef);

        public void showInfo()
        {
            Console.WriteLine($"İsim: {Name}, Can: {Health}");
        }
    }

    class Warrior : Character
    {

        public Warrior(string name, int health, int damage,int mana) : base(name, health, damage,mana)
        {

        }
        public override void Attack(Character hedef)
        {
            hedef.Health -= Damage; //Hedefin canını warrior'ın damage'ı kadar düşürdük.
            Console.WriteLine($"Savaşçı {Name} {hedef.Name}'e kılıcı ile saldırdı ve {Damage} vurdu!");
            Console.WriteLine($"Büyücü {hedef.Name}'in Canı: {hedef.Health}");
        }

        public override void SpecialSkill(Character hedef)
        {
            if(Mana >=20) //Ağır darbe 20 mana olarak belirledik
            {
                hedef.Health -= 2 * Damage;
                Mana -= 20;
                Console.WriteLine($"Savaşçı {Name} {hedef.Name}'e kılıcı ile ağır saldırı gerçekleştirdi ve {2*Damage} vurdu!");
                Console.WriteLine($"Büyücü {hedef.Name}'in Canı: {hedef.Health}");
            }
            else
            {
                Console.WriteLine("Ağır saldırı için yetersiz mana!");
                Console.WriteLine("Normal saldırı yapılıyor...");
                Attack(hedef);
            }
        }
    }

    class Wizard : Character
    {
        public Wizard(string name, int health, int damage, int mana) : base(name, health, damage,mana)
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
                Console.WriteLine($"Savaşçı {hedef.Name}'in Canı: {hedef.Health}");
            }
            else
            {
                hedef.Health -= Damage;
                Console.WriteLine($"Büyücü {Name} ateş topu fırlattı.");
                Console.WriteLine($"Savaşçı {hedef.Name}'in Canı: {hedef.Health}");
            }
        }

        public override void SpecialSkill(Character hedef) //Burada aslında hedef parametresine gerek yok ama mimari gereği metot o parametreyi almak zorunda. Eğer mana yetersizse normal saldırı yapacak.
        {
            if(Mana >= 30)
            {
                Health += 30;
                Mana -= 30;
                Console.WriteLine($"Büyücü {Name} canını 30 artırdı!");
                Console.WriteLine($"Büyücü {Name} canını 30 artırdı! (Güncel Canı: {Health}, Kalan Mana: {Mana})");
            }
            else
            {
                Attack(hedef);
            }

        }
    }
}



