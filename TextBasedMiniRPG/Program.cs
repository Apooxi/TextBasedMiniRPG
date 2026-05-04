using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Npgsql;

namespace TextBasedMiniRPG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DatabaseManager dbManager = new DatabaseManager();


            Console.WriteLine("Ne yapmak istersin?");
            Console.WriteLine("1 - Arenaya Git");
            Console.WriteLine("2 - Yeni Karakter Yarat");
            string userChoice = Console.ReadLine();

            if(userChoice == "2")
            {
                
                Console.WriteLine("Karakterin sınıfı ne olsun?");
                Console.WriteLine("Warrior, Wizard");
                string characterType = checkStringInput(Console.ReadLine());
                Console.Write("Karakterin adı: ");
                string characterName = checkStringInput(Console.ReadLine());
                Console.Write("Karakterin can değerini gir: ");
                int characterHealth = checkIntInput(Console.ReadLine());
                Console.Write("Karakterin hasar değeri kaç olsun: ");
                int characterDamage = checkIntInput(Console.ReadLine());
                Console.Write("Karakterin kaç manası olsun: ");
                int characterMana = checkIntInput(Console.ReadLine());

                dbManager.AddCharacter(characterType, characterName, characterHealth, characterDamage, characterMana);

            }

            Random random = new Random();

            List<Character> roster = dbManager.GetAllCharacters();

            Console.WriteLine("=== ARENA DÖVÜŞÇÜ LİSTESİ ===");
            int index = 1;
            foreach (var fighter in roster)
            {
                Console.WriteLine($"{index}. {fighter.Name} ({fighter.GetType().Name}) -> Can: {fighter.Health}, Hasar: {fighter.Damage}, Mana: {fighter.Mana}, Level: {fighter.Level}, XP: {fighter.Xp}, Gold: {fighter.Gold}");
                index++;
            }



            Console.WriteLine("=============================");
            Console.Write("Kendi karakterini seç (1 - 5): ");
            int player1Choice = Convert.ToInt32(Console.ReadLine()) - 1;
            Character Player1 = roster[player1Choice];

            Console.Write("Rakibini seç (1 - 5): ");
            int player2Choice = Convert.ToInt32(Console.ReadLine()) - 1;
            Character Player2 = roster[player2Choice];

            Shop cityShop = new Shop();

            HealthPotion smallPotion = new HealthPotion(20, 10, "Küçük İksir");
            HealthPotion bigPotion = new HealthPotion(50, 30, "Büyük İksir");

            cityShop.AddProduct(smallPotion);
            cityShop.AddProduct(bigPotion);

            Armor celikzirh = new Armor("Çelik Zırh", 5, 10);
            Armor gucluCelikzirh = new Armor("Güçlü Çelik Zırh", 15, 5);

            
            Player1.Inventory.Add(smallPotion);

            Player1.EquippedArmor = gucluCelikzirh;
            Player2.EquippedArmor = celikzirh;

            Console.WriteLine("Savaş Başlamadan mağazaya girmek ister misiniz? (E/H)");
            string shopChoice = Console.ReadLine().Trim().ToLower();
            if(shopChoice == "e")
            {
                Console.WriteLine($"Karakterin bakiyesi {Player1.Gold}");
                Console.WriteLine();
                Console.WriteLine("Mağazaya hoşgeldiniz ürünler aşağıdadır almak istediğiniz ürünü seçin: ");
                cityShop.ShowProducts();

                bool result;
                while (true)
                {
                    result = int.TryParse(Console.ReadLine(), out int itemChoice);

                    if (result)
                    {
                        cityShop.BuyItem(Player1, itemChoice - 1);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Hatalı giriş yaptınız! Tekrar girin: "); 
                    }
                }
            }

            Console.WriteLine($"\nSAVAŞ BAŞLIYOR: {Player1.Name} vs {Player2.Name}!");
            Console.ReadLine();

            Console.WriteLine("=== ARENAYA HOŞGELDİNİZ ===");

            while(Player1.Health > 0 && Player2.Health > 0)
            {
                Console.WriteLine("\n--------------------------------");
                Console.WriteLine($"Sıra {Player1.Name}'da! (Can: {Player1.Health}, Mana: {Player1.Mana})");
                Console.WriteLine("1 - Normal Saldırı");
                Console.WriteLine("2 - Özel Yetenek");
                Console.WriteLine("3 - Eşya Kullan");
                Console.Write("Seçiminiz: ");
                string playerChoice = Console.ReadLine();

                if(playerChoice == "1")
                {
                    Player1.Attack(Player2);
                }
                else if(playerChoice == "2")
                {
                    Player1.SpecialSkill(Player2);
                }
                else if (playerChoice == "3")
                {
                    if(Player1.Inventory.Count <= 0)
                    {
                        Console.WriteLine("Çantan boş!");
                    }
                    else
                    {
                        IUsable selectedItem = Player1.Inventory[0];
                        selectedItem.Use(Player1);
                        Player1.Inventory.Remove(selectedItem);
                    }
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
                Player2.GainXP(50); //Kazanana 50 xp verildiğini varsaydık
                Player2.GainGold(30); //Kazanana 30 altın verildiğini varsaydık
                dbManager.UpdateCharacterStats(Player2);
                dbManager.AddWinToCharacters(Player2.Name);
            }
            else
            {
                Console.WriteLine($"Kazanan {Player1.Name} oldu!");
                Player1.GainXP(50);
                Player1.GainGold(30);
                dbManager.UpdateCharacterStats(Player1);
                dbManager.AddWinToCharacters(Player1.Name);
            }

            Console.ReadLine();
        }

        public static string checkStringInput(string userInput)
        {
            string result;
            while(true)
            {
                if (!string.IsNullOrEmpty(userInput))
                {
                    result =  userInput;
                    break;
                }
                else
                {
                    Console.WriteLine("İsim boş olamaz!");
                    Console.WriteLine("Tekrar girin");
                    userInput = Console.ReadLine();
                }
            }
            return result;
            
        }

        public static int checkIntInput(string userInput)
        {
            int number;

            while (true)
            {
                
                bool result = int.TryParse(userInput, out number);

                if (result && !(number < 0))
                {
                    return number;
                }
                else 
                {
                    Console.WriteLine("Hatalı giriş! Lütfen geçerli ve sıfırdan büyük bir sayı girin.");
                    Console.WriteLine("Tekrar girin.");
                    userInput = Console.ReadLine();
                }
                
            }
        }
    }

    abstract class Character
    {
        private string name;
        private int health;
        private int damage;
        private int mana;
        private int level;
        private int xp;
        private int gold;
        private List<IUsable> inventory;

        public List<IUsable> Inventory { get; private set; } = new List<IUsable>();
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

        public int Xp
        {
            set
            {
                if(value < 0)
                {
                    Console.WriteLine("XP sıfırın altında olamaz.");
                }
                else
                {
                    xp = value;
                }
            }
            get { return xp; }
        }

        public int Level
        {
            set
            {
                if(value < 0)
                {
                    Console.WriteLine("Level sıfırn altında olamaz.");
                }
                else
                {
                    level = value;
                }
            }
            get { return level; }
        }

        public int Gold
        {
            set
            {
                if(value < 0)
                {
                    gold = 0;
                }
                else
                {
                    gold = value;
                }
            }
            get { return gold; }
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

            if (EquippedArmor != null)
            {
                int absorbedDamage;

                if (incomingDamage < EquippedArmor.Defense)
                {
                    absorbedDamage = incomingDamage;
                }
                else
                {
                    absorbedDamage = EquippedArmor.Defense;
                }

                EquippedArmor.Durability -= absorbedDamage;
                Console.WriteLine($"[Sistem] {EquippedArmor.Name} zırhı hasarın {absorbedDamage} kadarını emdi! (Zırhın Kalan Canı: {EquippedArmor.Durability})");

                netDamage -= absorbedDamage;

                if (EquippedArmor.Durability <= 0)
                {
                    Console.WriteLine($"\nÇAAAAT! {Name} üzerindeki {EquippedArmor.Name} paramparça oldu!\n");
                    EquippedArmor = null;
                }
            }

            if (netDamage < 0)
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

        public void GainXP(int amount)
        {
            Xp += amount;
            Console.WriteLine($"\n[Sistem] {Name} savaştan {amount} XP kazandı! (Toplam XP: {Xp})");

            if (Xp >= 100)
            {
                LevelUp();
            }
        }

        public void GainGold(int amount)
        {
            if(amount <= 0)
            {
                gold += 0;
                Console.WriteLine("Negatif değer olamaz.");
            }
            else
            {
                gold += amount;
                Console.WriteLine($"[Sistem] {Name} savaştan {amount} Altın kazandı! Güncel Bakiye: {gold}).");
            }
        }

        private void LevelUp()
        {
            Level++;
            Xp -= 100;

            Health += 20;
            Damage += 5;
            Mana += 15;

            Console.WriteLine($"\n=======================================");
            Console.WriteLine($"🌟 LEVEL UP! {Name} Seviye Atladı! 🌟");
            Console.WriteLine($"Yeni Seviye: {Level}");
            Console.WriteLine($"Yeni Statlar -> Can: {Health} | Hasar: {Damage} | Mana: {Mana}");
            Console.WriteLine($"=======================================\n");
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
        private int durability;
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

        public int Durability
        {
            set
            {
                if(value < 0)
                {
                    durability = 0;
                }
                else
                {
                    durability = value;
                }
            }
            get { return  durability; }
        }

        public Armor(string name, int defense, int durability)
        {
            Name = name;
            Defense = defense;
            Durability = durability;
        }
    }

    class HealthPotion : IUsable, IPurchasable
    {
        private int healAmount;
        public int HealAmount
        {
            set
            {
                if(value < 0)
                {
                    healAmount = 0;
                }
                else
                {
                    healAmount = value;
                }
            }
            get { return healAmount; }
        }
        public int Price { get; set; }
        public string Name { get; set; }

        public HealthPotion(int healAmount, int price, string name)
        {
            HealAmount = healAmount;
            Price = price;
            Name = name;
        }

        public void Use(Character target)
        {
            target.Health += HealAmount;
            Console.WriteLine($"[Sistem] İksir içildi can {HealAmount} kadar yenilendi. {target.Name} yeni sağlık değeri: {target.Health})");

        }
    }

    class Shop
    {
        public List<IPurchasable> Stack { get; private set; } = new List<IPurchasable>();

        public void AddProduct(IPurchasable item)
        {
            Stack.Add(item);
        }

        public void ShowProducts()
        {
            int index = 1;
            foreach(IPurchasable item in Stack)
            {
                Console.WriteLine($"{index}. {item.Name} - Fiyat: {item.Price} Altın");
                index++;
            }
        }

        public void BuyItem(Character player, int index)
        {
            IPurchasable selectedItem = Stack[index];

            if(player.Gold < selectedItem.Price)
            {
                Console.WriteLine("Bu eşyayı satın almak için yeterli altının yok.");
            }
            else
            {
                player.Gold -= selectedItem.Price;

                if(selectedItem is IUsable usableIem)
                {
                    player.Inventory.Add(usableIem);
                    Console.WriteLine($"[Sistem] {selectedItem.Name} satın alındı ve çantaya eklendi!");
                }
            }
        }
    }

    interface IUsable
    {
        void Use(Character target);
    }

    interface IPurchasable
    {
        int Price { get; set; }
        string Name { get; set; }

    }
}



