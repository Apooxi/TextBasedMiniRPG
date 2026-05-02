using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace TextBasedMiniRPG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();

            List<Character> roster = new List<Character>();

            string connString = "Host=localhost;Username=postgres;Password=admin;Database=ArenaDB";

            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string sqlQuery = "SELECT Id, CharacterType, Name, Health, Damage, Mana FROM Characters";
                using (NpgsqlCommand cmd = new NpgsqlCommand(sqlQuery, conn))
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("=== ARENA DÖVÜŞÇÜ LİSTESİ ===");
                    int index = 1;

                    while (reader.Read())
                    {
                        string type = reader["CharacterType"].ToString();
                        string name = reader["Name"].ToString();
                        int hp = Convert.ToInt32(reader["Health"]);
                        int dmg = Convert.ToInt32(reader["Damage"]);
                        int mana = Convert.ToInt32(reader["Mana"]);

                        Character newChar;
                        if (type == "Warrior")
                        {
                            newChar = new Warrior(name, hp, dmg, mana);
                        }
                        else
                        {
                            newChar = new Wizard(name, hp, dmg, mana);
                        }

                        roster.Add(newChar);

                        Console.WriteLine($"{index}. {name} ({type}) -> Can: {hp}, Hasar: {dmg}, Mana: {mana}");
                        index++;
                    }
                }

            }

            Console.WriteLine("=============================");
            Console.Write("Kendi karakterini seç (1 - 5): ");
            int player1Choice = Convert.ToInt32(Console.ReadLine()) - 1;
            Character Player1 = roster[player1Choice];

            Console.Write("Rakibini seç (1 - 5): ");
            int player2Choice = Convert.ToInt32(Console.ReadLine()) - 1;
            Character Player2 = roster[player2Choice];

            Armor celikzirh = new Armor("Çelik Zırh", 5);
            Armor gucluCelikzirh = new Armor("Güçlü Çelik Zırh", 15);
            Player1.EquippedArmor = gucluCelikzirh;
            Player2.EquippedArmor = celikzirh;

            Console.WriteLine($"\nSAVAŞ BAŞLIYOR: {Player1.Name} vs {Player2.Name}!");
            Console.ReadLine();

            Console.WriteLine("=== ARENAYA HOŞGELDİNİZ ===");

            while(Player1.Health > 0 && Player2.Health > 0)
            {
                Console.WriteLine("\n--------------------------------");
                Console.WriteLine($"Sıra {Player1.Name}'da! (Can: {Player1.Health}, Mana: {Player1.Mana})");
                Console.WriteLine("1 - Normal Saldırı");
                Console.WriteLine("2 - Özel Yetenek");
                Console.Write("Seçiminiz: ");
                string userChoice = Console.ReadLine();

                if(userChoice == "1")
                {
                    Player1.Attack(Player2);
                }
                else if(userChoice == "2")
                {
                    Player1.SpecialSkill(Player2);
                }
                else
                {
                    Console.WriteLine("Yanlış tuşa bastın, elin ayağına dolaştı ve hamle sıranı kaybettin!");
                }

                if (Player2.Health <= 0) break;

                Console.WriteLine("\nDevam etmek için Enter'a bas...");
                Console.ReadLine();

                Console.WriteLine("--------------------------------");
                Console.WriteLine($"Sıra {Player2.Name}'de! (Can: {Player2.Health}, Mana: {Player2.Mana})");

                int Player2Choice = random.Next(1, 3);

                if(Player2Choice == 1)
                {
                    Player2.Attack(Player1);
                }
                else
                {
                    Player2.SpecialSkill(Player1);
                }

                if (Player1.Health <= 0) break;

                Console.WriteLine("\nYeni tura geçmek için Enter'a bas...");
                Console.ReadLine();
            }

            Console.WriteLine("\n=== SAVAŞ BİTTİ ===");
            if (Player1.Health <= 0)
            {
                Console.WriteLine($"Kazanan {Player2.Name} oldu!");
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string sqlQuery = $"UPDATE Characters SET Wins = Wins + 1 WHERE Name = '{Player2.Name}'";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(sqlQuery, conn))
                        cmd.ExecuteNonQuery();
                }
            }
            else
            {
                Console.WriteLine($"Kazanan {Player1.Name} oldu!");
                using (NpgsqlConnection conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    string sqlQuery = $"UPDATE Characters SET Wins = Wins + 1 WHERE Name = '{Player1.Name}'";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(sqlQuery, conn))
                        cmd.ExecuteNonQuery();
                }
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
        public Armor EquippedArmor { get; set; } //Kapsülleme kısayolu artı olarak bir nesnenin başka bir nesneye sahip olması (Has-A) ilişkisi

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

        public virtual void TakeDamage(int incomingDamage)
        {
            int netDamage = incomingDamage;
            if(EquippedArmor != null)
            {
                netDamage -= EquippedArmor.Defense;
                Console.WriteLine($"[Sistem] {EquippedArmor.Name} zırhı hasarın {EquippedArmor.Defense} kadarını emdi!");
            }

            if(netDamage < 0)
            {
                netDamage = 0;
            }

            Health -= netDamage;

            Console.WriteLine($"[Sistem] {Name} {netDamage} net hasar aldı! (Kalan Can: {Health})");
        }

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
            Console.WriteLine($"{Name} kılıcı ile saldırdı!");
            hedef.TakeDamage(Damage);
        }

        public override void SpecialSkill(Character hedef)
        {
            if(Mana >=20) //Ağır darbe 20 mana olarak belirledik
            {
                hedef.TakeDamage(2 * Damage);
                Mana -= 20;
                Console.WriteLine($"{Name} {hedef.Name}'e kılıcı ile ağır saldırı gerçekleştirdi ve {2*Damage} vurdu!");
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
                hedef.TakeDamage(2 * Damage);
                Console.WriteLine($"{Name} kritik ateş topu fırlattı.");
            }
            else
            {
                hedef.TakeDamage(Damage);
                Console.WriteLine($"{Name} ateş topu fırlattı.");
            }
        }

        public override void SpecialSkill(Character hedef) //Burada aslında hedef parametresine gerek yok ama mimari gereği metot o parametreyi almak zorunda. Eğer mana yetersizse normal saldırı yapacak.
        {
            if(Mana >= 30)
            {
                Health += 30;
                Mana -= 30;
                Console.WriteLine($"{Name} canını 30 artırdı! (Güncel Canı: {Health}, Kalan Mana: {Mana})");
            }
            else
            {
                Attack(hedef);
            }

        }
    }

    class Armor
    {
        private string name;
        private int defense;
        public string Name
        {
            set
            {
                if(!string.IsNullOrEmpty(value))
                {
                    name = value;
                }
                else
                {
                    Console.WriteLine("İsim boş geçilemez.");
                }
            }
            get { return name; }
        }

        public int Defense
        {
            set
            {
                if(value < 0)
                {
                    Console.WriteLine("Defans gücü negatif olamaz.");
                }
                else
                {
                    defense = value;
                }
            }
            get { return defense; }
        }

        public Armor(string name, int defense)
        {
            Name = name;
            Defense = defense;
        }
    }
}



